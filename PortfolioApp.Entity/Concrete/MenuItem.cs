using PortfolioApp.Core.Entities;
using PortfolioApp.Core.Enums;

namespace PortfolioApp.Entity.Concrete;

public class MenuItem : BaseEntity, IUserOwnedEntity
{
    public string UserId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? IconClass { get; set; }
    public int? ParentMenuItemId { get; set; }
    public MenuLocation Location { get; set; } = MenuLocation.Header;
    public string? Target { get; set; }
    public string? CssClass { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual MenuItem? ParentMenuItem { get; set; }
    public virtual ICollection<MenuItem> Children { get; set; } = [];
}
