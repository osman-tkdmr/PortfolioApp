using PortfolioApp.Core.Entities;
using PortfolioApp.Core.Enums;

namespace PortfolioApp.Entity.Concrete;

public class FooterContent : BaseEntity, IUserOwnedEntity
{
    public string UserId { get; set; } = string.Empty;

    public string SectionTitle { get; set; } = string.Empty;
    public string? Content { get; set; }
    public FooterSectionType SectionType { get; set; } = FooterSectionType.Text;
    public int ColumnPosition { get; set; } = 1;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
