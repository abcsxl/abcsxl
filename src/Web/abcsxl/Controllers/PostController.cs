using abcsxl.Data;
using abcsxl.Helpers;
using abcsxl.Models.Enums;
using abcsxl.Models.ViewModels;
using abcsxl.Models.ViewModels.Post;
using abcsxl.Models.ViewModels.Category;
using abcsxl.Models.ViewModels.Tag;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace abcsxl.Controllers
{
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Posts
        public async Task<IActionResult> Index(
            int page = 1, 
            int pageSize = 10,
            string? category = null,  // 添加分类参数
            string? tag = null)       // 添加标签参数
        {
            if (page <= 0) page = 1;
            if (pageSize < 1) pageSize = 10;

            var query = from s in _context.Posts select s;

            query = query
                .Where(p => p.Status == PostStatus.Published)
                .Include(p => p.Author)
                .Include(p => p.Categories)
                .Include(p => p.Tags)
                ;

            // 根据分类筛选
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.Categories.Any(c => c.Name == category));
            }

            // 根据标签筛选
            if (!string.IsNullOrWhiteSpace(tag))
            {
                query = query.Where(p => p.Tags.Any(t => t.Name == tag));
            }

            query = query.OrderByDescending(p => p.PublishedAt ?? p.CreatedAt);

            var totalcount = await query.CountAsync();
            var totalpages = (int)Math.Ceiling(totalcount / (double)pageSize);

            if (page > totalpages) page = totalpages;
            if (pageSize > totalcount) pageSize = 10;


            var posts = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PostItemViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Excerpt = p.Excerpt ?? MarkdownHelper.GenerateExcerptFromMarkdown(p.Content),
                    Categories = p.Categories.Select(c => new CategoryItemViewModel { Id = c.Id, Name = c.Name }).ToList(),
                    Tags = p.Tags.Select(t => new TagItemViewModel { Id = t.Id, Name = t.Name }).ToList(),
                    Author = new AuthorViewModel { Id = p.AuthorId, Name = p.Author.Username },
                    PublishedTime = DateTimeHelper.ToChinaStandardTime(p.PublishedAt ?? p.CreatedAt)
                });

            // Categories - 计算每个分类的文章数量（包含子分类）
            var allCategories = await _context.Categories
                .Where(c => !c.IsDeleted)
                .Include(c => c.Posts)
                .ToListAsync();

            // 构建分类层级，并递归计算文章数
            var categories = CategoryHelper.BuildCategoryHierarchyWithCount(allCategories, null);

            // Tags - 计算每个标签的文章数量
            var allTags = await _context.Tags
                .Where(t => !t.IsDeleted)
                .Include(t => t.Posts)
                .ToListAsync();

            var tagsWithCount = allTags.Select(t => new TagItemViewModel
            {
                Id = t.Id,
                Name = t.Name,
                PostCount = t.Posts.Count
            }).ToList();

            var model = new IndexPageViewModel
            {
                Posts = await posts.ToListAsync(),
                TotalCount = totalcount,
                TotalPages = totalpages,
                CurrentPage = page,
                PageSize = pageSize,

                SidebarCategories = new CategorySidebarViewModel { Categories = categories, CurrentCategory= category },
                SidebarTags = new TagSidebarViewModel { Tags = tagsWithCount, CurrentTag=tag }
            };
            return View(model);
        }

        // GET: Post/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Categories)
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            var model = new DetailPageViewModel
            {
                Post = new PostDetailViewModel
                {
                    Id = post.Id,
                    Author = new AuthorViewModel { Id = post.AuthorId, Name = post.Author.Username },
                    Categories = post.Categories.Select(c => new abcsxl.Models.ViewModels.Category.CategoryItemViewModel { Id = c.Id, Name = c.Name }).ToList(),
                    Tags = post.Tags.Select(t => new abcsxl.Models.ViewModels.Tag.TagItemViewModel { Id = t.Id, Name = t.Name }).ToList(),
                    Content = post.Content,
                    Title = post.Title,
                    CoverImage = post.CoverImage,
                    CoverImageThumb = post.CoverImageThumb,
                    CreatedAt = post.CreatedAt.ToChinaStandardTime(),
                    Excerpt = post.Excerpt ?? MarkdownHelper.GenerateExcerptFromMarkdown(post.Content),
                    IsAllowComments = post.IsAllowComments,
                    LikeCount = post.LikeCount,
                    MetaDescription = post.MetaDescription,
                    MetaKeywords = post.MetaKeywords,
                    MetaTitle = post.MetaTitle,
                    PublishedAt = DateTimeHelper.ToChinaStandardTime(post.PublishedAt ?? post.CreatedAt),
                    ReadingMinutes = post.ReadingMinutes,
                    Slug = post.Slug,
                    Subtitle = post.Subtitle,
                    UpdatedAt = DateTimeHelper.ToChinaStandardTime(post.UpdatedAt ?? post.CreatedAt),
                    ViewCount = post.ViewCount
                }
            };

            return View(model);
        }
    }
}
