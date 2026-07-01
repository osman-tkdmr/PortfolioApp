using PortfolioApp.Core.Entities;

namespace PortfolioApp.Entity.Concrete;

public class Achievement : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconClass { get; set; }
    public string? ImageUrl { get; set; }
    public DateOnly? AchievedDate { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
