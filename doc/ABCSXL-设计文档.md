# ABCSXL 博客系统 - 详细设计文档

## 一、项目概述

### 1.1 项目简介

**ABCSXL** 是一个基于 ASP.NET Core MVC 的博客网站系统，采用 Entity Framework Core 进行数据持久化，支持 Markdown 文章编辑与展示，具有完整的用户认证、分类标签管理、文章管理等功能。

### 1.2 技术栈

| 类别 | 技术选型 | 版本 |
|------|----------|------|
| 框架 | ASP.NET Core MVC | .NET 10.0 |
| ORM | Entity Framework Core | 10.0.4 |
| 数据库 | PostgreSQL / SQLite | 16 / 3.x |
| 密码加密 | BCrypt.Net-Next | 4.1.0 |
| Markdown处理 | Markdig | 1.1.1 |
| 图片处理 | SixLabors.ImageSharp | 3.1.12 |
| 前端框架 | Bootstrap 5 + jQuery | 5.3.x / 3.7.x |
| Markdown编辑器 | Vditor | 3.x |

### 1.3 项目结构

```
src/Web/abcsxl/
├── Controllers/          # 控制器层
├── Areas/Admin/         # 管理后台区域
│   ├── Controllers/
│   ├── Views/
│   └── Models/
├── Views/               # 前台视图
│   ├── Home/
│   ├── Post/
│   ├── Account/
│   └── Shared/
├── Models/              # 数据模型
│   ├── Entities/        # 实体类
│   ├── Enums/           # 枚举定义
│   └── ViewModels/      # 视图模型
├── Data/               # 数据访问层
│   ├── Migrations/     # 数据库迁移
│   ├── ApplicationDbContext.cs
│   └── SeedData.cs
├── Helpers/             # 工具类
├── Extensions/          # 扩展方法
└── wwwroot/            # 静态资源
```

---

## 二、功能模块设计

### 2.1 前台功能模块

#### 2.1.1 首页 (HomeController)

- **功能**: 显示最新发布的博客文章列表
- **路由**: `/`, `/Home`
- **展示逻辑**:
  - 默认展示最新 `count` 篇文章 (默认4篇)
  - 按发布时间倒序排列
  - 显示文章标题、摘要、作者、发布时间
- **页面**:
  - `Views/Home/Index.cshtml` - 首页

#### 2.1.2 文章列表 (PostController)

- **功能**: 分页展示所有已发布文章，支持按分类/标签筛选
- **路由**: `/Post`, `/Post/Index`
- **参数**:
  - `page` - 当前页码 (默认1)
  - `pageSize` - 每页条数 (默认10)
  - `category` - 分类筛选
  - `tag` - 标签筛选
- **侧边栏**:
  - 分类目录树形结构
  - 标签云列表
- **页面**:
  - `Views/Post/Index.cshtml` - 文章列表页

#### 2.1.3 文章详情 (PostController)

- **功能**: 展示单篇文章完整内容
- **路由**: `/Post/Details/{id}`
- **展示内容**:
  - 标题、副标题
  - 封面图/缩略图
  - Markdown渲染后的内容
  - 作者、发布时间、更新时间
  - 阅读时长估算
  - 分类、标签
  - 统计信息 (阅读数、点赞数)
- **特性**:
  - 支持评论 (需开启)
  - SEO元数据 (MetaTitle, MetaDescription, MetaKeywords)
- **页面**:
  - `Views/Post/Details.cshtml` - 文章详情页

#### 2.1.4 账户管理 (AccountController)

- **功能**: 用户登录、登出、资料管理、密码修改
- **路由**:
  - `/Account/Login` - 登录页
  - `/Account/Logout` - 登出
  - `/Account/Profile` - 个人资料
  - `/Account/ChangePassword` - 修改密码
- **认证方式**: Cookie认证
- **页面**:
  - `Views/Account/Login.cshtml`
  - `Views/Account/Profile.cshtml`
  - `Views/Account/ChangePassword.cshtml`

#### 2.1.5 静态页面

- `/Home/About` - 关于页
- `/Home/Privacy` - 隐私政策页
- `/Home/Results` - 搜索结果页

---

### 2.2 后台管理模块 (Admin Area)

#### 2.2.1 仪表盘 (DashboardController)

- **功能**: 系统概览统计
- **路由**: `/Admin`, `/Admin/Dashboard`
- **统计数据**:
  - 文章总数、已发布数
  - 评论总数
  - 用户总数
  - 分类总数
  - 标签总数
  - 最近发布的文章列表

#### 2.2.2 文章管理 (PostController in Admin)

