using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PortfolioApp.Business.Mappings;
using PortfolioApp.Business.Security;
using PortfolioApp.Business.Services.Concrete;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.Business.Validators;
using PortfolioApp.Core.Interfaces;
using PortfolioApp.DataAccess.UnitOfWork;

namespace PortfolioApp.Business.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusiness(this IServiceCollection services)
    {
        // AutoMapper — register all profiles from this assembly
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(BlogMappingProfile).Assembly));

        // FluentValidation
        services.AddValidatorsFromAssemblyContaining<BlogPostCreateValidator>();

        // UnitOfWork (scoped so services share the same DbContext per request)
        services.AddScoped<UnitOfWork>();

        // Memory Cache
        services.AddMemoryCache();

        // Current-user accessor (used by services for ownership scoping)
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Services
        services.AddScoped<IBlogService, BlogService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IThemeService, ThemeService>();
        services.AddScoped<ISiteSettingsService, SiteSettingsService>();
        services.AddScoped<IHeroService, HeroService>();
        services.AddScoped<IAboutService, AboutService>();
        services.AddScoped<IExperienceService, ExperienceService>();
        services.AddScoped<IEducationService, EducationService>();
        services.AddScoped<ICertificateService, CertificateService>();
        services.AddScoped<ISkillService, SkillService>();
        services.AddScoped<ILanguageService, LanguageService>();
        services.AddScoped<IAchievementService, AchievementService>();
        services.AddScoped<ITestimonialService, TestimonialService>();
        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<ISocialMediaService, SocialMediaService>();
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<IFooterService, FooterService>();
        services.AddScoped<ISeoService, SeoService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IVisitorLogService, VisitorLogService>();

        return services;
    }
}
