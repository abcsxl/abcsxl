using System.ComponentModel;

namespace abcsxl.Models.ViewModels.Post
{
    public record IndexPagePostViewModel
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [DisplayName("标题")]
        public string Title { get; set; } = string.Empty;
        /// <summary>
        /// 摘要
        /// </summary>
        [DisplayName("摘要")]
        public string Excerpt { get; set; } = string.Empty;
        /// <summary>
        /// 所有分类
        /// </summary>
        [DisplayName("所属分类")]
        public List<PostListCategoryViewModel> Categories { get; set; } = [];
        /// <summary>
        /// 所有标签
        /// </summary>        
        [DisplayName("所有标签")]
        public List<PostListTagViewModel> Tags { get; set; }= [];
        /// <summary>
        /// 作者
        /// </summary>
        [DisplayName("作者")]
        public PostListUserViewModel Author { get; set; } = null!;
        /// <summary>
        /// 发布时间
        /// </summary>
        [DisplayName("发布时间")]
        public DateTime PublishedTime { get; set; }
    }

    public record PostListCategoryViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = String.Empty; 
    }

    public record PostListTagViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = String.Empty;
    }

    public record PostListUserViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = String.Empty;
    }
}
