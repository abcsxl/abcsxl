namespace abcsxl.Models.ViewModels
{
    public class CategorySidebarViewModel
    {
        public List<CategoryViewModel> Categories { get; set; } = [];

        public string? CurrentCategory { get; set; } = null;
    }

    public class CategoryViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public int PostCount { get; set; } = 0;
        public List<CategoryViewModel> SubCategories { get; set; } = new();

        // 辅助属性
        public bool HasSubCategories => SubCategories?.Any() == true;
        public string Url => $"/category/{Slug}";
    }
}
