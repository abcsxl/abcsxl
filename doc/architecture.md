# 架构设计

## 技术栈

| 类别 | 技术 | 版本 | 用途 |
|------|------|------|------|
| 框架 | ASP.NET Core MVC | .NET 10.0 | Web 框架 |
| ORM | Entity Framework Core | 10.0.7 | 数据访问 |
| 数据库 | SQLite | 3.x | 默认数据库（零配置） |
| 数据库 | PostgreSQL | 16 | Docker 部署（可切换） |
| 密码加密 | BCrypt.Net-Next | 4.1.0 | 密码哈希 |
| Markdown 渲染 | Markdig | 1.1.3 | 服务端 Markdown → HTML |
| 图片处理 | SixLabors.ImageSharp | 3.1.12 | 上传图片转 WebP |
| 前端 UI | Bootstrap 5 + jQuery | 5.3.x / 3.7.x | UI 框架 |
| Markdown 编辑 | Vditor | 3.11.2 | 后台编辑器 |
| 前端主题 | StartBootstrap Clean Blog | 6.0.9 | 前台模板 |
| 认证 | Cookie Authentication | 内置 | 本地认证 |
| 客户端依赖管理 | libman | - | `wwwroot/lib/` |

## 架构模式

**MVC**（Model-View-Controller）+ 服务层 + 仓储模式（DbContext 直注）。

```
┌─────────────────────────────────────────────────┐
│  Presentation Layer                             │
│  Controllers + Razor Views                      │
└─────────────────┬───────────────────────────────┘
                  │
┌─────────────────▼───────────────────────────────┐
│  Service Layer                                  │
│  Services/ (Authentication)                     │
└─────────────────┬───────────────────────────────┘
                  │
┌─────────────────▼───────────────────────────────┐
│  Domain Layer                                   │
│  Models/Entities + Models/Enums + Extensions    │
└─────────────────┬───────────────────────────────┘
                  │
┌─────────────────▼───────────────────────────────┐
│  Infrastructure Layer                           │
│  Data/ApplicationDbContext + EF Migrations      │
└─────────────────────────────────────────────────┘
```

## 项目结构

```
src/Web/abcsxl/
├── Program.cs                     # 应用入口（最小托管模型）
├── Controllers/                   # 前台 MVC 控制器
│   ├── HomeController.cs          # 首页、搜索、关于、错误
│   ├── PostController.cs          # 文章列表、详情
│   ├── CategoryController.cs      # 公开分类展示
│   ├── TagController.cs           # 公开标签展示（mutating 需 Admin）
│   ├── AccountController.cs       # 登录/登出/资料
│   └── UploadController.cs        # 图片上传 API
│
├── Areas/Admin/                   # 后台管理区域
│   ├── Controllers/
│   │   ├── DashboardController.cs
│   │   ├── PostController.cs
│   │   ├── CategoryController.cs
│   │   └── SettingController.cs
│   ├── Models/ViewModels/
│   └── Views/
│
├── Models/
│   ├── Entities/                  # 9 个 EF 实体（主键均为 Guid）
│   │   ├── User.cs
│   │   ├── Post.cs
│   │   ├── Category.cs            # 自引用层级
│   │   ├── Tag.cs
│   │   ├── Comment.cs
│   │   ├── PostCategory.cs        # 多对多连接实体
│   │   ├── PostTag.cs
│   │   ├── VisitLog.cs
│   │   └── Setting.cs
│   ├── Enums/                     # PostStatus / UserRole / CommentStatus
│   └── ViewModels/                # 按子目录分组（Account / Home / Post / ...）
│
├── Data/
│   ├── ApplicationDbContext.cs    # EF Core DbContext + Fluent 配置
│   ├── SeedData.cs                # 种子数据（admin + 演示文章）
│   └── Migrations/                # EF Core 迁移文件
│
├── Services/                      # 业务服务
│   └── Authentication/            # IAuthenticationService + Local 实现
│
├── Helpers/                       # 静态工具
│   ├── DateTimeHelper.cs          # UTC ↔ UTC+8 转换
│   ├── MarkdownHelper.cs          # 摘要生成
│   ├── CategoryHelper.cs          # 层级树构建
│   └── PaginationHelper.cs        # 分页
│
├── Extensions/                    # 实体扩展方法
│   ├── PostExtensions.cs
│   ├── CategoryExtensions.cs
│   └── TagExtensions.cs
│
├── wwwroot/
│   ├── uploads/                   # 用户上传图片（WebP）
│   ├── lib/                       # libman 管理的客户端库
│   └── ...
│
├── Properties/launchSettings.json
├── appsettings.json
├── appsettings.Development.json
├── abcsxl.csproj
├── libman.json                    # 客户端库清单
└── Dockerfile                     # 多阶段构建
```

## 路由

