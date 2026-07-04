using PortfolioApp.Core.Entities;

namespace PortfolioApp.Entity.Concrete;

public class SiteSettings : BaseEntity, IUserOwnedEntity
{
    public string UserId { get; set; } = string.Empty;

    public string SiteName { get; set; } = "My Portfolio";
    public string? SiteTitle { get; set; }
    public string? SiteDescription { get; set; }
    public string? LogoUrl { get; set; }
    public string? FaviconUrl { get; set; }
    public string? FooterText { get; set; }
    public string? CopyrightText { get; set; }
    public string? GoogleAnalyticsId { get; set; }
    public string Language { get; set; } = "tr";
    public bool IsMaintenanceMode { get; set; }
    public string? MaintenanceMessage { get; set; }
    public int? ActiveThemeId { get; set; }

    public virtual Theme? ActiveTheme { get; set; }
}
