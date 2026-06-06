namespace abcsxl.Models.ViewModels.Post
{
    /// <summary>
    /// 文章详情页数据
    /// </summary>
    public class DetailPageViewModel
    {
        /// <summary>
        /// 文章内容
        /// </summary>
        public PostDetailViewModel Post { get; set; } = null!;
    }

    /// <summary>
    /// 详情页中的文章数据
    /// </summary>
    public class PostDetailViewModel
    {
        public Guid Id { get; set; }
        
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; } = string.Empty;
        
        /// <summary>
        /// 副标题
        /// </summary>
        public string? Subtitle { get; set; }
        
        /// <summary>
        /// URL别名
        /// </summary>
        public string Slug { get; set; } = string.Empty;
        
        /// <summary>
        /// Markdown 内容
        /// </summary>
        public string Content { get; set; } = string.Empty;
        
        /// <summary>
        /// 摘要
        /// </summary>
        public string Excerpt { get; set; } = string.Empty;
        
        /// <summary>
        /// 封面图
        /// </summary>
        public string CoverImage { get; set; } = string.Empty;
        
        /// <summary>
        /// 封面缩略图
        /// </summary>
        public string CoverImageThumb { get; set; } = string.Empty;
        
        /// <summary>
        /// 作者
        /// </summary>
        public AuthorViewModel Author { get; set; } = null!;
        
        /// <summary>
        /// 分类
        /// </summary>
        public List<Category.CategoryItemViewModel> Categories { get; set; } = [];
        
        /// <summary>
        /// 标签
        /// </summary>
        public List<Tag.TagItemViewModel> Tags { get; set; } = [];
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime PublishedAt { get; set; }
        
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }
        
        /// <summary>
        /// 阅读数
        /// </summary>
        public int ViewCount { get; set; }
        
        /// <summary>
        /// 点赞数
        /// </summary>
        public int LikeCount { get; set; }
        
        /// <summary>
        /// 预计阅读时间（分钟）
        /// </summary>
        public int ReadingMinutes { get; set; }
        
        /// <summary>
        /// 是否允许评论
        /// </summary>
        public bool IsAllowComments { get; set; }
        
        /// <summary>
        /// SEO 标题
        /// </summary>
        public string? MetaTitle { get; set; }
        
        /// <summary>
        /// SEO 描述
        /// </summary>
        public string? MetaDescription { get; set; }
        
        /// <summary>
        /// SEO 关键词
        /// </summary>
        public string? MetaKeywords { get; set; }
    }
}