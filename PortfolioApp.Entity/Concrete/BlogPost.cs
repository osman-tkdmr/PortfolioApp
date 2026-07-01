using PortfolioApp.Core.Entities;

namespace PortfolioApp.Entity.Concrete;

public class BlogPost : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public int BlogCategoryId { get; set; }
    public string AuthorId { get; set; } = string.Empty;
    public DateTime? PublishedAt { get; set; }
    public bool IsPublished { get; set; }
    public bool IsFeatured { get; set; }
    public int ViewCount { get; set; }
    public int ReadTimeMinutes { get; set; } = 1;
    public bool AllowComments { get; set; } = true;

    public virtual BlogCategory BlogCategory { get; set; } = null!;
    public virtual ApplicationUser Author { get; set; } = null!;
    public virtual ICollection<BlogPostTag> BlogPostTags { get; set; } = [];
    public virtual ICollection<Comment> Comments { get; set; } = [];
}
