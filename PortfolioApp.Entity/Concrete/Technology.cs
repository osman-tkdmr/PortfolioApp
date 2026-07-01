using PortfolioApp.Core.Entities;

namespace PortfolioApp.Entity.Concrete;

public class Technology : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    public string? IconClass { get; set; }
    public string? Color { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual ICollection<ProjectTechnology> ProjectTechnologies { get; set; } = [];
}
