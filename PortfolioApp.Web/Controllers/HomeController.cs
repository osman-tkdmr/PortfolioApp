using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.DTO.ViewModels.Public;
using PortfolioApp.Web;

namespace PortfolioApp.Web.Controllers;

public class HomeController : Controller
{
    private readonly IHeroService _heroService;
    private readonly IAboutService _aboutService;
    private readonly ISkillService _skillService;
    private readonly IExperienceService _experienceService;
    private readonly IEducationService _educationService;
    private readonly IProjectService _projectService;
    private readonly IBlogService _blogService;
    private readonly ITestimonialService _testimonialService;
    private readonly ICertificateService _certificateService;
    private readonly ILanguageService _languageService;
    private readonly IAchievementService _achievementService;
    private readonly IContactService _contactService;
    private readonly ISocialMediaService _socialMediaService;
    private readonly ISeoService _seoService;
    private readonly ISiteSettingsService _siteSettingsService;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public HomeController(
        IHeroService heroService, IAboutService aboutService,
        ISkillService skillService, IExperienceService experienceService,
        IEducationService educationService, IProjectService projectService,
        IBlogService blogService, ITestimonialService testimonialService,
        ICertificateService certificateService, ILanguageService languageService,
        IAchievementService achievementService, IContactService contactService,
        ISocialMediaService socialMediaService, ISeoService seoService,
        ISiteSettingsService siteSettingsService, IStringLocalizer<SharedResource> localizer)
    {
        _heroService = heroService;
        _aboutService = aboutService;
        _skillService = skillService;
        _experienceService = experienceService;
        _educationService = educationService;
        _projectService = projectService;
        _blogService = blogService;
        _testimonialService = testimonialService;
        _certificateService = certificateService;
        _languageService = languageService;
        _achievementService = achievementService;
        _contactService = contactService;
        _socialMediaService = socialMediaService;
        _seoService = seoService;
        _siteSettingsService = siteSettingsService;
        _localizer = localizer;
    }

    public async Task<IActionResult> Index()
    {
        var hero = await _heroService.GetActiveAsync();
        var about = await _aboutService.GetActiveAsync();
        var skills = await _skillService.GetCategoriesWithSkillsAsync();
        var experiences = await _experienceService.GetAllActiveAsync();
        var educations = await _educationService.GetAllActiveAsync();
        var projects = await _projectService.GetFeaturedAsync(6);
        var blog = await _blogService.GetRecentAsync(3);
        var testimonials = await _testimonialService.GetApprovedAsync();
        var certificates = await _certificateService.GetAllActiveAsync();
        var languages = await _languageService.GetAllActiveAsync();
        var achievements = await _achievementService.GetAllActiveAsync();
        var contact = await _contactService.GetContactInfoAsync();
        var social = await _socialMediaService.GetAllActiveAsync();
        var seo = await _seoService.GetByPageSlugAsync("home");
        var settings = await _siteSettingsService.GetAsync();

        var themeFolder = HttpContext.Items["CurrentThemeFolder"]?.ToString() ?? "Modern";
        var themeCss = HttpContext.Items["CurrentThemeCss"]?.ToString() ?? "modern.css";
        var themeName = HttpContext.Items["CurrentThemeName"]?.ToString() ?? "Modern";

        var vm = new HomeViewModel
        {
            Hero = hero.Data,
            About = about.Data,
            SkillCategories = skills.Data ?? [],
            Experiences = experiences.Data ?? [],
            Educations = educations.Data ?? [],
            FeaturedProjects = projects.Data ?? [],
            RecentPosts = blog.Data ?? [],
            Testimonials = testimonials.Data ?? [],
            Certificates = certificates.Data ?? [],
            Languages = languages.Data ?? [],
            Achievements = achievements.Data ?? [],
            ContactInfo = contact.Data,
            SocialMediaLinks = social.Data ?? [],
            Seo = seo.Data,
            SiteSettings = settings.Data,
            ActiveThemeFolder = themeFolder,
            ActiveThemeCss = themeCss,
            ActiveThemeName = themeName
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Contact(string senderName, string senderEmail, string subject, string message)
    {
        if (string.IsNullOrWhiteSpace(senderName) || string.IsNullOrWhiteSpace(senderEmail) ||
            string.IsNullOrWhiteSpace(message))
            return Json(new { success = false, message = _localizer["ContactFillAllFields"].Value });

        var dto = new PortfolioApp.DTO.DTOs.Site.ContactMessageCreateDto
        {
            SenderName = senderName,
            SenderEmail = senderEmail,
            Subject = subject,
            Message = message
        };

        var result = await _contactService.SendMessageAsync(dto);
        return Json(new { success = result.Success, message = result.Message });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}
