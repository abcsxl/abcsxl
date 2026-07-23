using abcsxl.Data;
using abcsxl.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace abcsxl.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SettingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SettingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var settings = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);

            ViewBag.BlogName = GetSetting(settings, "BlogName", "偶尔勤快的烂笔头");
            ViewBag.BlogDescription = GetSetting(settings, "BlogDescription", "以无生之觉悟为有生之事业，以悲观之心情过乐观之生活");
            ViewBag.SEOKeywords = GetSetting(settings, "SEOKeywords", "博客,技术,分享");
            ViewBag.EnableComments = GetSetting(settings, "EnableComments", "true");
            ViewBag.CommentModeration = GetSetting(settings, "CommentModeration", "false");
            ViewBag.HomePageSize = GetSetting(settings, "HomePageSize", "4");
            ViewBag.ListPageSize = GetSetting(settings, "ListPageSize", "10");
            ViewBag.RecentPostCount = GetSetting(settings, "RecentPostCount", "5");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(
            string BlogName, string BlogDescription, string SEOKeywords,
            string EnableComments, string CommentModeration,
            string HomePageSize, string ListPageSize, string RecentPostCount)
        {
            var settings = new Dictionary<string, string>
            {
                ["BlogName"] = BlogName ?? "",
                ["BlogDescription"] = BlogDescription ?? "",
                ["SEOKeywords"] = SEOKeywords ?? "",
                ["EnableComments"] = EnableComments ?? "false",
                ["CommentModeration"] = CommentModeration ?? "false",
                ["HomePageSize"] = HomePageSize ?? "4",
                ["ListPageSize"] = ListPageSize ?? "10",
                ["RecentPostCount"] = RecentPostCount ?? "5"
            };

            foreach (var item in settings)
            {
                var setting = await _context.Settings.FirstOrDefaultAsync(s => s.Key == item.Key);
                if (setting != null)
                {
                    setting.Value = item.Value;
                    setting.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    _context.Settings.Add(new Setting
                    {
                        Key = item.Key,
                        Value = item.Value,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "设置已保存！";

            return RedirectToAction(nameof(Index));
        }

        private static string GetSetting(Dictionary<string, string> settings, string key, string defaultValue)
        {
            return settings.TryGetValue(key, out var value) ? value : defaultValue;
        }
    }
}