using PortfolioApp.Core.Entities;

namespace PortfolioApp.Entity.Concrete;

public class HeroSection : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? Description { get; set; }
    public string? BackgroundImageUrl { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? CtaButtonText { get; set; }
    public string? CtaButtonUrl { get; set; }
    public string? SecondaryButtonText { get; set; }
    public string? SecondaryButtonUrl { get; set; }
    public string? TypewriterTexts { get; set; }
    public bool ShowTypewriter { get; set; } = true;
    public bool IsActive { get; set; } = true;
}
