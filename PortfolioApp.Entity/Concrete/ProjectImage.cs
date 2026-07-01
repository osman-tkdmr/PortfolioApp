using PortfolioApp.Core.Entities;

namespace PortfolioApp.Entity.Concrete;

public class ProjectImage : BaseEntity
{
    public int ProjectId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public bool IsCover { get; set; }
    public int DisplayOrder { get; set; }

    public virtual Project Project { get; set; } = null!;
}
