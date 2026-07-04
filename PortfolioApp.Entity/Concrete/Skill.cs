using PortfolioApp.Core.Entities;
using PortfolioApp.Core.Enums;

namespace PortfolioApp.Entity.Concrete;

public class Skill : BaseEntity, IUserOwnedEntity
{
    public string UserId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public int SkillCategoryId { get; set; }
    public int Percentage { get; set; }
    public string? IconClass { get; set; }
    public SkillLevel Level { get; set; } = SkillLevel.Intermediate;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual SkillCategory SkillCategory { get; set; } = null!;
}
