using PortfolioApp.Core.Entities;

namespace PortfolioApp.Entity.Concrete;

public class SeoSettings : BaseEntity
{
    public string PageSlug { get; set; } = string.Empty;
    public string MetaTitle { get; set; } = string.Empty;
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public string? OgTitle { get; set; }
    public string? OgDescription { get; set; }
    public string? OgImageUrl { get; set; }
    public string? CanonicalUrl { get; set; }
    public bool NoIndex { get; set; }
    public bool NoFollow { get; set; }
}
