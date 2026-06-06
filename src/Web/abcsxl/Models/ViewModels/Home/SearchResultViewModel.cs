namespace abcsxl.Models.ViewModels.Home;

public class SearchResultViewModel
{
    public string Query { get; set; } = string.Empty;
    public int TotalCount { get; set; }
    public List<PostResultItem> Posts { get; set; } = [];
    public List<CategoryResultItem> Categories { get; set; } = [];
    public List<TagResultItem> Tags { get; set; } = [];
    public string ActiveTab { get; set; } = "all";
}

public class PostResultItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public string Url { get; set; } = string.Empty;
}

public class CategoryResultItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int PostCount { get; set; }
    public string Url { get; set; } = string.Empty;
}

public class TagResultItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int PostCount { get; set; }
    public string Url { get; set; } = string.Empty;
}