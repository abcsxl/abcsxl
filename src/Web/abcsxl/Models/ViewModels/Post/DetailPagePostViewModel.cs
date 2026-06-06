using abcsxl.Models.Entities;
using System.ComponentModel;

namespace abcsxl.Models.ViewModels.Post
{
    public record DetailPagePostViewModel
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [DisplayName("标题")]
        public string Title { get; set; } = string.Empty;
        /// <summary>
        /// 副标题
        /// </summary>
        [DisplayName("副标题")] 
        public string? Subtitle { get; set; }
        /// <summary>
        /// 内容（Markdown）
        /// </summary>
        [DisplayName("内容")] 
        public string Content { get; set; } = string.Empty; // 存储原始 Markdown
        /// <summary>
        /// URL友好名称
        /// </summary>
        [DisplayName("URL友好名称")] 
        public string Slug { get; set; } = string.Empty;
        /// <summary>
        /// 摘要
        /// </summary>
        [DisplayName("摘要")] 
        public string? Excerpt { get; set; }
        /// <summary>
        /// 封面图片缩略图
        /// </summary>
        [DisplayName("封面图片缩略图")] 
        public string? CoverImageThumb { get; set; }
        /// <summary>
        /// 封面图片
        /// </summary>
        [DisplayName("封面图片")] 
        public string CoverImage { get; set; } = string.Empty;
        // SEO 字段
        /// <summary>
        /// Meta 标题
        /// </summary>
        [DisplayName("Meta 标题")] public string? MetaTitle { get; set; }
        /// <summary>
        /// Meta 描述
        /// </summary>
        [DisplayName("Meta 描述")] public string? MetaDescription { get; set; }
        /// <summary>
        /// Meta 关键词
        /// </summary>
        [DisplayName("Meta 关键词")] public string? MetaKeywords { get; set; }
        /// <summary>
        /// 查看数
        /// </summary>
        [DisplayName("查看数")] public int ViewCount { get; set; } = 0;
        /// <summary>
        /// 点赞数
        /// </summary>
        [DisplayName("点赞数")] public int LikeCount { get; set; } = 0;
        /// <summary>
        /// 预计阅读时间
        /// </summary>
        [DisplayName("预计阅读时间")] public int ReadingMinutes { get; set; } = 0;
        /// <summary>
        /// 是否允许评论
        /// </summary>
        [DisplayName("是否允许评论")] public bool IsAllowComments { get; set; } = false;
        /// <summary>
        /// 创建时间
        /// </summary>
        [DisplayName("创建时间")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// 更新时间
        /// </summary>
        [DisplayName("更新时间")] public DateTime? UpdatedAt { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        [DisplayName("发布时间")] public DateTime? PublishedAt { get; set; }
        /// <summary>
        /// 文章作者
        /// </summary>
        [DisplayName("文章作者")] public PostDetailUserViewModel Author { get; set; } = null!;
        /// <summary>
        /// 文章的分类（多种）
        /// </summary>
        [DisplayName("文章的分类")] public ICollection<PostDetailCategoryViewModel> Categories { get; set; } = [];
        /// <summary>
        /// 文章的标签（多个）
        /// </summary>
        [DisplayName("文章的标签")] public ICollection<PostDetailTagViewModel> Tags { get; set; } = [];
        ///// <summary>
        ///// 文章的评论
        ///// </summary>
        //[DisplayName("文章的评论")] public ICollection<Comment> Comments { get;  set; } = [];
        ///// <summary>
        ///// 访问记录
        ///// </summary>
        //[DisplayName("访问记录")] public ICollection<VisitLog> VisitLogs { get;  set;} = [];
    }

    public record PostDetailCategoryViewModel
    {
        public Guid Id { get; set; }
        [DisplayName("名称")] 
        public string Title { get; set; } = String.Empty;
    }

    public record PostDetailTagViewModel
    {
        public Guid Id { get; set; }
        [DisplayName("名称")] 
        public string Title { get; set; } = String.Empty;
    }

    public record PostDetailUserViewModel
    {
        public Guid Id { get; set; }
        [DisplayName("名称")] 
        public string Name { get; set; } = String.Empty;
    }
}
