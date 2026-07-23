# Changelog

所有重要变更记录于此文件。格式基于 [Keep a Changelog](https://keepachangelog.com/zh-CN/1.1.0/)。

## [Unreleased]

### Added
- 认证抽象层 `IAuthenticationService` 与 `LocalAuthenticationService` 实现
- Admin 区域 4 个控制器加 `[Authorize(Roles = "Admin")]`
- `TagController` 加类级 `[Authorize(Roles = "Admin")]`，公开 `Index`/`Details` 显式 `[AllowAnonymous]`
- Cookie 认证服务注册（`AddAuthentication().AddCookie()`）与 `app.UseAuthentication()` 中间件
- `Admin/CategoryController.Edit` 加 `[Bind]` 白名单防止 over-posting
- `TagController.Create/Edit` 收紧 `[Bind]` 移除 `IsDeleted` 等字段

### Changed
- `AccountController.Login` 不再使用硬编码 `admin@example.com/123456`，改为通过 `IAuthenticationService` 查 `User` 表 + BCrypt 校验
- 登录成功签发 `ClaimTypes.NameIdentifier`（UserId），修复 `Admin/PostController` 新建文章时 `AuthorId` 写随机 GUID 的问题
- 种子 admin 用户 `abcsxl/Admin@123` 现在是真实可用的登录账号
- `Program.cs` 连接字符串缺失时的错误消息从 `WebApplication1Context` 修正为 `DefaultConnection`
- `Program.cs` 启动失败时不再 `EnsureDeletedAsync()` 清库重建，改为抛出异常让运维介入
- CI 工作流（`.github/workflows/build.yml`）升级 .NET 8.0 → 10.0，启用 NuGet 缓存
- `Admin/PostController` 3 处 `DateTime.Now` → `DateTime.UtcNow`（`CreatedAt` / `UpdatedAt`）
- `HomeController.Results` 搜索结果 URL 修正：3 处 `Url.Action("Detail", ...)` → `Url.Action("Details", ...)`（匹配实际 action 名）
- 所有视图 `.cshtml` 添加多语言支持（`IStringLocalizer` / `IViewLocalizer`）
- 资源文件从 `SixLabors.ImageSharp` 切换到 `SkiaSharp`
- 新增 `FlatFileStringLocalizerFactory` 自定义资源加载器
- 语言自动检测支持 QueryString → Cookie → Accept-Language → zh-CN 回退

### Fixed
- Admin 区域越权访问：未登录用户访问 `/Admin/*` 跳转登录页
- `TagController` mutating action 越权：未登录用户访问 `/Tag/Create` 等跳转登录页
- 登录后无法获取 `UserId`，新建文章 `AuthorId` 始终为随机 GUID
- 搜索结果点击跳转到 404（action 名错误）
- `Admin/PostController.GetCurrentUserId()` 查 `"UserId"` claim 但 `LocalAuthenticationService` 签发的是 `ClaimTypes.NameIdentifier`，导致 `AuthorId` 始终为随机 Guid
- 搜索结果中文章/分类/标签链接传递 Slug 但 Controller 期望 Guid → 搜索跳转 404
- `Admin/SettingController.Save` 缺少 `[ValidateAntiForgeryToken]`
- `Admin/PostController.Create` 中 `PublishAt` 初始化使用 `DateTime.Now` 而非 `UtcNow`

## [1.0.1] - 2026-04-28

详见 [历史变更文档](doc/history/v1.0.1-changelog.md)

### Added
- 文章管理分页（支持搜索、状态筛选）
- 分类管理分页（支持搜索）
- 系统设置模块（博客基本配置持久化）
- 搜索功能（支持文章、分类、标签搜索及 tab 筛选）
- 标签侧边栏悬停动效
- 文章默认封面图

### Fixed
- 编辑文章按钮跳转错误（改用 `Create` 视图复用）
- Markdown 摘要生成优化（移除表格、图片、代码块）

### Changed
- Admin 页面风格统一
- `ViewModels` 结构重构
- 分类辅助方法增强
- 系统设置控制器重构

## [1.0.0] - 2026-04-XX

初始版本。详见 [历史设计文档](doc/history/v1.0.0-design.md)

### Added
- 前台：首页、文章列表、文章详情、登录、搜索
- 后台：仪表盘、文章管理、分类管理、标签管理、系统设置
- 数据库：User / Post / Category / Tag / Comment / PostCategory / PostTag / VisitLog / Setting
- 图片上传 API（自动转 WebP）
- BCrypt 密码加密
- Markdig Markdown 渲染
- Vditor 编辑器
- Docker 部署支持

[Unreleased]: https://github.com/user/repo/compare/v1.0.1...HEAD
[1.0.1]: https://github.com/user/repo/compare/v1.0.0...v1.0.1
[1.0.0]: https://github.com/user/repo/releases/tag/v1.0.0
