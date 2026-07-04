using PortfolioApp.Core.Entities;

namespace PortfolioApp.Entity.Concrete;

public class ProjectCategory : BaseEntity, IUserOwnedEntity
{
    public string UserId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconClass { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual ICollection<Project> Projects { get; set; } = [];
}
