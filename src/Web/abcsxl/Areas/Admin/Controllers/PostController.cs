using abcsxl.Areas.Admin.Models.ViewModels.Post;
using abcsxl.Data;
using abcsxl.Models.Entities;
using abcsxl.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace abcsxl.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Post
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Posts.Include(p => p.Author);
            return View(await applicationDbContext.ToListAsync());
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
            // 手动验证
            if (string.IsNullOrWhiteSpace(model.Title))
            {
                ModelState.AddModelError("Title", "请输入文章标题");
            }
            else if (model.Title.Length < 2)
            {
                ModelState.AddModelError("Title", "标题长度不能少于2个字符");
            }
            else if (model.Title.Length > 200)
            {
                ModelState.AddModelError("Title", "标题长度不能超过200个字符");
            }

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

            // ============ 保存文章逻辑（GUID版本）============
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
                //CategoryId = model.CategoryId,  // 假设 CategoryId 也是 Guid?
                //FeaturedImage = model.FeaturedImage,
                Status = model.Status,
                //AllowComments = model.AllowComments,
                //IsPinned = model.IsPinned,
                PublishedAt = model.Status == PostStatus.Published ? (model.PublishAt ?? DateTime.UtcNow) : null,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                AuthorId = GetCurrentUserId()  // 假设 AuthorId 是 Guid
            };

            // 保存到数据库
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            // 处理标签
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
                            Id = Guid.NewGuid(),  // GUID 主键
                            Name = tagName,
                            Slug = GenerateSlug(tagName),
                            CreatedAt = DateTime.UtcNow
                        };
                        await _context.Tags.AddAsync(tag);
                        await _context.SaveChangesAsync();
                    }

                    //// 更新标签文章计数
                    //tag.PostCount++;
                    //_context.Tags.Update(tag);

                    //var postTag = new PostTag
                    //{
                    //    PostId = post.Id,
                    //    TagId = tag.Id
                    //};
                    post.Tags.Add(tag);
                    //await _context.PostTags.AddAsync(postTag);
                }
                await _context.SaveChangesAsync();
            }
            // ============ 保存文章逻辑结束 ============


            return RedirectToAction(nameof(Edit), new { id = post.Id });
        }

        // GET: Admin/Post/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "PasswordHash", post.AuthorId);
            return View(post);
        }

        // POST: Admin/Post/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Subtitle,Content,Slug,Excerpt,CoverImageThumb,CoverImage,MetaTitle,MetaDescription,MetaKeywords,ViewCount,LikeCount,ReadingMinutes,IsFeatured,IsAllowComments,CreatedAt,UpdatedAt,DeletedAt,PublishedAt,Status,AuthorId")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "PasswordHash", post.AuthorId);
            return View(post);
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
