namespace PortfolioApp.DTO.DTOs.Blog;

public class BlogPostDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int BlogCategoryId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public string? AuthorImageUrl { get; set; }
    public DateTime? PublishedAt { get; set; }
    public bool IsPublished { get; set; }
    public bool IsFeatured { get; set; }
    public int ViewCount { get; set; }
    public int ReadTimeMinutes { get; set; }
    public bool AllowComments { get; set; }
    public IList<string> TagNames { get; set; } = [];
    public IList<BlogTagDto> Tags { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}

public class BlogPostCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public int BlogCategoryId { get; set; }
    public bool IsPublished { get; set; }
    public bool IsFeatured { get; set; }
    public bool AllowComments { get; set; } = true;
    public IList<int> TagIds { get; set; } = [];
}

public class BlogPostUpdateDto : BlogPostCreateDto
{
    public int Id { get; set; }
}
