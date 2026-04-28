# ABCSXL 博客系统 - 设计文档 v1.0.1

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
| 前端框架 | Bootstrap 5 + jQuery | 5.3.x / 3.7.x |
| Markdown编辑器 | Vditor | 3.x |
| 前端主题 | StartBootstrap Clean Blog | 6.0.9 |

### 1.3 项目结构

```
src/Web/abcsxl/
├── Controllers/           # 前台控制器
├── Areas/Admin/          # 管理后台区域
│   ├── Controllers/
│   └── Views/
├── Models/
│   ├── Entities/       # 实体类
│   ├── Enums/          # 枚举定义
│   └── ViewModels/     # 视图模型
├── Data/                # 数据访问层
├── Helpers/             # 工具类
├── Extensions/           # 扩展方法
└── wwwroot/             # 静态资源
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
  - 分类目录树形结构 (支持子分类)
  - 标签云列表 (悬停动效)
- **页面**: `Views/Post/Index.cshtml`

#### 2.1.3 文章详情 (PostController)

- **功能**: 展示单篇文章完整内容
- **路由**: `/Post/Details/{slug}`
- **展示内容**:
  - 标题、副标题
  - 如果无封面图则显示默认背景图
  - Markdown渲染后的内容 (支持 Mermaid 流程图)
  - 作者、发布时间
  - 分类、标签
- **页面**: `Views/Post/Details.cshtml`

#### 2.1.4 搜索 (HomeController)

- **功能**: 全文搜索文章、分类、标签
- **路由**: `/Home/Results`
- **参数**: `q` - 搜索关键词
- **选项卡**: 全部 / 文章 / 分类 / 标签
- **页面**: `Views/Home/Results.cshtml`

#### 2.1.5 账户管理 (AccountController)

- **功能**: 用户登录、登出、资料管理、密码修改
- **路由**:
  - `/Account/Login` - 登录页
  - `/Account/Logout` - 登出
  - `/Account/Profile` - 个人资料
  - `/Account/ChangePassword` - 修改密码
- **页面**: 使用 StartBootstrap Clean Blog 主题风格

---

### 2.2 后台管理模块 (Admin Area)

#### 2.2.1 仪表盘 (DashboardController)

- **功能**: 系统概览统计
- **路由**: `/Admin`, `/Admin/Dashboard`
- **统计数据**:
  - 文章总数、已发布数
  - 分类总数
  - 标签总数
  - 用户总数
  - 评论总数
- **最新文章列表** (带编辑按钮)
- **页面**: Bootstrap 卡片风格

#### 2.2.2 文章管理 (PostController in Admin)

- **功能**: 文章CRUD操作
- **路由**:
  - `/Admin/Post` - 文章列表 (分页、搜索、状态筛选)
  - `/Admin/Post/Create` - 创建文章
  - `/Admin/Post/Edit/{id}` - 编辑文章
  - `/Admin/Post/Delete/{id}` - 删除文章
- **编辑器**: Vditor (支持Markdown)
- **特性**:
  - 标题自动生成Slug
  - 手动编辑Slug
  - 分类选择 (单选)
  - 标签选择 (多选，逗号分隔)
  - 状态设置 (草稿/已发布)
  - 定时发布
  - 封面图上传
- **文章列表**:
  - 分页 (默认10条/页)
  - 搜索标题/内容
  - 筛选状态 (全部/草稿/已发布)
  - 操作按钮: 编辑、查看、删除
  - 卡片式布局

#### 2.2.3 分类管理 (CategoryController in Admin)

- **功能**: 分类CRUD操作
- **路由**:
  - `/Admin/Category` - 分类列表 (分页、搜索)
  - `/Admin/Category/Create` - 创建分类
  - `/Admin/Category/Edit/{id}` - 编辑分类
  - `/Admin/Category/Delete/{id}` - 删除分类
- **特性**:
  - 分页 (默认20条/页)
  - 搜索名称/描述
  - 上级分类选择
  - 描述、Slug、排序
  - 显示/隐藏控制
  - 导航栏显示设置

#### 2.2.4 系统设置 (SettingController in Admin)

- **功能**: 博客基本配置
- **路由**: `/Admin/Setting`
- **设置项**:
  - 博客名称
  - 博客描述
  - SEO关键词
  - 评论开关
  - 评论审核开关
  - 首页文章数
  - 列表每页文章数
  - 最新文章数

---

### 2.3 数据库表更新

#### 2.3.1 User (用户表)

无变化

#### 2.3.2 Post (文章表)

无变化

#### 2.3.3 Category (分类表)

无变化

#### 2.3.4 Tag (标签表)

无变化

#### 2.3.5 Comment (评论表)

无变化

#### 2.3.6 新增 VisitLog (访问日志表)
#### 2.3.7 新增 Setting (系统设置表)

| 字段 | 类型 | 约束 | 说明 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| Key | string | UNIQUE, NOT NULL | 设置键名 |
| Value | string | | 设置值 |
| Description | string | | 说明 |
| CreatedAt | DateTime | NOT NULL | 创建时间 |
| UpdatedAt | DateTime | | 更新时间 |

---

## 三、核心组件更新

### 3.1 工具类 (Helpers)

#### DateTimeHelper
- `ToChinaStandardTime()` - UTC转中国时区
- `BeijingNow` - 获取当前中国时间

#### MarkdownHelper
- `GenerateExcerptFromMarkdown()` - 从Markdown生成纯文本摘要 (已优化: 移除表格、图片、代码块等)

#### CategoryHelper
- `BuildCategoryHierarchy()` - 构建分类层级树
- `BuildCategoryHierarchyWithCount()` - 构建带文章数的分类层级

#### PaginationHelper
- `GetPaginationModel()` - 生成分页导航模型

### 3.2 扩展方法 (Extensions)

#### PostExtensions
- `GetExcerpt()` - 获取摘要 (调用 MarkdownHelper)
- `Publish()` - 发布文章
- `Draft()` - 设为草稿

#### CategoryExtensions
- `GetFullPath()` - 获取分类全路径

### 3.3 ViewModels (已重构)

- `CategorySidebarViewModel` + `CategoryItemViewModel`
- `TagSidebarViewModel` + `TagItemViewModel`
- `SearchResultViewModel` + `PostResultItem` + `CategoryResultItem` + `TagResultItem`

---

## 四、已知问题

### 4.1 待修复

| 问题 | 位置 |
|------|------|
| 登录未添加UserId claim | AccountController |
| 创建文章时作者外键可能为空 | PostController |
| 分类管理新增点击无反应 | 需验证 |
| 编辑按钮跳转错误 | 修改为使用Create视图 |

### 4.2 未实现功能

- 用户注册
- 完整角色权限控制
- 评论系统
- 深色主题切换
- 访问统计分析

---

## 五、开发规范

### 5.1 命名规范

- **控制器**: `{Entity}Controller.cs`
- **视图**: 与控制器Action同名的.cshtml文件
- **实体类**: PascalCase
- **ViewModel**: 完整名称如 `IndexPageViewModel`，不缩写

### 5.2 CSS规范

- 优先使用 Bootstrap 5 组件
- 特定样式使用 admin.css / sidebar.css
- 避免硬编码颜色，使用 CSS 变量

### 5.3 前端页面

- Admin Area: 使用 Bootstrap 5 + Bootstrap Icons
- 非Admin Area: 使用 StartBootstrap Clean Blog 主题

---

## 六、更新日志 v1.0.1

### 新增功能

1. **文章管理分页** - 支持搜索、状态筛选
2. **分类管理分页** - 支持搜索
3. **系统设置** - 博客基本配置持久化
4. **搜索功能** - 支持文章、分类、标签搜索及筛选
5. **标签侧边栏悬停动效**
6. **文章默认封面图**

### Bug修复

1. 编辑按钮跳转错误 - 改用Create视图
2. Markdown摘要生成优化 - 移除表格、图片、代码块
3. Admin页面风格统一

### 代码重构

1. ViewModels结构重构
2. 分类辅助方法增强
3. 系统设置控制器重构

---

*文档更新于 2026-04-28*
*项目版本: v1.0.1*