using PortfolioApp.Core.Entities;

namespace PortfolioApp.Entity.Concrete;

public class Comment : BaseEntity, IUserOwnedEntity
{
    public string UserId { get; set; } = string.Empty;

    public int BlogPostId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorEmail { get; set; } = string.Empty;
    public string? AuthorWebsite { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public bool IsSpam { get; set; }
    public int? ParentCommentId { get; set; }
    public string? IpAddress { get; set; }

    public virtual BlogPost BlogPost { get; set; } = null!;
    public virtual Comment? ParentComment { get; set; }
    public virtual ICollection<Comment> Replies { get; set; } = [];
}
