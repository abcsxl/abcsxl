using abcsxl.Areas.Admin.Models.ViewModels.Dashboard;
using abcsxl.Data;
using abcsxl.Helpers;
using abcsxl.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace abcsxl.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new IndexPageViewModel
            {
                TotalPosts = await _context.Posts.CountAsync(),
                PublishedPosts = await _context.Posts.CountAsync(p => p.Status == PostStatus.Published),
                TotalComments = await _context.Comments.CountAsync(),
                TotalUsers = await _context.Users.CountAsync(),
                TotalCategories = await _context.Categories.CountAsync(c => !c.IsDeleted),
                TotalTags = await _context.Tags.CountAsync(),
                RecentPosts = await _context.Posts
                   .OrderByDescending(p => p.CreatedAt)
                   .Take(5)
                   .Select(p => new IndexPagePostViewModel
                   {
                       Id = p.Id,
                       Title = p.Title,
                       PublishedAt = DateTimeHelper.ToChinaStandardTime(p.PublishedAt ?? p.CreatedAt),
                       IsPublished = p.Status == PostStatus.Published,
                       AuthorName = p.Author.Username
                   })
                   .ToListAsync()
            };

            return View(model);
        }
    }
}
