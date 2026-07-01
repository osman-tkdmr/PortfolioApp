using PortfolioApp.Core.Entities;

namespace PortfolioApp.Entity.Concrete;

public class BlogCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual ICollection<BlogPost> BlogPosts { get; set; } = [];
}
