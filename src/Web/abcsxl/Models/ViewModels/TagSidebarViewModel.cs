namespace abcsxl.Models.ViewModels
{
    public class TagSidebarViewModel
    {
        public List<TagViewModel> Tags { get; set; } = [];

        public string? CurrentTag { get; set; } = null;
    }

    public class TagViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = String.Empty;
    }
}
