# abcsxl Blog

一个基于 ASP.NET Core MVC 的个人博客系统，Markdown 编辑与展示，支持文章/分类/标签/评论/访问统计等完整功能。

## 技术栈

| 类别 | 技术 | 版本 |
|------|------|------|
| 框架 | ASP.NET Core MVC | .NET 10.0 |
| ORM | Entity Framework Core | 10.0.7 |
| 数据库 | SQLite (默认) / PostgreSQL (Docker) | - |
| 密码加密 | BCrypt.Net-Next | 4.1.0 |
| Markdown 渲染 | Markdig | 1.1.3 |
| 图片处理 | SixLabors.ImageSharp | 3.1.12 |
| 前端 UI | Bootstrap 5 + jQuery | 5.3.x / 3.7.x |
| Markdown 编辑器 | Vditor | 3.11.2 |

## 快速开始

### 准备
- .NET 10.0 SDK
- 可选：Docker（用于 PostgreSQL 部署）

### 启动（SQLite 默认）

```bash
dotnet run --project src/Web/abcsxl
```

访问 `http://localhost:5145`（HTTP）或 `https://localhost:7066`（HTTPS）。

首次启动会自动创建 SQLite 数据库并写入种子数据。

### 启动（PostgreSQL via Docker）

```bash
docker run -d --name pg_abcsxl -p 5432:5432 \
  -e POSTGRES_DB=abcsxl \
  -e POSTGRES_USER=abcsxl \
  -e POSTGRES_PASSWORD=abcsxl \
  -v pgdata_abcsxl:/var/lib/postgresql/data \
  postgres:16
```

设置环境变量后启动：
```bash
export ConnectionStrings__DefaultConnection="Host=localhost;Database=abcsxl;Username=abcsxl;Password=abcsxl"
dotnet run --project src/Web/abcsxl
```

## 目录结构

```
abcsxl/
├── src/Web/abcsxl/              # 唯一 ASP.NET 项目
│   ├── Controllers/             # 前台控制器
│   ├── Areas/Admin/             # 后台管理区域
│   │   ├── Controllers/
│   │   ├── Models/ViewModels/
│   │   └── Views/
│   ├── Models/
│   │   ├── Entities/            # EF Core 实体（主键均为 Guid）
│   │   ├── Enums/               # 枚举
│   │   └── ViewModels/          # 视图模型（按领域分子目录）
│   ├── Data/                    # EF Core DbContext + SeedData + Migrations
│   ├── Services/                # 业务服务（Authentication 等）
│   ├── Helpers/                 # 工具类
│   ├── Extensions/              # 扩展方法
│   └── wwwroot/                 # 静态资源（libman 管理）
├── doc/                         # 项目文档
├── .github/workflows/           # CI/CD
├── docker-compose.dcproj
└── abcsxl.sln
```

## 核心功能

- **前台**：首页文章列表、文章列表（分页+分类/标签筛选）、文章详情、搜索、登录
- **后台**（`/Admin`）：仪表盘、文章 CRUD、分类层级管理、标签管理、系统设置
- **API**：`POST /api/Upload/image`（图片上传，自动转 WebP）
- **认证**：Cookie 认证，BCrypt 密码哈希

## 常用命令

```bash
# 构建
dotnet build abcsxl.sln

# EF Core 迁移
dotnet ef migrations add <Name> -o Data/Migrations --project src/Web/abcsxl
dotnet ef database update --project src/Web/abcsxl

# Docker
docker compose up -d
```

## 文档

- [架构设计](doc/architecture.md)
- [认证机制](doc/authentication.md)
- [开发规范与 CI/CD](doc/development.md)
- [变更日志](CHANGELOG.md)
- [历史文档](doc/history/)

## License

本项目为个人博客系统，按原样提供。
