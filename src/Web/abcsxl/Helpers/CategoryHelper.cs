using abcsxl.Models.Entities;
using abcsxl.Models.Enums;
using abcsxl.Models.ViewModels.Category;

namespace abcsxl.Helpers
{
    public static class CategoryHelper
    {
        public static List<CategoryItemViewModel> BuildCategoryHierarchy(List<Category> allCategories, Guid? parentId)
        {

            var result = new List<CategoryItemViewModel>();

            var parentCategories = allCategories
                .Where(c => c.ParentId == parentId)
                //.OrderBy(c => c.DisplayOrder) // 如果有排序字段
                .OrderBy(c => c.Name);

            foreach (var category in parentCategories)
            {
                var viewModel = new CategoryItemViewModel
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

        // 递归计算分类文章数（包含子分类）
        public static List<CategoryItemViewModel> BuildCategoryHierarchyWithCount(List<Category> allCategories, Guid? parentId)
        {
            var result = new List<CategoryItemViewModel>();

            var parentCategories = allCategories
                .Where(c => c.ParentId == parentId)
                .OrderBy(c => c.Name);

            foreach (var category in parentCategories)
            {
                // 统计当前分类及其所有子分类的文章数
                var postCount = CountPostsInCategory(allCategories, category.Id);

                var viewModel = new CategoryItemViewModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    Slug = category.Slug,
                    PostCount = postCount,
                    SubCategories = BuildCategoryHierarchyWithCount(allCategories, category.Id)
                };

                result.Add(viewModel);
            }

            return result;
        }

        // 递归统计分类及其子分类的文章数
        private static int CountPostsInCategory(List<Category> allCategories, Guid categoryId)
        {
            var category = allCategories.FirstOrDefault(c => c.Id == categoryId);
            if (category == null) return 0;

            // 当前分类的文章数
            int count = category.Posts?.Count(p => p.Status == PostStatus.Published) ?? 0;

            // 递归统计所有子分类的文章数
            var childCategories = allCategories.Where(c => c.ParentId == categoryId);
            foreach (var child in childCategories)
            {
                count += CountPostsInCategory(allCategories, child.Id);
            }

            return count;
        }
    }
}
