namespace abcsxl.Models.ViewModels.Category
{
    /// <summary>
    /// 侧边栏分类数据
    /// </summary>
    public class CategorySidebarViewModel
    {
        /// <summary>
        /// 分类树（顶级分类及其子分类）
        /// </summary>
        public List<CategoryItemViewModel> Categories { get; set; } = [];

        /// <summary>
        /// 当前筛选的分类名称
        /// </summary>
        public string? CurrentCategory { get; set; }
    }

    /// <summary>
    /// 单个分类项（含子分类树）
    /// </summary>
    public class CategoryItemViewModel
    {
        public Guid Id { get; set; }
        
        /// <summary>
        /// 分类名称
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// URL友好名称
        /// </summary>
        public string Slug { get; set; } = string.Empty;
        
        /// <summary>
        /// 文章数量（包含子分类）
        /// </summary>
        public int PostCount { get; set; }
        
        /// <summary>
        /// 子分类
        /// </summary>
        public List<CategoryItemViewModel> SubCategories { get; set; } = [];

        /// <summary>
        /// 是否有子分类
        /// </summary>
        public bool HasSubCategories => SubCategories?.Any() == true;
    }
}