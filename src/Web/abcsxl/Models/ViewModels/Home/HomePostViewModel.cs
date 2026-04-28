using System.ComponentModel;

namespace abcsxl.Models.ViewModels.Home
{
    public record HomePostViewModel
    {
        public Guid Id { get; set; }
        [DisplayName("Title")]
        public string Title { get; set; } = string.Empty;
        [DisplayName("Excerpt")]
        public string Excerpt { get; set; } = string.Empty;
        [DisplayName("PublishedAt")]
        public DateTime? PublishedAt { get; set; }
        [DisplayName("Author")]
        public string Author { get; set; } = string.Empty;
    }
}
