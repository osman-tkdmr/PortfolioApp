using PortfolioApp.DTO.DTOs.Blog;
using PortfolioApp.DTO.DTOs.Portfolio;
using PortfolioApp.DTO.DTOs.Project;
using PortfolioApp.DTO.DTOs.Site;

namespace PortfolioApp.DTO.ViewModels.Public;

public class HomeViewModel
{
    public HeroSectionDto? Hero { get; set; }
    public AboutDto? About { get; set; }
    public IList<SkillCategoryDto> SkillCategories { get; set; } = [];
    public IList<ExperienceDto> Experiences { get; set; } = [];
    public IList<EducationDto> Educations { get; set; } = [];
    public IList<ProjectDto> FeaturedProjects { get; set; } = [];
    public IList<BlogPostDto> RecentPosts { get; set; } = [];
    public IList<TestimonialDto> Testimonials { get; set; } = [];
    public IList<CertificateDto> Certificates { get; set; } = [];
    public IList<LanguageDto> Languages { get; set; } = [];
    public IList<AchievementDto> Achievements { get; set; } = [];
    public ContactInfoDto? ContactInfo { get; set; }
    public IList<SocialMediaDto> SocialMediaLinks { get; set; } = [];
    public SeoSettingsDto? Seo { get; set; }
    public SiteSettingsDto? SiteSettings { get; set; }
    public string ActiveThemeFolder { get; set; } = "Modern";
    public string ActiveThemeCss { get; set; } = "modern.css";
    public string ActiveThemeName { get; set; } = "Modern";
}
