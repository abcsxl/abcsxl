# ABCSXL — Agent Guide

## 技术栈
- ASP.NET Core MVC (.NET 10), EF Core 10, SQLite (默认) / PostgreSQL (Docker)
- Bootstrap 5, jQuery, Vditor, StartBootstrap Clean Blog 主题
- BCrypt.Net-Next, Markdig, SixLabors.ImageSharp, Cookie 认证

## 常用命令
```bash
# 运行应用
dotnet run --project src/Web/abcsxl

# 构建
dotnet build abcsxl.sln

# EF Core 迁移（首次脚手架）
dotnet ef migrations add <名称> -o Data/Migrations --project src/Web/abcsxl
dotnet ef database update --project src/Web/abcsxl

# Docker
docker compose up -d
```

## 目录结构
- `src/Web/abcsxl/` — 单个 ASP.NET 项目（入口：`Program.cs`）
- `Controllers/` — 前台控制器；`Areas/Admin/Controllers/` — 后台管理控制器
- `Models/Entities/` — EF 实体（所有主键为 `Guid`）；`Models/Enums/`；`Models/ViewModels/`
- `Data/` — `ApplicationDbContext`、`SeedData.cs`、`Migrations/`
- `Extensions/` — 扩展方法；`Helpers/` — 工具类
- `wwwroot/lib/` — 通过 `libman.json` 管理的前端依赖

## 关键注意事项
- 认证：基于 Cookie。种子用户 `admin` / `Admin@123`（`Data/SeedData.cs:54`）
  **AccountController 使用硬编码凭据**（`admin@example.com`/`123456`）——不查数据库
- 路由：区域 `{area:exists}/{controller=Dashboard}/{action=Index}/{id?}`，默认 `{controller=Home}/{action=Index}/{id?}`
- 图片上传：`POST /api/Upload/image` → 自动转为 WebP（最大宽度 1920px，质量 80）
- 数据库连接：Docker 中通过环境变量 `ConnectionStrings__DefaultConnection` 切换
- 时间显示：通过 `DateTimeHelper.ToChinaStandardTime()` 转换为 UTC+8
- CI 使用 .NET 8.0，但项目目标框架为 `net10.0` —— CI 配置已过时
- 无测试项目
- Admin 和 Tag 控制器**没有授权过滤器** —— 任意访客均可访问 `/Admin/*` 和 `/Tag/Create`

## 重要规则
- **Git 操作**：不要自动执行 git add/commit/push 等任何 git 操作。需要我确认后再执行。
- **代码编写** 任何时候不要着急给代码，尽量等我确认。每次改之前先把相关影响都排查清楚，然后定出todo方案，再确认后实施代码编写。
- **注释规范**：严禁使用带圈数字、项目符号、几何图形等 Unicode 装饰性符号。（乘号 ×、比较符 ≥、箭头 →、货币符号 ¥ 等语义符号不受限）
- **代码质量**：实施前做完整审查，一次性交干净代码，不留冗余或明显可预见的缺陷等用户指出。用户指出的每处问题 = 我本应在实施前主动发现并解决的。

## 代码实施约束

**原则**：每次实际编写或修改代码前，必须获得用户明确确认。
**总体流程**：方案 → 确认 → 实施，中间不跳步。

**触发范围**（以下任一情况都需要确认）：
- 新增代码文件或函数
- 修改现有代码逻辑（包括修复bug）
- 删除或注释代码
- 重构（变量重命名、提取函数等结构性改动）

**确认流程**：
1. AI先输出修改方案（格式：影响哪些文件 → 具体改动点 → 预期结果）
2. 询问：“**是否确认开始实施？** (请回复：确认/否)”
3. 等待用户回复”确认”或等效关键词（yes/是/ok/好/可以/同意）
4. 收到确认后才开始编写代码

**例外情况**：
- 如果用户在本轮对话中明确说“不需要确认，直接改”，则跳过确认直到当前任务完成
- 仅分析、解释、搜索代码、文档更新不需要确认

**禁止行为**：
- 不得将“已输出方案”视为“已获得确认”
- 不得因“改动很小”而自行跳过确认