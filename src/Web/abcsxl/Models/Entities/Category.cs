using abcsxl.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace abcsxl.Models.Entities
{
    /// <summary>
    /// 分类
    /// </summary>
    public class Category
    {
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// 类别名称
        /// </summary>
        [Required]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 类别描述
        /// </summary>
        public string Description { get; set; } = string.Empty;
        /// <summary>
        /// URL友好名称
        /// </summary>
        public string Slug { get; set; } = string.Empty;
        /// <summary>
        /// 图标
        /// </summary>
        public string? Icon { get; set; }
        /// <summary>
        /// 封面图片
        /// </summary>
        public string? CoverImage { get; set; }
        /// <summary>
        /// 父分类Id
        /// </summary>
        public Guid? ParentId { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        [Required]
        public bool IsDeleted { get; set; } = false;

        // ========== 显示设置 ==========
        /// <summary>
        /// 排序
        /// </summary>
        public int Order { get; set; } = 0;
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsVisible { get; set; } = true;
        /// <summary>
        /// 是否在导航中显示
        /// </summary>
        public bool ShowInNav { get; set; } = true;
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

        /// <summary>
        /// 文章数
        /// </summary>
        public int PostCount { get; set; } = 0;

        // ========== 导航属性 ==========
        /// <summary>
        /// 父分类
        /// </summary>
        public Category? Parent { get; set; }
        /// <summary>
        /// 包含的子分类
        /// </summary>
        public ICollection<Category> Children { get; } = [];
        /// <summary>
        /// 包含的文章
        /// </summary>
        public ICollection<Post> Posts { get; } = [];

        public ICollection<PostCategory> PostCategories { get; } = [];

        // ========== 便捷属性 ==========

        [NotMapped]
        public bool HasChildren => Children?.Any(c => !c.IsDeleted) ?? false;

        [NotMapped]
        public bool HasPosts => PostCount > 0;

        [NotMapped]
        public ICollection<Post> PrimaryPosts => PostCategories
            .Where(pc => pc.IsPrimaryCategory && pc.Post != null && pc.Post.Status != PostStatus.Deleted && pc.Post.Status == PostStatus.Published)
            .OrderByDescending(pc => pc.Post!.PublishedAt ?? pc.Post!.CreatedAt)
            .Select(pc => pc.Post!)
            .ToList();

        /// <summary>
        /// 获取层级深度
        /// </summary>
        [NotMapped]
        public int Level
        {
            get
            {
                int level = 1;
                var current = Parent;
                while (current != null)
                {
                    level++;
                    current = current.Parent;
                }
                return level;
            }
        }

        /// <summary>
        /// 获取完整路径（如：技术/编程/C#）
        /// </summary>
        [NotMapped]
        public string FullPath
        {
            get
            {
                var path = new List<string> { Name };
                var current = Parent;
                while (current != null)
                {
                    path.Insert(0, current.Name);
                    current = current.Parent;
                }
                return string.Join(" / ", path);
            }
        }

        /// <summary>
        /// 获取完整路径（Slug格式）
        /// </summary>
        [NotMapped]
        public string FullSlugPath
        {
            get
            {
                var path = new List<string> { Slug };
                var current = Parent;
                while (current != null)
                {
                    path.Insert(0, current.Slug);
                    current = current.Parent;
                }
                return string.Join("/", path);
            }
        }

        /// <summary>
        /// 获取所有父级分类（从根到当前）
        /// </summary>
        [NotMapped]
        public List<Category> Breadcrumb
        {
            get
            {
                var breadcrumb = new List<Category>();
                var current = this;
                while (current != null)
                {
                    breadcrumb.Insert(0, current);
                    current = current.Parent;
                }
                return breadcrumb;
            }
        }
    }
}
