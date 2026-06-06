using System.ComponentModel.DataAnnotations;

namespace abcsxl.Models.Entities
{
    /// <summary>
    /// 标签
    /// </summary>
    public class Tag
    {
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; } = string.Empty;
        /// <summary>
        /// URL友好名称
        /// </summary>
        public string Slug { get; set; } = string.Empty;
        /// <summary>
        /// 颜色
        /// </summary>
        public string? Color { get; set; } = "#666666";
        /// <summary>
        /// 是否删除
        /// </summary>
        [Required]
        public bool IsDeleted { get; set; } = false;
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsVisible { get; set; } = true;
        /// <summary>
        /// 使用次数
        /// </summary>
        public int UsageCount { get; set; }
        /// <summary>
        /// 最近使用时间
        /// </summary>
        public DateTime? LastUsedAt { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        // ========== 导航属性 ==========
        // 多对多
        public ICollection<Post> Posts { get; } = [];

    }
}
