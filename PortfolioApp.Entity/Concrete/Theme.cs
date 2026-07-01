using PortfolioApp.Core.Entities;

namespace PortfolioApp.Entity.Concrete;

public class Theme : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string FolderName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? PreviewImageUrl { get; set; }
    public string CssFileName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
}
