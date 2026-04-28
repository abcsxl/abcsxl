# ABCSXL 博客系统 - 已知问题与待实现功能

## 一、已实现功能总览

### 1.1 前台功能

| 功能模块 | 完成度 | 说明 |
|----------|--------|------|
| 首页 | ✅ 完整 | 显示最新文章列表 |
| 文章列表 | ✅ 完整 | 分页展示，支持分类/标签筛选，侧边栏显示 |
| 文章详情 | ✅ 完整 | Markdown渲染，SEO支持，阅读时间估算 |
| 登录/登出 | ⚠️ 部分 | 仅硬编码账号，功能不可用 |
| 个人资料 | ❌ 假功能 | 仅UI，无实际数据保存 |
| 修改密码 | ❌ 未实现 | 页面存在但功能为空 |
| 关于/隐私页 | ✅ 完整 | 静态页面 |
| 搜索 | ❌ 未实现 | 页面存在但功能为空 |

### 1.2 后台管理

| 功能模块 | 完成度 | 说明 |
|----------|--------|------|
| 仪表盘 | ✅ 完整 | 统计数据展示 |
| 文章列表 | ✅ 完整 | 显示所有文章 |
| 创建文章 | ⚠️ 部分 | 可保存，但标签/分类关联有BUG |
| 编辑文章 | ⚠️ 部分 | SelectList 错误 |
| 删除文章 | ✅ 完整 | 物理删除 |
| 分类管理 | ❌ 未实现 | 无后台管理 |
| 标签管理 | ❌ 未实现 | 无后台管理 |
| 评论管理 | ❌ 未实现 | 无后台管理 |
| 用户管理 | ❌ 未实现 | 无后台管理 |

---

## 二、已知问题汇总

### 🔴 Critical（必须修复）

#### Bug 1: 硬编码密码凭证 - 安全漏洞
**文件**: `Controllers/AccountController.cs:38`
```csharp
if (model.Email == "admin@example.com" && model.Password == "123456")
```
**问题**: 硬编码凭证，任何人均可使用该账号登录
**修复**: 从数据库读取用户，使用 BCrypt 验证密码

#### Bug 2: 后台无认证保护
**文件**: `Areas/Admin/Controllers/DashboardController.cs`
**文件**: `Areas/Admin/Controllers/PostController.cs`
```csharp
[Area("Admin")]
public class DashboardController : Controller  // 缺少 [Authorize]
```
**问题**: 任何人可直接访问 `/Admin` 所有功能
**修复**: 添加 `[Authorize]` 特性，并配置认证服务

#### Bug 3: 认证服务未配置
**文件**: `Program.cs`
**问题**: 缺少 `AddAuthentication()` 和 `AddCookie()` 配置
**修复**: 在 `builder.Services.AddControllersWithViews()` 前添加认证配置

#### Bug 4: 文章创建 - 标签关联无效
**文件**: `Areas/Admin/Controllers/PostController.cs:169`
```csharp
post.Tags.Add(tag);  // 直接导航属性不会自动保存
```
**问题**: 标签关联不会写入数据库
**修复**: 使用显式 SaveChanges 或 Include 更新

#### Bug 5: 文章创建 - 分类关联无效
**文件**: `Areas/Admin/Controllers/PostController.cs:122`
```csharp
//CategoryId = model.CategoryId,  // 被注释掉
```
**问题**: 创建文章时选择的分类未保存
**修复**: 取消注释并正确保存关联

#### Bug 6: 编辑页面 - 作者下拉框显示字段错误
**文件**: `Areas/Admin/Controllers/PostController.cs:193,229`
```csharp
new SelectList(_context.Users, "Id", "PasswordHash", post.AuthorId)
```
**问题**: 
1. 密码哈希字段不应暴露给前端
2. 无法正确选中当前作者
**修复**: 使用 `Username` 作为显示字段

#### Bug 7: GetCurrentUserId() 返回随机 GUID
**文件**: `Areas/Admin/Controllers/PostController.cs:347-352`
```csharp
private Guid GetCurrentUserId()
{
    var userIdClaim = User.FindFirst("UserId");
    return userIdClaim != null ? new Guid(userIdClaim.Value) : Guid.NewGuid(); // 无 Claim 时返回新 GUID
}
```
**问题**: 由于登录未正确配置 Claim，创建文章时 AuthorId 随机生成
**修复**: 配合修复 Bug 2 和 3 后，使用正确获取方式

---

### 🟠 High（重要）

#### Bug 8: 文章编辑 - 分类/标签字段遗漏
**文件**: `Areas/Admin/Controllers/PostController.cs:202`
```csharp
[Bind("Id,Title,Subtitle,Content,Slug,Excerpt,...")]  // 缺少 Categories, Tags
```
**问题**: 编辑时不会更新文章的标签和分类关联
**修复**: 使用 ViewModel 或显式处理关联

