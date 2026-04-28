# ABCSXL 分类与标签模块修改计划

## 一、修改目标

1. **分类**: 1对多关系（Post ← Category），支持层级结构，点击父分类聚合显示子分类文章
2. **标签**: 1对多关系（Post ← Tag），支持一文章多标签
3. **后台管理**: 修复 Admin 区域文章管理的分类/标签保存
4. **数量统计**: 修复侧边栏各分类/标签的文章数量统计

---

## 二、问题汇总

### 2.1 后台管理问题

| # | 问题 | 优先级 | 位置 |
|---|------|--------|------|
| 1 | 后台无认证保护，任何人可访问 `/Admin` | 🔴 Critical | Admin Controllers |
| 2 | 创建文章未保存分类关联 | 🔴 Critical | PostController.Create():122 |
| 3 | 创建文章未保存标签关联 | 🔴 Critical | PostController.Create():169 |
| 4 | 编辑文章未正确加载/更新分类 | 🔴 Critical | PostController.Edit() |
| 5 | 编辑文章未正确加载/更新标签 | 🔴 Critical | PostController.Edit() |
| 6 | 编辑页面 SelectList 使用 `PasswordHash` | 🔴 Critical | PostController:193,229 |
| 7 | 分类管理 Controller 不存在 | 🟠 High | 缺失文件 |
| 8 | 标签管理 Controller 不存在 | 🟠 High | 缺失文件 |
| 9 | 评论管理 Controller 不存在 | 🟠 High | 缺失文件 |
| 10 | 用户管理 Controller 不存在 | 🟠 High | 缺失文件 |

### 2.2 前台显示问题

| # | 问题 | 优先级 | 位置 |
|---|------|--------|------|
| 1 | 侧边栏分类数量统计不正确 | 🟠 High | PostController.Index |
| 2 | 侧边栏标签数量未显示 | 🟡 Medium | _TagSidebar.cshtml |
| 3 | 文章详情页不显示分类/标签 | - | 已确认不需要 |

### 2.3 数据模型问题

| # | 问题 | 优先级 | 说明 |
|---|------|--------|------|
| 1 | Category.PostCount 字段未维护 | 🟡 Medium | 保存/删除文章时未更新 |
| 2 | Tag.UsageCount 字段未维护 | 🟡 Medium | 保存/删除文章时未更新 |

---

## 三、实际代码关系（与文档描述不符）

### 当前代码实际使用的关系

```
Post → Category: 使用直接导航属性 Post.Categories
Post → Tag: 使用直接导航属性 Post.Tags
```

代码位置 `Models/Entities/Post.cs:115-127`:
```csharp
public ICollection<Category> Categories { get; } = [];
public ICollection<PostCategory> PostCategories { get; } = [];
public ICollection<Tag> Tags { get; } = [];
public List<PostTag> PostTags { get; } = [];
```

代码位置 `Models/Entities/Category.cs:91`:
```csharp
public ICollection<PostCategory> PostCategories { get; } = [];
```

代码位置 `Models/Entities/Tag.cs:62`:
```csharp
public ICollection<Post> Posts { get; } = [];
```

> **问题**: 代码混用了直接导航属性和显式关联表，但实际保存只用了直接导航属性，导致关联表（PostCategory, PostTag）未被使用。

---

## 四、详细修改计划

### 阶段一：后台核心修复（Priority 1）

#### 1.1 修复 PostController.Create() 保存逻辑

**文件**: `Areas/Admin/Controllers/PostController.cs`

**当前问题**: 
- CategoryId 被注释掉 (line 122)
- 使用 `post.Tags.Add(tag)` 无效 (line 169)

