# 开发规范

## 命名规范

### C#

| 类型 | 规则 | 示例 |
|------|------|------|
| 命名空间 | PascalCase，与目录结构一致 | `abcsxl.Services.Authentication` |
| 类 / 接口 / 记录 | PascalCase，接口以 `I` 开头 | `LocalAuthenticationService`, `IAuthenticationService` |
| 公共方法 | PascalCase，动词或动词短语 | `SignInAsync`, `BuildCategoryHierarchy` |
| 私有方法 | PascalCase | `GenerateSlug` |
| 公共属性 | PascalCase | `UserName`, `IsActive` |
| 私有字段 | `_camelCase` | `_context`, `_logger` |
| 局部变量 | camelCase | `userId`, `postList` |
| 常量 | PascalCase | `MaxRetryCount` |
| 枚举 | PascalCase 类型，PascalCase 值 | `UserRole.Admin` |
| 布尔属性 | `Is` / `Has` / `Can` 前缀 | `IsActive`, `HasChildren`, `CanEdit` |
| 异步方法 | `Async` 后缀 | `SignInAsync`, `GetUserAsync` |

### 文件 / 目录

| 类型 | 规则 | 示例 |
|------|------|------|
| 控制器 | `{Entity}Controller.cs` | `PostController.cs` |
| 视图 | 与 action 同名（PascalCase） | `Index.cshtml`, `Details.cshtml` |
| Razor 部分视图 | `_` 前缀 | `_Layout.cshtml`, `_TagSidebar.cshtml` |
| ViewModel | 完整描述性名称 | `IndexPageViewModel`, `PostDetailViewModel` |
| EF 实体 | 单数名词 | `Post`, `Category` |
| DbSet | 复数名词 | `Posts`, `Categories` |
| 表名 | 单数名词（PascalCase） | `Post`, `Category` |

## 代码风格

### 通用

- **空集合初始化**：`= []`（集合表达式，.NET 8+）
- **字符串插值**：优先 `$"..."`，避免 `string.Format`
- **可空性**：项目启用 `Nullable=enable`，所有引用类型必须明确可空性
- **隐式 using**：项目启用 `ImplicitUsings=enable`
- **文件作用域命名空间**：推荐 `namespace xxx;` 形式（注意与现有块作用域命名空间混用时保持局部一致）
- **依赖注入**：构造函数注入，私有字段以 `_` 前缀
- **async/await**：I/O 操作必须异步，方法名以 `Async` 结尾
- **DateTime**：存储一律 UTC（`DateTime.UtcNow`），显示用 `DateTimeHelper.ToChinaStandardTime()` 转 UTC+8

### 控制器

- 所有需要授权的控制器或 action 加 `[Authorize(Roles = "Admin")]`
- POST action 必须加 `[ValidateAntiForgeryToken]`
- 修改/创建使用 `[Bind(...)]` 白名单防止 over-posting
- 控制器**不直接持有业务逻辑**：复杂业务下沉到 `Services/`

### Entity / DbContext

- 实体主键一律 `Guid`，由应用层生成
- 关系在 `ApplicationDbContext.OnModelCreating` 中配置
- 软删除用 `IsDeleted` + `HasQueryFilter`
- 不要在实体中持有逻辑；行为通过 `Extensions/` 扩展方法

### Views

- 优先使用 Tag Helpers（`asp-controller`, `asp-action`, `asp-for`）
- 避免在视图中写 C# 业务逻辑
- 重复 UI 抽到 `_Layout.cshtml` 或 Partial
- 启用客户端验证：`_ValidationScriptsPartial` + `jquery-validation-unobtrusive`

## 前端规范

### 主题

- **前台**：StartBootstrap Clean Blog 主题（Bootstrap 5）
- **后台**：Bootstrap 5 + Bootstrap Icons

### CSS

- 优先使用 Bootstrap 5 组件
- 自定义样式集中在 `wwwroot/css/admin.css`、`sidebar.css`
- **禁止硬编码颜色**，使用 CSS 变量
- 避免 `!important`

### JavaScript

- 使用 jQuery（项目既有依赖）
- 后台编辑器：Vditor（Markdown）
- 前台无 SPA；必要时用 jQuery + AJAX

## Git 提交规范

使用 [Conventional Commits](https://www.conventionalcommits.org/zh-hans/)：

```
<type>(<scope>): <subject>

<body>

<footer>
```

### Type

| Type | 用途 |
|------|------|
| `feat` | 新功能 |
| `fix` | Bug 修复 |
| `docs` | 文档变更 |
| `style` | 代码格式（不影响逻辑） |
| `refactor` | 重构（既不是新功能也不是 Bug 修复） |
| `perf` | 性能优化 |
| `test` | 测试相关 |
| `chore` | 构建/工具/依赖变更 |

### Scope

可选，用 `()` 包裹，指出影响的范围：`auth`, `admin`, `post`, `category`, `tag`, `ci`, `deps` 等。

### 示例

```bash
feat(auth): introduce IAuthenticationService abstraction
fix(admin): require Authorize attribute on all admin controllers
docs: reorganize doc/ folder and add architecture.md
chore(ci): upgrade GitHub Actions to .NET 10
```

## EF Core 工作流

### 新增字段

```bash
# 1. 修改实体类
# 2. 生成迁移
dotnet ef migrations add AddPostSubtitle -o Data/Migrations --project src/Web/abcsxl
# 3. 检查生成的 Up()/Down() 方法
# 4. 应用迁移
dotnet ef database update --project src/Web/abcsxl
```

### 重置数据库（开发环境）

```bash
# SQLite：直接删除 .db 文件
rm src/Web/abcsxl/abcsxl.db
# 重启应用会自动 EnsureCreated + Seed
```

## CI/CD

### 工作流

| 文件 | 触发 | 作用 |
|------|------|------|
| `.github/workflows/build.yml` | push / PR 到 master | 编译 + 测试（.NET 10） |
| `.github/workflows/docker-image.yml` | 手动触发 | 推送到 GHCR |
| `.github/workflows/publish-docker.yml` | Release / 手动 | 推送到 Docker Hub / Quay / GHCR |

### 本地验证

提交前：

```bash
dotnet build abcsxl.sln
dotnet ef migrations add SomeChange --project src/Web/abcsxl  # 若改了实体
```

## 调试

- 开发环境：VS / VS Code + C# Dev Kit
- 日志：`appsettings.Development.json` 中 `LogLevel: Default=Information, Microsoft.AspNetCore=Warning`
- EF Core SQL 日志：在 `Program.cs` 临时添加 `optionsBuilder.LogTo(Console.WriteLine)` 调试
- Cookie 调试：浏览器 DevTools → Application → Cookies

## 不推荐的做法

- ❌ 在控制器直接 `new` 服务（用 DI）
- ❌ 业务逻辑写在 Razor 视图
- ❌ `DateTime.Now` 存数据库（用 `DateTime.UtcNow`）
- ❌ 在 `catch` 中吞掉异常
- ❌ 用 `[Bind]` 黑名单（必须白名单）
- ❌ 硬编码凭据 / 密钥进代码
- ❌ 提交前不 `dotnet build` 验证
- ❌ 跳过 `[ValidateAntiForgeryToken]`
- ❌ 在生产环境使用 `EnsureDeletedAsync()` 自动重置
