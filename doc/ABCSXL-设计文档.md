# ABCSXL 博客系统 - 详细设计文档

## 一、项目概述

### 1.1 项目简介

**ABCSXL** 是一个基于 ASP.NET Core MVC 的博客网站系统，采用 Entity Framework Core 进行数据持久化，支持 Markdown 文章编辑与展示，具有完整的用户认证、分类标签管理、文章管理等功能。

### 1.2 技术栈

| 类别 | 技术选型 | 版本 |
|------|----------|------|
| 框架 | ASP.NET Core MVC | .NET 10.0 |
| ORM | Entity Framework Core | 10.0.7 |
| 数据库 | SQLite (默认) | 3.x |
| 密码加密 | BCrypt.Net-Next | 4.1.0 |
| Markdown处理 | Markdig | 1.1.3 |
| 图片处理 | SixLabors.ImageSharp | 3.1.12 |
| 前端框架 | Bootstrap 5 + jQuery | 5.3.x / 3.7.x |
| Markdown编辑器 | Vditor | 3.x |

### 1.3 项目结构

```
src/Web/abcsxl/
├── Controllers/          # 前台控制器
├── Areas/Admin/         # 管理后台区域
│   ├── Controllers/
│   └── Views/
├── Models/
│   ├── Entities/      # 实体类
│   ├── Enums/        # 枚举定义
│   └── ViewModels/    # 视图模型
├── Data/             # 数据访问层
│   ├── Migrations/
│   ├── ApplicationDbContext.cs
│   └── SeedData.cs
├── Helpers/           # 工具类
├── Extensions/       # 扩展方法
└── wwwroot/          # 静态资源
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
- **页面**: `Views/Home/Index.cshtml`

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
- **页面**: `Views/Post/Index.cshtml`

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
  - SEO元数据
- **页面**: `Views/Post/Details.cshtml`

#### 2.1.4 账户管理 (AccountController)

- **功能**: 用户登录、登出、资料管理、密码修改
- **路由**:
  - `/Account/Login` - 登录页
  - `/Account/Logout` - 登出
  - `/Account/Profile` - 个人资料
  - `/Account/ChangePassword` - 修改密码
- **认证方式**: Cookie认证 (未配置)
- **页面**:
  - `Views/Account/Login.cshtml`
  - `Views/Account/Profile.cshtml`
  - `Views/Account/ChangePassword.cshtml`

#### 2.1.5 静态页面

- `/Home/About` - 关于页
- `/Home/Privacy` - 隐私政策页

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
  - 封面图上传
  - 分类选择 (单选)
  - 标签选择 (多选，逗号分隔)
  - 状态设置 (草稿/已发布/已归档)

> **⚠️ 已知问题**:
> - 创建文章时分类/标签关联未正确保存
> - 编辑页面 SelectList 显示字段错误

#### 2.2.3 分类管理

- **功能**: 分类的层级管理
- **路由**: `/Admin/Category` (TODO - 未实现)
- **特性**:
  - 树形层级结构
  - 软删除
  - 显示/隐藏控制
  - 导航栏显示设置

#### 2.2.4 标签管理

- **功能**: 标签的CRUD操作
- **路由**: `/Admin/Tag` (TODO - 未实现)
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
| DisplayName | string | | 显示名称 |
| Bio | string | | 个人简介 |
| Avatar | string | | 头像URL |
| Role | UserRole | NOT NULL | 角色 |
| IsActive | bool | NOT NULL | 账号是否激活 |
| CreatedAt | DateTime | NOT NULL | 创建时间 |
| LastLoginTime | DateTime | | 最后登录时间 |

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
| IsAllowComments | bool | DEFAULT false | 允许评论 |
| MetaTitle | string | | SEO标题 |
| MetaDescription | string | | SEO描述 |
| MetaKeywords | string | | SEO关键词 |
| CreatedAt | DateTime | NOT NULL | 创建时间 |
| UpdatedAt | DateTime | | 更新时间 |
| PublishedAt | DateTime | | 发布时间 |
| DeletedAt | DateTime | | 删除时间 |
| Status | PostStatus | NOT NULL | 状态 |

**关系** (实际代码中使用直接导航属性):
- N:1 -> User (作者)
- 1:N -> Category (直接导航 `Categories`)
- 1:N -> Tag (直接导航 `Tags`)
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
| ShowInNav | bool | DEFAULT true | 导航栏显示 |
| Order | int | DEFAULT 0 | 排序 |
| PostCount | int | DEFAULT 0 | 文章数 |
| CreatedAt | DateTime | NOT NULL | ��建时间 |
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

#### 3.1.6 PostCategory (文章分类关联表)

| 字段 | 类型 | 约束 | 说明 |
|------|------|------|------|
| PostId | GUID | PK, FK | 文章ID |
| CategoryId | GUID | PK, FK | 分类ID |
| IsPrimaryCategory | bool | DEFAULT false | 主分类 |
| Order | int | DEFAULT 0 | 排序 |
| CreatedAt | DateTime | NOT NULL | 创建时间 |

> **注意**: 代码中实际使用 `Post.Categories` 直接导航属性，此关联表未被使用。

#### 3.1.7 PostTag (文章标签关联表)

| 字段 | 类型 | 约束 | 说明 |
|------|------|------|------|
| PostId | GUID | PK, FK | 文章ID |
| TagId | GUID | PK, FK | 标签ID |
| CreatedAt | DateTime | NOT NULL | 创建时间 |

> **注意**: 代码中实际使用 `Post.Tags` 直接导航属性，此关联表未被使用。

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
```

