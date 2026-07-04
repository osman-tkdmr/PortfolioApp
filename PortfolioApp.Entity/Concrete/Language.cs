using PortfolioApp.Core.Entities;
using PortfolioApp.Core.Enums;

namespace PortfolioApp.Entity.Concrete;

public class Language : BaseEntity, IUserOwnedEntity
{
    public string UserId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public string? FlagEmoji { get; set; }
    public LanguageLevel Level { get; set; } = LanguageLevel.Intermediate;
    public int Percentage { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
