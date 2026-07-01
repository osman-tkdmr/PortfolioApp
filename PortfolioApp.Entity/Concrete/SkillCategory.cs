using PortfolioApp.Core.Entities;

namespace PortfolioApp.Entity.Concrete;

public class SkillCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? IconClass { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual ICollection<Skill> Skills { get; set; } = [];
}
