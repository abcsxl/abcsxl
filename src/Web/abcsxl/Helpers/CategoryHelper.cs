using abcsxl.Models.Entities;
using abcsxl.Models.ViewModels;

namespace abcsxl.Helpers
{
    public static class CategoryHelper
    {
        public static List<CategoryViewModel> BuildCategoryHierarchy(List<Category> allCategories, Guid? parentId)
        {

            var result = new List<CategoryViewModel>();

            var parentCategories = allCategories
                .Where(c => c.ParentId == parentId)
                //.OrderBy(c => c.DisplayOrder) // 如果有排序字段
                .OrderBy(c => c.Name);

            foreach (var category in parentCategories)
            {
                var viewModel = new CategoryViewModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    Slug = category.Slug,
                    PostCount = category.PostCount,
                    SubCategories = BuildCategoryHierarchy(allCategories, category.Id)
                };

                result.Add(viewModel);
            }

            return result;
        }
    }
}