- **功能**: 文章CRUD操作
- **路由**:
  - `/Admin/Post` - 文章列表
  - `/Admin/Post/Create` - 创建文章
  - `/Admin/Post/Edit/{id}` - 编辑文章
  - `/Admin/Post/Details/{id}` - 查看详情
  - `/Admin/Post/Delete/{id}` - 删除文章
- **编辑器**: Vditor (支持Markdown富文本编辑)
- **支持功能**:
  - 标题、副标题
  - Markdown内容
  - 封面图上传 (自动生成缩略图，WebP格式)
  - 分���选择 (支持多选，设定主分类)
  - 标签选择 (支持多选)
  - 状态设置 (草稿/已发布/已归档)
  - 评论开关
  - 定时发布
  - SEO元数据

#### 2.2.3 分类管理

- **功能**: 分类的层级管理
- **路由**: `/Category` (前台), `/Admin/Category` (后台 - TODO)
- **特性**:
  - 树形层级结构
  - 软删除
  - 显示/隐藏控制
  - 导航栏显示设置

#### 2.2.4 标签管理

- **功能**: 标签的CRUD操作
- **路由**: `/Tag` (前台), `/Admin/Tag` (后台 - TODO)
- **特性**:
  - 软删除
  - 使用统计
  - 自定义颜色

---

### 2.3 API接口

#### 2.3.1 图片上传API

- **路由**: `POST /api/upload/image`
- **功能**: 上传图片并转换为WebP格式
- **处理逻辑**:
  1. 验证文件类型 (jpg, jpeg, png, gif, bmp)
  2. 如宽度>1920px则等比压缩
  3. 转换为WebP格式 (质量80%)
  4. 生成唯一文件名并保存到 `/wwwroot/uploads/`
- **返回**: `{ succ: true, data: "/uploads/{filename}.webp" }`

---

## 三、数据库设计

### 3.1 实体模型

#### 3.1.1 User (用户表)

| 字段 | 类型 | 约束 | 说明 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| Username | string | UNIQUE, NOT NULL | 用户名 |
| PasswordHash | string | NOT NULL | BCrypt加密密码 |
| Email | string | UNIQUE | 邮箱 |
| PhoneNumber | string | UNIQUE | 手机号 |
| NationalId | string | UNIQUE | 身份证号 |
| DisplayName | string | | 显示名称 |
| Bio | string | | 个人简介 |
| Avatar | string | | 头像URL |
| Role | UserRole | NOT NULL | 角色 (User/Author/Admin) |
| IsActive | bool | NOT NULL | 账号是否激活 |
| CreatedAt | DateTime | NOT NULL | 创建时间 |
| LastLoginTime | DateTime | | 最后登录时间 |

**关系**:
- 1:N -> Post (文章)
- 1:N -> Comment (评论)

#### 3.1.2 Post (文章表)

| 字段 | 类型 | 约束 | 说明 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| Title | string | NOT NULL | 标题 |
| Subtitle | string | | 副标题 |
| Content | string | NOT NULL | Markdown内容 |
| Slug | string | | URL别名 |
| Excerpt | string | | 摘要 |
| CoverImage | string | | 封面图 |
| CoverImageThumb | string | | 封面缩略图 |
| AuthorId | GUID | FK, NOT NULL | 作者ID |
| Status | PostStatus | NOT NULL | 状态 |
| ViewCount | int | DEFAULT 0 | 阅读数 |
| LikeCount | int | DEFAULT 0 | 点赞数 |
| ReadingMinutes | int | DEFAULT 0 | 预计阅读分钟 |
| IsFeatured | bool | DEFAULT false | 置顶 |
| IsAllowComments | bool | DEFAULT true | 允许评论 |
| MetaTitle | string | | SEO标题 |
| MetaDescription | string | | SEO描述 |
| MetaKeywords | string | | SEO关键词 |
| CreatedAt | DateTime | NOT NULL | 创建时间 |
| UpdatedAt | DateTime | | 更新时间 |
| PublishedAt | DateTime | | 发布时间 |
| DeletedAt | DateTime | | 删除时间 |

**关系**:
- N:1 -> User (作者)
- N:N -> Category (通过PostCategory)
- N:N -> Tag (通过PostTag)
- 1:N -> Comment
- 1:N -> VisitLog

#### 3.1.3 Category (分类表)