**修改方案**:
```csharp
// 保存分类 - 使用直接导航属性
if (model.CategoryId.HasValue && model.CategoryId != Guid.Empty)
{
    var category = await _context.Categories.FindAsync(model.CategoryId.Value);
    if (category != null)
    {
        post.Categories.Add(category);
        // 更新 PostCount
        category.PostCount++;
        _context.Categories.Update(category);
    }
}

// 保存标签 - 使用直接导航属性
if (!string.IsNullOrWhiteSpace(model.Tags))
{
    var tagNames = model.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Select(t => t.Trim())
        .Distinct();

    foreach (var tagName in tagNames)
    {
        var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
        if (tag == null)
        {
            tag = new Tag
            {
                Id = Guid.NewGuid(),
                Name = tagName,
                Slug = GenerateSlug(tagName),
                CreatedAt = DateTime.UtcNow
            };
            await _context.Tags.AddAsync(tag);
        }
        
        post.Tags.Add(tag);
        // 更新 UsageCount
        tag.UsageCount++;
        _context.Tags.Update(tag);
    }
}
```

#### 1.2 修复 PostController.Edit() 加载和保存逻辑

```csharp
[HttpGet]
public async Task<IActionResult> Edit(Guid? id)
{
    var post = await _context.Posts
        .Include(p => p.Categories)
        .Include(p => p.Tags)
        .FirstOrDefaultAsync(p => p.Id == id);

    var model = new PostCreateViewModel
    {
        Id = post.Id,
        Title = post.Title,
        CategoryId = post.Categories.FirstOrDefault()?.Id,
        Tags = string.Join(", ", post.Tags.Select(t => t.Name)),
        // ...
    };

    return View(model);
}
```

#### 1.3 修复 SelectList 问题

**位置**: PostController:193, 229

```csharp
// 错误
ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "PasswordHash", post.AuthorId);

// 修复为
ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Username", post.AuthorId);
```

---

### 阶段二：认证与安全修复（Priority 0）

#### 2.1 配置 Cookie 认证

**文件**: `Program.cs`

```csharp
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    });

builder.Services.AddAuthorization();
```

#### 2.2 添加认证特性

```csharp
[Area("Admin")]
[Authorize]
public class DashboardController : Controller { }

[Area("Admin")]
[Authorize]
public class PostController : Controller { }
```

---

### 阶段三：前台显示修复（Priority 2）

#### 3.1 修复分类数量统计

**文件**: `Controllers/PostController.cs` 的 `Index` 方法

```csharp
// Categories - 使用实际关系
var categories = await _context.Categories
    .Where(c => !c.IsDeleted)
    .Include(c => c.Posts)
    .Select(c => new CategorySidebarItemViewModel
    {
        Id = c.Id,
        Name = c.Name,
        Slug = c.Slug,
        PostCount = c.Posts.Count(p => p.Status == PostStatus.Published)
    })
    .ToListAsync();

// Tags
var tags = await _context.Tags
    .Where(t => !t.IsDeleted)
    .Select(t => new TagSidebarItemViewModel
    {
        Id = t.Id,
        Name = t.Name,
        PostCount = t.Posts.Count
    })
    .ToListAsync();
```

---

### 阶段四：后台管理完善（Priority 3）

#### 4.1 新增分类管理 Controller

**文件**: `Areas/Admin/Controllers/CategoryController.cs`

```csharp
[Area("Admin")]
[Authorize]
public class CategoryController : Controller
{
    private readonly ApplicationDbContext _context;

    public CategoryController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories
            .Where(c => !c.IsDeleted)
            .Include(c => c.Parent)
            .OrderBy(c => c.Order)
            .ToListAsync();

        return View(categories);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category)
    {
        if (ModelState.IsValid)
        {
            category.Id = Guid.NewGuid();
            category.CreatedAt = DateTime.UtcNow;
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(category);
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound();

        var allCategories = await _context.Categories
            .Where(c => !c.IsDeleted && c.Id != id)
            .ToListAsync();
        ViewBag.ParentCategories = new SelectList(allCategories, "Id", "Name", category.ParentId);

        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Category category)
    {
        if (id != category.Id) return NotFound();

        if (ModelState.IsValid)
        {
            category.UpdatedAt = DateTime.UtcNow;
            _context.Update(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category != null)
        {
            category.IsDeleted = true;
            category.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
```

