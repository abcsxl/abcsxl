using abcsxl.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace abcsxl.Areas.Admin.Models.ViewModels.Post
{
    public class PostCreateViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "标题")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "别名")]
        public string? Slug { get; set; }

        [Display(Name = "内容")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "摘要")]
        public string? Excerpt { get; set; }

        [Display(Name = "分类")]
        public Guid? CategoryId { get; set; }

        [Display(Name = "标签")]
        public string? Tags { get; set; }

        [Display(Name = "特色图片")]
        public string? FeaturedImage { get; set; }

        [Display(Name = "发布状态")]
        public PostStatus Status { get; set; } = PostStatus.Draft;

        [Display(Name = "允许评论")]
        public bool AllowComments { get; set; }

        [Display(Name = "置顶")]
        public bool IsPinned { get; set; }

        [Display(Name = "发布时间")]
        public DateTime? PublishAt { get; set; }

        public List<SelectListItem> Categories { get; set; } = [];
        public List<string> ExistingTags { get; set; } = [];

        public Guid? AuthorId { get; set; }
    }
}
