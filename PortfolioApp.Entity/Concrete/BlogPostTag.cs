namespace PortfolioApp.Entity.Concrete;

public class BlogPostTag
{
    public int BlogPostId { get; set; }
    public int BlogTagId { get; set; }

    public virtual BlogPost BlogPost { get; set; } = null!;
    public virtual BlogTag BlogTag { get; set; } = null!;
}