| 字段 | 类型 | 约束 | 说明 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| Name | string | UNIQUE, NOT NULL | 分类名称 |
| Description | string | | 描述 |
| Slug | string | | URL别名 |
| Icon | string | | 图标 |
| CoverImage | string | | 封面图 |
| ParentId | GUID | FK | 父分类ID |
| IsDeleted | bool | DEFAULT false | 软删除 |
| IsVisible | bool | DEFAULT true | 显示 |
| ShowInNav | bool | DEFAULT false | 导航栏显示 |
| Order | int | DEFAULT 0 | 排序 |
| PostCount | int | DEFAULT 0 | 文章数 |
| CreatedAt | DateTime | NOT NULL | 创建时间 |
| UpdatedAt | DateTime | | 更新时间 |
| DeletedAt | DateTime | | 删除时间 |

**关系**:
- 自引用: 1:N (层级结构)

#### 3.1.4 Tag (标签表)

| 字段 | 类型 | 约束 | 说明 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| Name | string | UNIQUE, NOT NULL | 标签名 |
| Description | string | | 描述 |
| Slug | string | | URL别名 |
| Color | string | DEFAULT #666666 | 颜色 |
| IsDeleted | bool | DEFAULT false | 软删除 |
| IsVisible | bool | DEFAULT true | 显示 |
| UsageCount | int | DEFAULT 0 | 使用次数 |
| LastUsedAt | DateTime | | 最后使用时间 |
| CreatedAt | DateTime | NOT NULL | 创建时间 |
| UpdatedAt | DateTime | | 更新时间 |
| DeletedAt | DateTime | | 删除时间 |

#### 3.1.5 Comment (评论表)

| 字段 | 类型 | 约束 | 说明 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| Content | string | NOT NULL, MAX 1000 | 评论内容 |
| PostId | GUID | FK | 文章ID |
| UserId | GUID | FK, NOT NULL | 用户ID |
| ParentId | GUID | FK | 父评论ID |
| Status | CommentStatus | NOT NULL | 状态 |
| LikeCount | int | DEFAULT 0 | 点赞数 |
| CreatedAt | DateTime | NOT NULL | 创建时间 |
| UpdatedAt | DateTime | | 更新时间 |

**关系**:
- N:1 -> Post
- N:1 -> User
- 自引用: 1:N (嵌套回复)

#### 3.1.6 PostCategory (文章分类关联表)

| 字段 | 类型 | 约束 | 说明 |
|------|------|------|------|
| PostId | GUID | PK, FK | 文章ID |
| CategoryId | GUID | PK, FK | 分类ID |
| IsPrimaryCategory | bool | DEFAULT false | 主分类 |
| Order | int | DEFAULT 0 | 排序 |
| CreatedAt | DateTime | NOT NULL | 创建时间 |
| UpdatedAt | DateTime | | 更新时间 |
| UpdateCount | int | DEFAULT 0 | 更新次数 |
| UserId | GUID | FK | 添加人ID |

#### 3.1.7 PostTag (文章标签关联表)

| 字段 | 类型 | 约束 | 说明 |
|------|------|------|------|
| PostId | GUID | PK, FK | 文章ID |
| TagId | GUID | PK, FK | 标签ID |
| CreatedAt | DateTime | NOT NULL | 创建时间 |

#### 3.1.8 VisitLog (访问日志表)

| 字段 | 类型 | 约束 | 说明 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| PostId | GUID | FK | 文章ID |
| AccessTime | DateTime | NOT NULL | 访问时间 |
| IPAddress | string | NOT NULL | IP地址 |
| UserAgent | string | | 用户代理 |
| Referrer | string | | 来源URL |
| Country | string | | 国家 |
| City | string | | 城市 |

---

### 3.2 枚举定义

#### PostStatus (文章状态)

| 值 | 名称 | 说明 |
|----|------|------|
| 0 | Draft | 草稿 |
| 1 | Published | 已发布 |
| 2 | Archived | 已归档 |
| 99 | Deleted | 已删除 |

#### UserRole (用户角色)

| 值 | 名称 | 说明 |
|----|------|------|
| 0 | User | 普通用户 |
| 1 | Author | 作者 |
| 2 | Admin | 管理员 |

#### CommentStatus (评论状态)

| 值 | 名称 | 说明 |
|----|------|------|
| 0 | Pending | 待审核 |
| 1 | Approved | 已批准 |
| 2 | Rejected | 已拒绝 |

---

## 四、技术架构设计

### 4.1 架构模式

采用 **MVC (Model-View-Controller)** 架构模式：

- **Model**: 实体类 + ViewModel
- **View**: Razor Pages
- **Controller**: 业务逻辑处理

### 4.2 分层设计

```
Presentation Layer (表现层)
    ├── Controllers
    └── Views

Application Layer (应用层)
    └── ViewModels

Domain Layer (领域层)
    ├── Entities
    ├── Enums
    └── Extensions

Infrastructure Layer (基础设施层)
    ├── Data (EF Core)
    └── Helpers
```