#### Bug 9: 摘要生成使用正则
**文件**: `Areas/Admin/Controllers/PostController.cs:334-345`
```csharp
var plainText = Regex.Replace(content, "<[^>]*>", "");
```
**问题**: Markdown 语法未正确处理
**修复**: 复用 `MarkdownHelper.GenerateExcerptFromMarkdown()`

#### Bug 10: 侧边栏分类数量统计不正确
**文件**: `Controllers/PostController.cs`
**问题**: PostCount 字段未在文章创建/删除时更新
**修复**: 在保存文章时更新 Category.PostCount

#### Bug 11: 侧边栏标签数量未显示
**文件**: `Controllers/PostController.cs`
**问题**: Tag.UsageCount 未使用
**修复**: 在保存文章时更新 Tag.UsageCount

---

### 🟡 Medium（中等）

#### Bug 12: 个人资料修改不保存
**文件**: `Controllers/AccountController.cs:101-114`
```csharp
// TODO: 保存到数据库
TempData["Success"] = "资料更新成功！";
```
**问题**: 仅显示成功消息，未实际更新数据库

#### Bug 13: 密码修改功能未实现
**文件**: `Controllers/AccountController.cs:124-137`
```csharp
// TODO: 验证旧密码，更新新密码
ViewBag.Success = true;
```
**问题**: 密码修改功能完全未实现

#### Bug 14: 硬编码数据库连接字符串
**文件**: `Program.cs:14`
```csharp
"WebApplication1Context"  // 默认连接名错误
```
**问题**: 异常信息引用旧项目名

#### Bug 15: 中文乱码
**文件**: `Program.cs:38`
```csharp
logger.LogError(ex, "���ݿ��ʼ��ʧ��");  // "数据库初始化失败"
```
**问题**: 文件编码导致中文乱码

#### Bug 16: 文章列表分页计算错误
**文件**: `Controllers/PostController.cs:62`
```csharp
.Take(totalcount < pageSize ? totalcount : pageSize)
```
**问题**: 当总记录数 < 每页大小时，取值异常

---

### 🟢 Low（轻微）

#### Bug 17: 文章详情页面包屑作者链接错误
**文件**: `Views/Post/Details.cshtml`
```html
<a href="#!">@Model.Post.Author.Name</a>
```
**问题**: 作者链接为死链接

#### Bug 18: 后台导航栏 Badge 硬编码
**文件**: `Areas/Admin/Views/Shared/_AdminLayout.cshtml`
```html
<span class="badge">12</span>
```
**问题**: 数字硬编码，应动态获取

---

## 三、待实现功能清单

### 3.1 必须实现（安全/功能）

1. **用户注册系统**
   - 注册页面 (`/Account/Register`)
   - 邮箱验证
   - 密码强度校验

2. **完整认证授权**
   - Cookie Authentication 配置
   - `[Authorize]` 属性保护
   - 角色权限控制 (Admin/Author/User)

3. **评论系统**
   - 评论展示与提交 UI
   - 评论审核管理后台

4. **搜索功能**
   - 全文搜索支持

5. **后台管理完整CRUD**
   - 分类管理 (创建/编辑/删除)
   - 标签管理
   - 评论管理
   - 用户管理

### 3.2 建议实现

6. **访问统计分析**
   - 热门文章排行
   - 访问趋势图

7. **邮件通知**
   - 新评论通知
   - 密码重置邮件

8. **API扩展**
   - RESTful API
   - OpenAPI/Swagger

9. **性能优化**
   - Redis 缓存
   - 图片懒加载

10. **SEO增强**
    - Sitemap
    - RSS/Atom

---

## 四、修复优先级顺序

```
第一阶段（安全紧急 - 必须先修复）:
├─ Bug 2: 后台无认证保护
├─ Bug 3: 认证服务未配置
├─ Bug 1: 硬编码密码
└─ Bug 7: GetCurrentUserId 随机 GUID

第二阶段（功能可用）:
├─ Bug 4: 标签关联无效
├─ Bug 5: 分类关联无���
��─ Bug 6: SelectList 显示字段错误
├─ 待实现 1-2: 用户注册、完整认证
└─ 待实现 4: 搜索功能

第三阶段（体验优化）:
├─ Bug 8: 编辑页面遗漏字段
├─ Bug 9-11: 统计相关
├─ Bug 12-13: 资料/密码修改
└─ Bug 16: 分页计算错误

第四阶段（长期完善）:
├─ 待实现 5: 后台完整 CRUD
├─ 待实现 6-10
└─ 改进建议
```

---

## 五、测试验证清单

修复后应验证以下场景:

- [ ] 使用数据库中的用户凭证登录成功
- [ ] 未登录用户访问 `/Admin/*` 自动跳转登录页
- [ ] 创建文章后，标签和分类正确关联
- [ ] 编辑文章后，所有字段正确更新
- [ ] 个人资料修改正确保存
- [ ] 密码修改验证旧密码
- [ ] 分类软删除后不在前台显示