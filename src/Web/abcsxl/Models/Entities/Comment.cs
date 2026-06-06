using abcsxl.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace abcsxl.Models.Entities
{
    /// <summary>
    /// 评论
    /// </summary>
    public class Comment
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 评论的文章Id
        /// </summary>
        public Guid? PostId { get; set; }
        /// <summary>
        /// 评论用户Id
        /// </summary>
        [Required]
        public Guid UserId { get; set; }
        /// <summary>
        /// 回复的评论Id
        /// </summary>
        public Guid? ParentId { get; set; } // 回复的评论ID，实现嵌套评论

        /// <summary>
        /// 状态：待审核、已批准、已拒绝
        /// </summary>
        [Required]
        public CommentStatus Status { get; set; } = CommentStatus.Pending;
        /// <summary>
        /// 点赞数
        /// </summary>
        [Required]
        public int LikeCount { get; set; } = 0;
        /// <summary>
        /// 发布时间
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /////////////// 导航属性
        /// <summary>
        /// 评论的文章
        /// </summary>
        [ForeignKey(nameof(PostId))]
        public Post? Post { get; set; } = null!;
        /// <summary>
        /// 评论用户
        /// </summary>
        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;
        /// <summary>
        /// 回复的评论
        /// </summary>
        [ForeignKey(nameof(ParentId))]
        public Comment? Parent { get; set; }
        /// <summary>
        /// 本评论的所有回复
        /// </summary>
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();
    }
}