> **⚠️ 已知问题**: 未配置 `AddAuthentication()`

---

## 五、核心组件设计

### 5.1 工具类 (Helpers)

#### DateTimeHelper
- `ToChinaStandardTime()` - UTC转中国时区
- `BeijingNow` - 获取当前中国时间

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

#### CategoryExtensions
- `GetFullPath()` - 获取分类全路径
- `GetLevel()` - 获取分类层级

#### TagExtensions
- `IncrementUsage()` / `DecrementUsage()` - 使用次数增减

### 5.3 数据上下文 (ApplicationDbContext)

- 配置所有实体关系
- 配置唯一索引约束
- 配置全局查询过滤器 (Category, Tag的软删除过滤)

---

## 六、安全设计

### 6.1 认证授权

> **⚠️ 严重问题**: 当前未正确实现

- **认证方式**: Cookie Authentication (未配置)
- **登录机制**: 硬编码凭证 `admin@example.com/123456`
- **角色权限**: 预留但未实现

### 6.2 数据安全

- 密码使用BCrypt加密存储 (依赖已安装，未使用)
- 使用`@Html.AntiForgeryToken()`防止CSRF攻击
- 输入验证和模型验证
- 防止SQL注入 (EF Core参数化查询)

---

## 七、已知问题汇总

### 🔴 Critical (必须修复)

| # | 问题 | 位置 |
|---|------|------|
| 1 | 后台无认证保护，任何人可访问 `/Admin` | Admin Controllers |
| 2 | 认证服务未配置 | Program.cs |
| 3 | 硬编码密码凭证 | AccountController:38 |
| 4 | 创建文章未保存分类关联 | PostController.Create() |
| 5 | 创建文章未保存标签关联 | PostController.Create() |
| 6 | 编辑文章 SelectList 显示字段错误 | PostController:193,229 |

### 🟠 High (重要)

| # | 问题 | 位置 |
|---|------|------|
| 1 | 分类管理 Controller 不存在 | ��失文件 |
| 2 | 标签管理 Controller 不存在 | 缺失文件 |
| 3 | 评论管理 Controller 不存在 | 缺失文件 |
| 4 | 侧边栏分类数量统计不正确 | PostController.Index |
| 5 | 个人资料修改不保存 | AccountController |
| 6 | 密码修改功能未实现 | AccountController |

---

## 八、待完善功能

1. **用户注册系统**
2. **完整认证授权**
3. **评论系统**
4. **搜索功能**
5. **后台管理完整CRUD** (分类、标签、用户)
6. **访问统计分析**
7. **邮件通知**
8. **API扩展**
9. **缓存优化**

---

## 九、开发规范

### 9.1 命名规范

- **控制器**: `{Entity}Controller.cs`
- **视图**: 与控制器Action同名的.cshtml文件
- **实体类**: PascalCase，如 `Post`, `User`
- **枚举**: PascalCase

### 9.2 Git提交规范

```
feat: 新功能
fix: Bug修复
docs: 文档更新
style: 代码格式
refactor: 重构
perf: 性能优化
test: 测试相关
chore: 构建/工具变更
```