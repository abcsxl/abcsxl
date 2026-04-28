using abcsxl.Data;
using abcsxl.Extensions;
using abcsxl.Helpers;
using abcsxl.Models.Enums;
using abcsxl.Models.Entities;
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
        var posts = _context.Posts
            .Include(p => p.Author)
            .Where(p => p.Status == PostStatus.Published)
            .OrderByDescending(p => p.PublishedAt ?? p.CreatedAt)
            .Take(count)
            .Select(p => new HomePostViewModel
            {
                Id = p.Id,
                Title = p.Title,
                Excerpt = p.GetExcerpt(100),
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

    public IActionResult Results(string? q, string tab = "all")
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return View(new SearchResultViewModel { ActiveTab = tab });
        }

        q = q.Trim();
        var model = new SearchResultViewModel { Query = q, ActiveTab = tab };

        if (tab == "all" || tab == "post")
        {
            var posts = _context.Posts
                .Include(p => p.Author)
                .Where(p => p.Status == PostStatus.Published)
                .Where(p => p.Title.Contains(q) || p.Content.Contains(q) || (p.Excerpt != null && p.Excerpt.Contains(q)))
                .OrderByDescending(p => p.PublishedAt ?? p.CreatedAt)
                .Take(20)
                .Select(p => new PostResultItem
                {
                    Id = p.Id,
                    Title = p.Title,
                    Excerpt = p.GetExcerpt(150),
                    Author = p.Author.Username,
                    PublishedAt = DateTimeHelper.ToChinaStandardTime(p.PublishedAt ?? p.CreatedAt),
                    Url = Url.Action("Detail", "Post", new { id = p.Slug })
                })
                .ToList();
            model.Posts = posts;
            model.TotalCount = posts.Count;
        }

        if (tab == "all" || tab == "category")
        {
            var categories = _context.Categories
                .Where(c => c.Name.Contains(q) || (c.Description != null && c.Description.Contains(q)))
                .OrderByDescending(c => c.Name)
                .Take(10)
                .Select(c => new CategoryResultItem
                {
                    Id = c.Id,
                    Name = c.Name,
                    Slug = c.Slug,
                    PostCount = c.Posts.Count,
                    Url = Url.Action("Detail", "Category", new { slug = c.Slug })
                })
                .ToList();
            model.Categories = categories;
            model.TotalCount += categories.Count;
        }

        if (tab == "all" || tab == "tag")
        {
            var tags = _context.Tags
                .Where(t => t.Name.Contains(q))
                .OrderByDescending(t => t.Name)
                .Take(10)
                .Select(t => new TagResultItem
                {
                    Id = t.Id,
                    Name = t.Name,
                    Slug = t.Slug,
                    PostCount = t.Posts.Count,
                    Url = Url.Action("Detail", "Tag", new { slug = t.Slug })
                })
                .ToList();
            model.Tags = tags;
            model.TotalCount += tags.Count;
        }

        return View(model);
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