using PortfolioApp.Core.Entities;

namespace PortfolioApp.Entity.Concrete;

public class SkillCategory : BaseEntity, IUserOwnedEntity
{
    public string UserId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public string? IconClass { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual ICollection<Skill> Skills { get; set; } = [];
}