### 4.3 依赖注入

在 `Program.cs` 中配置服务：

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ApplicationDbContext>();
```

---

## 五、核心组件设计

### 5.1 工具类 (Helpers)

#### DateTimeHelper
- `ToChinaStandardTime()` - UTC转中国时区
- `BeijingNow` - 获取当前中国时间
- `ToTimeZone()` - 转换到指定时区

#### MarkdownHelper
- `GenerateExcerptFromMarkdown()` - 从Markdown生成纯文本摘要

#### CategoryHelper
- `BuildCategoryHierarchy()` - 构建分类层级树

#### PaginationHelper
- `GetPaginationModel()` - 生成分页导航模型

### 5.2 扩展方法 (Extensions)

#### PostExtensions
- `GetExcerpt()` - 获取摘要
- `Publish()` - 发布文章
- `Draft()` - 设为草稿
- `SoftDelete()` - 软删除
- `IncrementViewCount()` - 增加阅读数
- `UpdateReadingMinutes()` - 更新阅读时间

#### CategoryExtensions
- `GetFullPath()` - 获取分类全路径
- `GetLevel()` - 获取分类层级
- `GetAllDescendantIds()` - 获取所有后代ID
- `MoveTo()` - 移动分类
- `SoftDelete()` / `Restore()` - 软删除/恢复

#### TagExtensions
- `IncrementUsage()` / `DecrementUsage()` - 使用次数增减
- `SoftDelete()` / `Restore()` - 软删除/恢复

### 5.3 数据上下文 (ApplicationDbContext)

- 配置所有实体关系
- 配置唯一索引约束 (User的Username, Email, PhoneNumber, NationalId)
- 配置全局查询过滤器 (Category, Tag的软删除过滤; Comment的删除文章评论过滤)
- 配置级联删除策略

---

## 六、安全设计

### 6.1 认证授权

- **认证方式**: Cookie Authentication
- **登录机制**: 用户名密码验证，BCrypt加密存储
- **角色权限**:
  - Admin: 完全访问后台管理
  - Author: 可管理自己的文章
  - User: 基本访问权限

### 6.2 数据安全

- 密码使用BCrypt加密存储
- 使用`@Html.AntiForgeryToken()`防止CSRF攻击
- 输入验证和模型验证
- 防止SQL注入 (EF Core参数化查询)

### 6.3 XSS防护

- Markdown内容在前端渲染时需注意XSS过滤
- 使用Markdig的安全模式渲染

---

## 七、部署设计

### 7.1 Docker部署

项目提供完整的Docker配置：

```yaml
# docker-compose.yml
services:
  web:
    build: .
    ports:
      - "5000:5000"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
    depends_on:
      - db

  db:
    image: postgres:16
    environment:
      POSTGRES_DB: db_abcsxl
      POSTGRES_USER: u_abcsxl
      POSTGRES_PASSWORD: password
    volumes:
      - pgdata:/var/lib/postgresql/data
```

### 7.2 数据库迁移

```bash
# 开发环境
dotnet ef migrations add InitialCreate -o Data/Migrations
dotnet ef database update

# 生产环境
dotnet ef database update -Production
```

### 7.3 环境配置

- **Development**: SQLite本地数据库，详细日志
- **Production**: PostgreSQL数据库，最小化日志

---

## 八、待完善功能 (TODO)

1. **用户管理**: 用户注册、个人资料编辑、头像上传
2. **评论功能**: 评论列表、评论审核、回复功能
3. **搜索功能**: 全文搜索支持
4. **后台管理**: 分类、标签、用户的完整CRUD
5. **访问统计**: 访问日志分析、报表展示
6. **邮件通知**: 新评论通知、密码重置
7. **API扩展**: RESTful API供移动端使用
8. **缓存优化**: Redis缓存支持
9. **性能优化**: 静态文件CDN、图片懒加载

---

## 九、开发规范

### 9.1 命名规范

- **控制器**: `{Entity}Controller.cs`
- **视图**: 与控制器Action同名的.cshtml文件
- **实体类**: PascalCase，如 `Post`, `User`
- **枚举**: PascalCase，成员大写下划线分隔，如 `PostStatus`
- **扩展方法**: 动词开头，如 `Publish()`, `SoftDelete()`

### 9.2 代码风格

- 使用C# 10+语法特性
- 空引用检查启用
- 使用文件顶部全局using
- 避免不必要的注释（除非解释复杂逻辑）

### 9.3 Git提交规范

```
feat: 新功能
fix: Bug修复
docs: 文档更新
style: 代码格式（不影响功能）
refactor: 重构
perf: 性能优化
test: 测试相关
chore: 构建/工具变更
```