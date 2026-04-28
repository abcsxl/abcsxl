namespace abcsxl.Models.ViewModels.Post
{
    /// <summary>
    /// 文章列表页数据
    /// </summary>
    public class IndexPageViewModel
    {
        /// <summary>
        /// 文章列表
        /// </summary>
        public List<PostItemViewModel> Posts { get; set; } = [];
        
        /// <summary>
        /// 侧边栏分类
        /// </summary>
        public Category.CategorySidebarViewModel? SidebarCategories { get; set; }
        
        /// <summary>
        /// 侧边栏标签
        /// </summary>
        public Tag.TagSidebarViewModel? SidebarTags { get; set; }

        /// <summary>
        /// 每页显示数量
        /// </summary>
        public int PageSize { get; set; } = 10;
        
        /// <summary>
        /// 总文章数
        /// </summary>
        public int TotalCount { get; set; }
        
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get; set; }
        
        /// <summary>
        /// 当前页码
        /// </summary>
        public int CurrentPage { get; set; }
        
        /// <summary>
        /// 是否有上一页
        /// </summary>
        public bool HasPrevious => CurrentPage > 1;
        
        /// <summary>
        /// 是否有下一页
        /// </summary>
        public bool HasNext => CurrentPage < TotalPages;
    }

    /// <summary>
    /// 列表中的单篇文章
    /// </summary>
    public class PostItemViewModel
    {
        public Guid Id { get; set; }
        
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; } = string.Empty;
        
        /// <summary>
        /// 摘要
        /// </summary>
        public string Excerpt { get; set; } = string.Empty;
        
        /// <summary>
        /// 所属分类
        /// </summary>
        public List<Category.CategoryItemViewModel> Categories { get; set; } = [];
        
        /// <summary>
        /// 标签
        /// </summary>
        public List<Tag.TagItemViewModel> Tags { get; set; } = [];
        
        /// <summary>
        /// 作者
        /// </summary>
        public AuthorViewModel Author { get; set; } = null!;
        
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime PublishedTime { get; set; }
    }

    /// <summary>
    /// 文章作者信息
    /// </summary>
    public class AuthorViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}