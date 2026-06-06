using abcsxl.Areas.Admin.Models.ViewModels.Post;
using abcsxl.Data;
using abcsxl.Models.Entities;
using abcsxl.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace abcsxl.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Post
        public async Task<IActionResult> Index(string? search, string? status, int page = 1, int pageSize = 10)
        {
            var query = _context.Posts.Include(p => p.Author).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Title.Contains(search) || p.Content.Contains(search));
            }

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<PostStatus>(status, out var postStatus))
            {
                query = query.Where(p => p.Status == postStatus);
            }

            var totalCount = await query.CountAsync();
            var posts = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.TotalCount = totalCount;
            ViewBag.Search = search;
            ViewBag.Status = status;

            return View(posts);
        }

        // GET: Admin/Post/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Admin/Post/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewData["Title"] = "写文章";

            var model = new PostCreateViewModel
            {
                Categories = await GetCategoriesSelectList(),
                ExistingTags = await GetExistingTags(),
                Status = PostStatus.Draft,
                PublishAt = DateTime.Now
            };

            return View(model);
        }

        // POST: Admin/Post/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostCreateViewModel model, string saveType)
        {
            // 验证标题
            if (string.IsNullOrWhiteSpace(model.Title) || model.Title.Length < 2)
            {
                ModelState.AddModelError("Title", "标题长度2-200个字符");
            }

            // 验证内容
            if (string.IsNullOrWhiteSpace(model.Content))
            {
                ModelState.AddModelError("Content", "文章内容不能为空");
            }

            if (!ModelState.IsValid)
            {
                model.Categories = await GetCategoriesSelectList(model.CategoryId);
                model.ExistingTags = await GetExistingTags();
                return View(model);
            }

            // 自动生成别名
            if (string.IsNullOrWhiteSpace(model.Slug))
            {
                model.Slug = GenerateSlug(model.Title);
            }

            // 根据 saveType 设置状态
            if (saveType == "publish")
            {
                model.Status = PostStatus.Published;
                model.PublishAt = model.PublishAt ?? DateTime.Now;
            }
            else
            {
                model.Status = PostStatus.Draft;
            }

            // 创建文章实体 - 使用 Guid
            var post = new Post
            {
                Id = Guid.NewGuid(),  // GUID 主键
                Title = model.Title,
                Slug = model.Slug,
                Content = model.Content,
                Excerpt = string.IsNullOrEmpty(model.Excerpt) ? GenerateExcerpt(model.Content) : model.Excerpt,
                Status = model.Status,
                PublishedAt = model.Status == PostStatus.Published ? (model.PublishAt ?? DateTime.UtcNow) : null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                AuthorId = GetCurrentUserId()
            };

            // 保存文章到数据库，获取 Id
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            // 处理分类（一对多：只保存一个分类）
            if (model.CategoryId.HasValue && model.CategoryId != Guid.Empty)
            {
                var category = await _context.Categories.FindAsync(model.CategoryId.Value);
                if (category != null)
                {
                    post.Categories.Add(category);
                }
            }

            // 处理标签（多对多）
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
                }
            }

            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Edit), new { id = post.Id });
        }

        // GET: Admin/Post/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Categories)
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            // 构建 ViewModel
            var model = new PostCreateViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Slug = post.Slug,
                Content = post.Content,
                Excerpt = post.Excerpt,
                FeaturedImage = post.CoverImage,
                Status = post.Status,
                AllowComments = post.IsAllowComments,
                IsPinned = post.IsFeatured,
                PublishAt = post.PublishedAt,
                
                // 加载分类
                CategoryId = post.Categories.FirstOrDefault()?.Id,
                Categories = await GetCategoriesSelectList(post.Categories.FirstOrDefault()?.Id),
                
                // 加载标签
                Tags = string.Join(", ", post.Tags.Select(t => t.Name)),
                ExistingTags = await GetExistingTags()
            };

            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Username", post.AuthorId);
            return View("Create", model);
        }

        // POST: Admin/Post/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, PostCreateViewModel model, string saveType)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                model.Categories = await GetCategoriesSelectList(model.CategoryId);
                model.ExistingTags = await GetExistingTags();
                ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Username", model.AuthorId);
                return View("Create", model);
            }

            var post = await _context.Posts
                .Include(p => p.Categories)
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            // 更新文章基本信息
            post.Title = model.Title;
            post.Slug = string.IsNullOrWhiteSpace(model.Slug) ? GenerateSlug(model.Title) : model.Slug;
            post.Content = model.Content;
            post.Excerpt = string.IsNullOrEmpty(model.Excerpt) ? GenerateExcerpt(model.Content) : model.Excerpt;
            post.CoverImage = model.FeaturedImage;
            post.UpdatedAt = DateTime.UtcNow;
            post.IsAllowComments = model.AllowComments;
            post.IsFeatured = model.IsPinned;

            // 更新状态
            if (saveType == "publish")
            {
                post.Status = PostStatus.Published;
                if (post.PublishedAt == null)
                {
                    post.PublishedAt = model.PublishAt ?? DateTime.UtcNow;
                }
            }
            else if (saveType == "draft")
            {
                post.Status = PostStatus.Draft;
            }

            // 更新分类
            post.Categories.Clear();
            if (model.CategoryId.HasValue && model.CategoryId != Guid.Empty)
            {
                var category = await _context.Categories.FindAsync(model.CategoryId.Value);
                if (category != null)
                {
                    post.Categories.Add(category);
                }
            }

            // 更新标签
            post.Tags.Clear();
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
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "文章已更新！";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Post/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Admin/Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(Guid id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }

        // 获取分类下拉框
        private async Task<List<SelectListItem>> GetCategoriesSelectList(Guid? selectedId = null)
        {
            var categories = await _context.Categories
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.Order)
                .ThenBy(c => c.Name)
                .ToListAsync();

            var list = new List<SelectListItem>
        {
            new SelectListItem { Text = "-- 选择分类 --", Value = "" }
        };

            list.AddRange(categories.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString(),
                Selected = c.Id == selectedId
            }));

            return list;
        }

        // 获取已存在的标签
        private async Task<List<string>> GetExistingTags()
        {
            return await _context.Tags
                .OrderByDescending(t => t.Posts.Count)
                .Take(20)
                .Select(t => t.Name)
                .ToListAsync();
        }

        // 生成别名
        private string GenerateSlug(string text)
        {
            // 转为小写，替换空格为连字符，移除特殊字符
            var slug = text.ToLowerInvariant()
                .Replace(" ", "-")
                .Replace("_", "-")
                .Replace(".", "")
                .Replace(",", "")
                .Replace("!", "")
                .Replace("?", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace("[", "")
                .Replace("]", "")
                .Replace("{", "")
                .Replace("}", "");

            // 移除重复连字符
            while (slug.Contains("--"))
            {
                slug = slug.Replace("--", "-");
            }

            return slug.Trim('-');
        }

        // 生成摘要
        private string GenerateExcerpt(string content, int length = 200)
        {
            // 移除 HTML 标签
            var plainText = Regex.Replace(content, "<[^>]*>", "");

            if (plainText.Length <= length)
            {
                return plainText;
            }

            return plainText.Substring(0, length) + "...";
        }

        private Guid GetCurrentUserId()
        {
            // 从 Claims 获取当前用户ID
            var userIdClaim = User.FindFirst("UserId");
            return userIdClaim != null ? new Guid(userIdClaim.Value) : Guid.NewGuid();
        }
    }
}
