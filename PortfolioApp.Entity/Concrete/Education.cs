using PortfolioApp.Core.Entities;

namespace PortfolioApp.Entity.Concrete;

public class Education : BaseEntity, IUserOwnedEntity
{
    public string UserId { get; set; } = string.Empty;

    public string Degree { get; set; } = string.Empty;
    public string School { get; set; } = string.Empty;
    public string? FieldOfStudy { get; set; }
    public string? Location { get; set; }
    public string? Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public decimal? GPA { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
