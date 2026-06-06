using abcsxl.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace abcsxl.Models.Entities
{
    /// <summary>
    /// 用户
    /// </summary>
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;         // AK1：用户名唯一

        /// <summary>
        /// 登录密码
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// 手机号
        /// </summary>
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }         // AK2：手机号唯一

        /// <summary>
        /// 邮箱
        /// </summary>
        [EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }               // AK3：邮箱唯一

        /// <summary>
        /// 身份证号
        /// </summary>
        [MaxLength(18)]
        public string? NationalId { get; set; }          // AK4：身份证号唯一

        /// <summary>
        /// 显示名称
        /// </summary>
        [MaxLength(100)]
        public string? DisplayName { get; set; }

        /// <summary>
        /// 个人简介
        /// </summary>
        [MaxLength(500)]
        public string? Bio { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [MaxLength(200)]
        public string? Avatar { get; set; }

        /// <summary>
        /// 用户角色：普通用户、作者、管理员
        /// </summary>
        [Required]
        public UserRole Role { get; set; } = UserRole.User;

        /// <summary>
        /// 是否活跃
        /// </summary>
        [Required]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// 注册日期
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 最近登录时间
        /// </summary>
        public DateTime? LastLoginTime { get; set; }

        /////////////////// 导航属性
        /// <summary>
        /// 用户发表的文章
        /// </summary>
        public ICollection<Post> Posts { get; } = [];
        /// <summary>
        /// 用户发表的评论
        /// </summary>
        public ICollection<Comment> Comments { get; } = [];

        // ========== 便捷属性 ==========
        /// <summary>
        /// 文章数
        /// </summary>
        [NotMapped]
        public int PostCount => Posts?.Count(p => p.Status != PostStatus.Deleted && p.Status == PostStatus.Published) ?? 0;
        /// <summary>
        /// 评论数
        /// </summary>
        public int CommentCount => Comments?.Count ?? 0;
    }
}
