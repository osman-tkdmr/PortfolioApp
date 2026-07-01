using PortfolioApp.Core.Entities;

namespace PortfolioApp.Entity.Concrete;

public class BlogTag : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual ICollection<BlogPostTag> BlogPostTags { get; set; } = [];
}
