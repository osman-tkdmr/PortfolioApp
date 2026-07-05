using PortfolioApp.Core.Entities;

namespace PortfolioApp.Entity.Concrete;

public class Project : BaseEntity, IUserOwnedEntity
{
    public string UserId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? DemoUrl { get; set; }
    public string? SourceUrl { get; set; }
    public int ProjectCategoryId { get; set; }
    public bool IsFeatured { get; set; }
    public DateOnly? CompletedDate { get; set; }
    public string? ClientName { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual ProjectCategory ProjectCategory { get; set; } = null!;
    public virtual ICollection<ProjectImage> Images { get; set; } = [];
    public virtual ICollection<ProjectTechnology> ProjectTechnologies { get; set; } = [];
}
