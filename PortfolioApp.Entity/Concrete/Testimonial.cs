using PortfolioApp.Core.Entities;

namespace PortfolioApp.Entity.Concrete;

public class Testimonial : BaseEntity, IUserOwnedEntity
{
    public string UserId { get; set; } = string.Empty;

    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorTitle { get; set; }
    public string? AuthorCompany { get; set; }
    public string? AuthorImageUrl { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; } = 5;
    public bool IsApproved { get; set; } = true;
    public bool IsFeatured { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
