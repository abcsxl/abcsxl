using System.ComponentModel.DataAnnotations;

namespace abcsxl.Models.ViewModels.Post
{
    public class IndexPageViewModel
    {
        public List<IndexPagePostViewModel> Posts { get; set; } = [];
        public CategorySidebarViewModel? SidebarCategories { get; set; } = null;
        public TagSidebarViewModel? SidebarTags { get; set; } = null;


        [Range(1, int.MaxValue, ErrorMessage = "'PageSize' 必须大于等于 1。")]
        public int PageSize { get; set; } = 5;
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "'CurrentPage' 必须大于等于 1。")]
        public int CurrentPage { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
    }
}
