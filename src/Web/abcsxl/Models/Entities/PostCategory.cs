using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace abcsxl.Models.Entities
{
    public class PostCategory
    {
        public Guid PostId { get; set; }
        public Guid CategoryId { get; set; }

        // 业务字段
        [Display(Name = "是否主要分类")]
        public bool IsPrimaryCategory { get; set; } = false;

        [Display(Name = "排序")]
        public int Order { get; set; } = 0;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "更新时间")]
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "更新次数")]
        public int UpdateCount { get; set; } = 0;

        // ========== 导航属性 ==========
        /// <summary>
        /// 添加人Id
        /// </summary>
        public Guid? UserId { get; set; }
        /// <summary>
        /// 添加人
        /// </summary>
        [ForeignKey(nameof(UserId))]
        public User? AddedBy { get; set; }

        [ForeignKey(nameof(PostId))]
        public Post Post { get;  } = null!;

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; } = null!;

    }
}
