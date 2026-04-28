namespace abcsxl.Models.ViewModels.Tag
{
    /// <summary>
    /// 侧边栏标签数据
    /// </summary>
    public class TagSidebarViewModel
    {
        /// <summary>
        /// 所有标签列表
        /// </summary>
        public List<TagItemViewModel> Tags { get; set; } = [];

        /// <summary>
        /// 当前筛选的标签名称
        /// </summary>
        public string? CurrentTag { get; set; }
    }

    /// <summary>
    /// 单个标签项
    /// </summary>
    public class TagItemViewModel
    {
        public Guid Id { get; set; }
        
        /// <summary>
        /// 标签名称
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// 使用此标签的文章数量
        /// </summary>
        public int PostCount { get; set; }
    }
}