```csharp
// Areas 路由（Area 存在时）
{area:exists}/{controller=Dashboard}/{action=Index}/{id?}

// 默认路由
{controller=Home}/{action=Index}/{id?}
```

- 公开：`/`, `/Post`, `/Post/Details/{id}`, `/Category/Details/{id}`, `/Tag/Details/{id}`, `/Account/Login`, `/Home/Results`
- 后台：`/Admin`, `/Admin/Post`, `/Admin/Category`, `/Admin/Setting`（全部需 `[Authorize(Roles="Admin")]`）
- API：`/api/Upload/image`

## 数据库设计

### 实体清单

| 实体 | 说明 | 关系 |
|------|------|------|
| `User` | 用户 | 1:N Posts, 1:N Comments |
| `Post` | 文章 | N:1 User, M:N Category (via PostCategory), M:N Tag (via PostTag), 1:N Comments, 1:N VisitLogs |
| `Category` | 分类 | 自引用（ParentId），M:N Posts |
| `Tag` | 标签 | M:N Posts |
| `Comment` | 评论 | N:1 Post, N:1 User, 自引用（ParentId） |
| `PostCategory` | 文章-分类关联 | PK(PostId, CategoryId)，含 IsPrimaryCategory / Order |
| `PostTag` | 文章-标签关联 | PK(PostId, TagId) |
| `VisitLog` | 访问日志 | N:1 Post |
| `Setting` | 系统设置 KV | 无 |

### 关键设计

- **主键**：全部使用 `Guid`，由应用层生成
- **唯一索引**：User.{Username, Email, PhoneNumber, NationalId}、Category.Name、Tag.Name
- **软删除**：`Category` 和 `Tag` 通过 `IsDeleted` + `HasQueryFilter` 全局过滤；`Post` 用 `Status == Deleted` 表示
- **时间**：全部 UTC 存储；显示时通过 `DateTimeHelper.ToChinaStandardTime()` 转 UTC+8
- **级联删除**：
  - `User` → Post/Comment：`Restrict`（防止误删用户丢失内容）
  - `Post` → Comment/VisitLog：`Cascade`（删文章连带清理）
  - `Category` → 自引用：`Restrict`（防止循环删除）

### 索引清单

| 表 | 索引 | 类型 |
|----|------|------|
| User | Username, Email, PhoneNumber, NationalId | Unique |
| Post | Title, CreatedAt | Non-unique |
| Category | Name | Unique |
| Comment | PostId, UserId, CreatedAt | Non-unique |
| VisitLog | AccessTime, IPAddress | Non-unique |

### 枚举

```csharp
PostStatus { Draft=0, Published=1, Archived=2, Deleted=99 }
UserRole   { User=0, Author=1, Admin=2 }
CommentStatus { Pending=0, Approved=1, Rejected=2 }
```

## 服务层

### 认证服务

详见 [authentication.md](authentication.md)。

```
Services/Authentication/
├── IAuthenticationService.cs    # 接口
├── LocalAuthenticationService.cs # 本地 BCrypt + Cookie 实现
├── AuthResult.cs                # 登录结果 DTO
└── AuthenticatedUser.cs         # 已认证用户 DTO
```

未来会新增 `OidcAuthenticationService` 实现 OIDC 客户端模式，控制器零改动。

## API

| 路由 | 方法 | 权限 | 说明 |
|------|------|------|------|
| `/api/Upload/image` | POST | Admin | 图片上传，自动转 WebP（最大宽度 1920px，质量 80） |

请求：`multipart/form-data`，字段名 `file`，支持 `.jpg .jpeg .png .gif .bmp`
响应：`{ data: { succ: true, data: "/uploads/{guid}.webp" } }`

## 关键依赖注入

```csharp
// Program.cs
builder.Services.AddDbContext<ApplicationDbContext>(...);
builder.Services.AddControllersWithViews();
builder.Services.AddValidation();                                    // .NET 10 内置端点验证
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(...);                                                 // Cookie 认证
builder.Services.AddAuthorization();                                 // 授权服务
builder.Services.AddScoped<IAuthenticationService, LocalAuthenticationService>();
```

## 关键中间件顺序

```csharp
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();   // 必须在 Authorization 之前
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(...);
```

## 性能与可扩展性

- 当前定位：个人博客级别，SQLite 即可承载
- 切换 PostgreSQL：仅需修改 `ConnectionStrings__DefaultConnection` 环境变量 + 替换 `UseSqlite` 为 `UseNpgsql`（并安装 `Npgsql.EntityFrameworkCore.PostgreSQL`）
- 缓存层：未引入；高流量场景需加 `IMemoryCache` 或 Redis
- CDN：未引入；静态资源当前走 Kestrel

## 不在本架构范围

- 多租户
- 全文搜索（当前用 `LIKE '%q%'`，未引入 Elasticsearch/Meilisearch）
- 消息队列
- 微服务拆分
