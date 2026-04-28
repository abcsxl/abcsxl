using abcsxl.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace abcsxl.Areas.Admin.Models.ViewModels.Post
{
    public class PostCreateViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "请输入文章标题")]
        [Display(Name = "标题")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "标题长度2-200个字符")]
        public string Title { get; set; }

        [Display(Name = "别名")]
        //[RegularExpression(@"^[a-z0-9\-]+$", ErrorMessage = "别名只能包含小写字母、数字和连字符")]
        public string Slug { get; set; }

        [Required(ErrorMessage = "请输入文章内容")]
        [Display(Name = "内容")]
        public string Content { get; set; }

        [Display(Name = "摘要")]
        [StringLength(500, ErrorMessage = "摘要最多500个字符")]
        public string Excerpt { get; set; }

        [Display(Name = "分类")]
        public Guid? CategoryId { get; set; }

        [Display(Name = "标签")]
        public string Tags { get; set; }  // 逗号分隔

        [Display(Name = "特色图片")]
        public string FeaturedImage { get; set; }

        [Display(Name = "发布状态")]
        public PostStatus Status { get; set; } = PostStatus.Published;

        [Display(Name = "允许评论")]
        public bool AllowComments { get; set; } = false;

        [Display(Name = "置顶")]
        public bool IsPinned { get; set; }

        [Display(Name = "发布时间")]
        public DateTime? PublishAt { get; set; }

        // 下拉框数据
        public List<SelectListItem> Categories { get; set; }
        public List<string> ExistingTags { get; set; }
    }
}
