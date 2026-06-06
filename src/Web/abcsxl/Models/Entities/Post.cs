using abcsxl.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace abcsxl.Models.Entities
{
    /// <summary>
    /// 文章
    /// </summary>
    public class Post
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Required]
        public string Title { get; set; } = string.Empty;
        /// <summary>
        /// 副标题
        /// </summary>
        public string? Subtitle { get; set; }
        /// <summary>
        /// 内容（Markdown）
        /// </summary>
        [Required]
        public string Content { get; set; } = string.Empty; // 存储原始 Markdown
        /// <summary>
        /// URL友好名称
        /// </summary>
        public string Slug { get; set; } = string.Empty;
        /// <summary>
        /// 摘要
        /// </summary>
        public string? Excerpt { get; set; }
        /// <summary>
        /// 封面图片缩略图
        /// </summary>
        public string? CoverImageThumb { get; set; }
        /// <summary>
        /// 封面图片
        /// </summary>
        public string CoverImage { get; set; } = string.Empty;
        // SEO 字段
        /// <summary>
        /// Meta 标题
        /// </summary>
        public string? MetaTitle { get; set; }
        /// <summary>
        /// Meta 描述
        /// </summary>
        public string? MetaDescription { get; set; }
        /// <summary>
        /// Meta 关键词
        /// </summary>
        public string? MetaKeywords { get; set; }
        /// <summary>
        /// 查看数
        /// </summary>
        public int ViewCount { get; set; } = 0;
        /// <summary>
        /// 点赞数
        /// </summary>
        public int LikeCount { get; set; } = 0;
        /// <summary>
        /// 预计阅读时间
        /// </summary>
        public int ReadingMinutes { get; set; } = 0;
        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool IsFeatured { get; set; } = false;
       /// <summary>
        /// 是否允许评论
        /// </summary>
        public bool IsAllowComments { get; set; } = false;
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime? DeletedAt { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime? PublishedAt { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [Required]
        public PostStatus Status { get; set; } = PostStatus.Draft;

        // ========== 导航属性 ==========
        /// <summary>
        /// 文章作者Id
        /// </summary>
        [Required]
        public Guid AuthorId { get; set; }
        /// <summary>
        /// 文章作者
        /// </summary>
        [ForeignKey(nameof(AuthorId))]
        public User Author { get; set; } = null!;
        /// <summary>
        /// 文章的分类（多种）
        /// </summary>
        public ICollection<Category> Categories { get; } = [];
        /// <summary>
        /// 分类（多对多）
        /// </summary>
        public ICollection<PostCategory> PostCategories { get; } = [];
        /// <summary>
        /// 文章的标签（多个）
        /// </summary>
        public ICollection<Tag> Tags { get; } = [];
        /// <summary>
        /// 标签（多对多）
        /// </summary>
        public List<PostTag> PostTags { get; } = [];
        /// <summary>
        /// 文章的评论
        /// </summary>
        public ICollection<Comment> Comments { get; } = [];
        /// <summary>
        /// 访问记录
        /// </summary>
        public ICollection<VisitLog> VisitLogs {  get; } = [];

        // ========== 便捷属性（NotMapped）==========
        /// <summary>
        /// 主分类
        /// </summary>
        [NotMapped]
        public Category? PrimaryCategory => PostCategories
            .FirstOrDefault(pc => pc.IsPrimaryCategory)?.Category;
        /// <summary>
        /// 主分类名称
        /// </summary>
        [NotMapped]
        public string? PrimaryCategoryName => PrimaryCategory?.Name;
        /// <summary>
        /// 主分类URL友好名称
        /// </summary>
        [NotMapped]
        public string? PrimaryCategorySlug => PrimaryCategory?.Slug;
        /// <summary>
        /// 次分类
        /// </summary>
        [NotMapped]
        public ICollection<Category> SecondaryCategories => PostCategories
            .Where(pc => !pc.IsPrimaryCategory)
            .OrderBy(pc => pc.Order)
            .Select(pc => pc.Category)
            .ToList();
    }
}
