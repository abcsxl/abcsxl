namespace abcsxl.Areas.Admin.Models.ViewModels.Dashboard
{
    public record IndexPagePostViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime? PublishedAt { get; set; }
        public bool IsPublished { get; set; }
        public string AuthorName { get; set; } = string.Empty;
    }
}
