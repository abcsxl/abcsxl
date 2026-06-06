using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace abcsxl.Models.Entities
{
    /// <summary>
    /// 访问统计
    /// </summary>
    public class VisitLog
    {
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// 所属文章Id
        /// </summary>
        public Guid? PostId { get; set; }
        /// <summary>
        /// 访问时间
        /// </summary>
        [Required]
        public DateTime AccessTime { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// IP地址
        /// </summary>
        [MaxLength(50)]
        [Required]
        public string IPAddress { get; set; } = string.Empty;
        /// <summary>
        /// 用户代理
        /// </summary>
        [MaxLength(500)]
        public string? UserAgent { get; set; }
        /// <summary>
        /// 推荐人
        /// </summary>
        [MaxLength(200)]
        public string? Referrer { get; set; }
        /// <summary>
        /// 国家
        /// </summary>
        [MaxLength(50)]
        public string? Country { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        [MaxLength(50)]
        public string? City { get; set; }

        // 导航属性
        /// <summary>
        /// 所属文章
        /// </summary>
        [ForeignKey(nameof(PostId))]
        public Post? Post { get; set; } = null!;
    }
}
