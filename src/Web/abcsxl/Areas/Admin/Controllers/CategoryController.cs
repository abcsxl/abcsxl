using abcsxl.Data;
using abcsxl.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace abcsxl.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 20)
        {
            var query = _context.Categories
                .Where(c => !c.IsDeleted)
                .Include(c => c.Parent)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Name.Contains(search) || c.Description.Contains(search));
            }

            var totalCount = await query.CountAsync();
            var categories = await query
                .OrderBy(c => c.Order)
                .ThenBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.TotalCount = totalCount;
            ViewBag.Search = search;

            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid? parentId)
        {
            var allCategories = await GetSelectableCategories();
            ViewBag.ParentCategories = new SelectList(allCategories, "Id", "FullPath", parentId);

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
                TempData["Success"] = "分类创建成功！";
                return RedirectToAction(nameof(Index));
            }

            var allCategories = await GetSelectableCategories();
            ViewBag.ParentCategories = new SelectList(allCategories, "Id", "FullPath", category.ParentId);
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var allCategories = await GetSelectableCategories(id);
            ViewBag.ParentCategories = new SelectList(allCategories, "Id", "FullPath", category.ParentId);

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    category.UpdatedAt = DateTime.UtcNow;
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "分类更新成功！";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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

            var allCategories = await GetSelectableCategories(id);
            ViewBag.ParentCategories = new SelectList(allCategories, "Id", "FullPath", category.ParentId);
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
                TempData["Success"] = "分类已删除！";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(Guid id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }

        private async Task<List<Category>> GetSelectableCategories(Guid? excludeId = null)
        {
            var query = _context.Categories.Where(c => !c.IsDeleted);
            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }
            return await query.OrderBy(c => c.Name).ToListAsync();
        }
    }
}