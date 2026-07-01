using PortfolioApp.DTO.DTOs.Site;

namespace PortfolioApp.DTO.ViewModels.Public;

public class ContactViewModel
{
    public ContactInfoDto? ContactInfo { get; set; }
    public IList<SocialMediaDto> SocialMediaLinks { get; set; } = [];
    public ContactMessageCreateDto Form { get; set; } = new();
    public SeoSettingsDto? Seo { get; set; }
    public string ActiveThemeFolder { get; set; } = "Modern";
    public string ActiveThemeCss { get; set; } = "modern.css";
}
