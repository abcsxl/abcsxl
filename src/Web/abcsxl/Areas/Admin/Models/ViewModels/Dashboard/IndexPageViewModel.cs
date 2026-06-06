namespace abcsxl.Areas.Admin.Models.ViewModels.Dashboard
{
    public record IndexPageViewModel
    {
        public int TotalPosts { get; set; }
        public int PublishedPosts { get; set; }
        public int TotalComments { get; set; }
        public int TotalUsers { get; set; }
        public int TotalCategories { get; set; }
        public int TotalTags { get; set; }
        public ICollection<IndexPagePostViewModel> RecentPosts { get; set; } = [];
    }
}