#### 4.2 新增标签管理 Controller

**文件**: `Areas/Admin/Controllers/TagController.cs`

```csharp
[Area("Admin")]
[Authorize]
public class TagController : Controller
{
    private readonly ApplicationDbContext _context;

    public TagController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var tags = await _context.Tags
            .Where(t => !t.IsDeleted)
            .Include(t => t.Posts)
            .ToListAsync();

        return View(tags);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Tag tag)
    {
        if (ModelState.IsValid)
        {
            tag.Id = Guid.NewGuid();
            tag.CreatedAt = DateTime.UtcNow;
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(tag);
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null) return NotFound();
        return View(tag);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Tag tag)
    {
        if (id != tag.Id) return NotFound();

        if (ModelState.IsValid)
        {
            tag.UpdatedAt = DateTime.UtcNow;
            _context.Update(tag);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(tag);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag != null)
        {
            tag.IsDeleted = true;
            tag.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
```

---

## 五、修改文件清单

### 需要修改的文件

| 文件 | 修改内容 |
|------|----------|
| `Program.cs` | 添加 Cookie 认证配置 |
| `Areas/Admin/Controllers/PostController.cs` | 修复 Create/Edit 保存逻辑，修复 SelectList |
| `Controllers/PostController.cs` | 修复分类/标签数量统计 |

### 需要新增的文件

| 文件 | 说明 |
|------|------|
| `Areas/Admin/Controllers/CategoryController.cs` | 分类管理 |
| `Areas/Admin/Controllers/TagController.cs` | 标签管理 |
| `Areas/Admin/Views/Category/Index.cshtml` | 分类列表页 |
| `Areas/Admin/Views/Category/Create.cshtml` | 创建分类页 |
| `Areas/Admin/Views/Category/Edit.cshtml` | 编辑分类页 |
| `Areas/Admin/Views/Tag/Index.cshtml` | 标签列表页 |
| `Areas/Admin/Views/Tag/Create.cshtml` | 创建标签页 |
| `Areas/Admin/Views/Tag/Edit.cshtml` | 编辑标签页 |

---

## 六、实施顺序

```
┌─────────────────────────────────────────────────────────────┐
│ 阶段零：安全紧急                                             │
│ ├─ 配置 Cookie 认证                                           │
│ ├─ 添加 [Authorize] 特性                                      │
│ └─ 修复硬编码密码                                            │
├─────────────────────────────────────────────────────────────┤
│ 阶段一：后台核心修复                                          │
│ ├─ 1. 修复 PostController.Create() 保存逻辑                   │
│ ├─ 2. 修复 PostController.Edit() 加载和保存逻辑               │
│ └─ 3. 修复 SelectList 使用 Username 代替 PasswordHash       │
├─────────────────────────────────────────────────────────────┤
│ 阶段二：前台显示修复                                          │
│ ├─ 4. 修复侧边栏分类/标签数��统计                            │
│ └─ 5. 修复分类/标签链接                                     │
├─────────────────────────────────────────────────────────────┤
│ 阶段三：后台管理完善                                          │
│ ├─ 6. 新增 CategoryController                              │
│ ├─ 7. 新增 TagController                                  │
│ └─ 8. 新增 CommentController                              │
└─────────────────────────────────────────────────────────────┘
```

---

## 七、注意事项

1. **直接导航属性**: 代码中使用 `Post.Categories` 和 `Post.Tags` 直接导航属性，而非显式关联表。

2. **软删除**: Category 和 Tag 使用 `IsDeleted` 字段软删除，查询时应使用 `.Where(c => !c.IsDeleted)`

3. **分类聚合**: 点击父分类时，需要递归获取所有子分类 ID 进行筛选

4. **PostCount/UsageCount**: 创建/删除文章时需要更新分类和标签的计数字段