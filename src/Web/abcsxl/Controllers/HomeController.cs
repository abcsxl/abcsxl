using abcsxl.Data;
using abcsxl.Extensions;
using abcsxl.Helpers;
using abcsxl.Models.Enums;
using abcsxl.Models.ViewModels;
using abcsxl.Models.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace abcsxl.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, IConfiguration configuration, ApplicationDbContext context)
    {
        _logger = logger;
        _configuration = configuration;
        _context = context;
    }

    public IActionResult Index(int count = 4)
    {
        // 获取最新count篇已发布文章
        var posts = _context.Posts
            .Where(p => p.Status == PostStatus.Published)
            .OrderByDescending(p => p.PublishedAt ?? p.CreatedAt)
            .Take(count)
            .Select(p => new HomePostViewModel
            {
                Id = p.Id,
                Title = p.Title,
                Excerpt = p.GetExcerpt(200),
                Author = p.Author.Username,
                PublishedAt = DateTimeHelper.ToChinaStandardTime(p.PublishedAt ?? p.CreatedAt)
            })
            .ToList();

        var model = new HomeIndexViewModel
        {
            LastPosts = posts,
            LastCount = count
        };

        return View(model);
    }

    public IActionResult Results()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
