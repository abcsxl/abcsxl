# 认证机制

## 当前实现

### 流程概览

```
用户访问受保护资源
  → ASP.NET Core 检查 Cookie
  → 无 Cookie 或已失效 → 302 → /Account/Login
  → 用户提交 Email + Password
  → AccountController 调用 IAuthenticationService.SignInAsync
  → LocalAuthenticationService 查 User 表 + BCrypt 校验
  → 成功后 SignInAsync 写 Cookie（NameIdentifier / Name / Email / Role）
  → 跳转到 returnUrl 或 /Admin/Dashboard
```

### 关键代码

```csharp
// Claims 签发
var claims = new List<Claim>
{
    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
    new(ClaimTypes.Name, user.Username),
    new(ClaimTypes.Email, user.Email ?? string.Empty),
    new(ClaimTypes.Role, user.Role.ToString())
};
```

```csharp
// Program.cs 注册
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });
```

### 抽象层

通过 `IAuthenticationService` 解耦认证方式：

```csharp
public interface IAuthenticationService
{
    Task<AuthResult> SignInAsync(HttpContext, string usernameOrEmail, string password, bool rememberMe);
    Task SignOutAsync(HttpContext);
    Task<AuthenticatedUser?> GetCurrentUserAsync(ClaimsPrincipal);
}
```

当前唯一实现：`LocalAuthenticationService`（BCrypt 校验 + 查 `User` 表 + Cookie 签发）。

未来新增 OIDC 客户端实现时，仅需替换 DI 注入，控制器零改动。

## 授权模型

### 角色

`UserRole` 枚举：`User=0, Author=1, Admin=2`

种子数据：
| 用户名 | 密码 | 角色 |
|--------|------|------|
| `admin` | `Admin@123` | Admin |
| `author1` | `Author@123` | Author |
| `author2` | `Author@456` | Author |

### 授权属性

| 控制器 | 授权 |
|--------|------|
| `Admin/DashboardController` | `[Authorize(Roles = "Admin")]` |
| `Admin/PostController` | `[Authorize(Roles = "Admin")]` |
| `Admin/CategoryController` | `[Authorize(Roles = "Admin")]` |
| `Admin/SettingController` | `[Authorize(Roles = "Admin")]` |
| `TagController`（mutating） | 类级 `[Authorize(Roles = "Admin")]`，`Index`/`Details` 显式 `[AllowAnonymous]` |

所有前台其他控制器（Home/Post/Category/Account）匿名可访问。

### 不足与待完善

- `Author` 角色目前**未实际使用**：所有后台操作均要求 `Admin`
- `User` 角色目前**未实际使用**：前台无登录用户交互（评论、点赞）
- 策略授权（policy-based）未引入
- 用户注册功能缺失（仅种子数据）

## 密码存储

- 算法：BCrypt（`BCrypt.Net-Next`）
- 种子数据密码：`Admin@123` / `Author@123` / `Author@456`
- 首次启动随机生成管理员密码：尚未实现（当前使用固定密码）
- 改密码：仅前端表单，**未真正写入数据库**（`ChangePassword` action 仅有 TODO）

## 当前已知限制

1. `AccountController.Profile` 仍返回硬编码假数据（`DisplayName="管理员"` 等）
2. `AccountController.ChangePassword` 未真正修改密码
3. 登录失败信息统一为 "用户名或密码错误"（不区分账号不存在/密码错误，防枚举）
4. 同一 Cookie 在 7 天后过期，无 refresh token 机制
5. 失败登录无锁定/限流（暴力破解风险）
6. 无审计日志（登录成功/失败未记录到 `VisitLog` 或其他日志表）

## 远期：OIDC 客户端演进

> 目标：将认证外移到独立的 OIDC 认证服务（独立仓库），ABCSXL 改为 OIDC 客户端。

### 架构变化

```
┌─────────────────┐      ┌──────────────────────┐
│   ABCSXL Blog   │      │  OIDC 认证服务        │
│   (OIDC 客户端) │ ───► │  (OpenIddict Server)  │
│                 │ ◄─── │                       │
└─────────────────┘      └──────────────────────┘
   Cookie (本地会话)        User Store
   OIDC Challenge
   Token 缓存
```

### 实施阶段

| 阶段 | 范围 | 状态 |
|------|------|------|
| 1 | 本地认证加固 + `IAuthenticationService` 抽象 | **已完成** |
| 2 | 独立仓库实现 OIDC 认证服务（OpenIddict server） | 未开始 |
| 3 | ABCSXL 改造为 OIDC 客户端（authorization code + PKCE） | 未开始 |
| 4 | 用户数据迁移（一次性），双写期 | 未开始 |
| 5 | 下线 `LocalAuthenticationService` | 未开始 |

### 阶段 3 关键改动

- 新增 NuGet 包：`OpenIddict.Client.AspNetCore` 或 `Microsoft.AspNetCore.Authentication.OpenIdConnect`
- 添加 `AddOpenIdConnect("oidc", ...)` scheme，`ResponseType = "code"`，`UsePkce = true`
- `AccountController.Login` POST 改为 `HttpContext.ChallengeAsync("oidc", ...)`
- 新增 callback action 处理 authorization code 交换
- 新增 `OidcAuthenticationService : IAuthenticationService`
- Cookie 方案保留不变（仍是本地会话机制）

### 兼容性保证

- `IAuthenticationService` 接口**不会变**，所有控制器调用无需修改
- Cookie 仍是 DefaultScheme（本地会话）
- OIDC 仅作 ChallengeScheme（远端登录）
- Claims 映射（sub → NameIdentifier, role claim → Role）一次性处理

## 安全建议

- [ ] 生产环境首次启动随机生成 admin 密码，控制台打印
- [ ] 启用 `RequireConfirmedEmail` / `RequireConfirmedAccount` 标记
- [ ] 失败登录限流（如 5 次/15 分钟）
- [ ] 引入审计日志表
- [ ] Cookie 启用 `Secure` + `HttpOnly` + `SameSite=Strict`（生产环境）
- [ ] HTTPS-only Cookie
- [ ] 考虑 2FA（Google Authenticator 等）
