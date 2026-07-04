using PortfolioApp.Core.Entities;

namespace PortfolioApp.Entity.Concrete;

public class Experience : BaseEntity, IUserOwnedEntity
{
    public string UserId { get; set; } = string.Empty;

    public string JobTitle { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? CompanyLogoUrl { get; set; }
    public string? Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
