using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PortfolioApp.Core.Constants;
using PortfolioApp.DataAccess.Context;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.DataAccess.SeedData;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<PortfolioDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.MigrateAsync();

        await SeedRolesAsync(roleManager);
        await SeedAdminUserAsync(userManager);
        await SeedThemesAsync(context);
        await SeedSiteSettingsAsync(context);
        await SeedSeoSettingsAsync(context);
        await SeedContactInfoAsync(context);
        await SeedHeroSectionAsync(context);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = [AppConstants.Roles.Admin, AppConstants.Roles.SuperAdmin];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
    {
        if (await userManager.FindByEmailAsync(AppConstants.SeedData.AdminEmail) is not null)
            return;

        var admin = new ApplicationUser
        {
            UserName = AppConstants.SeedData.AdminEmail,
            Email = AppConstants.SeedData.AdminEmail,
            FirstName = AppConstants.SeedData.AdminFirstName,
            LastName = AppConstants.SeedData.AdminLastName,
            Title = "Full-Stack Developer",
            EmailConfirmed = true,
            IsActive = true
        };

        var result = await userManager.CreateAsync(admin, AppConstants.SeedData.AdminPassword);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(admin, AppConstants.Roles.Admin);
    }

    private static async Task SeedThemesAsync(PortfolioDbContext context)
    {
        if (await context.Themes.AnyAsync())
            return;

        var themes = new List<Theme>
        {
            new() { Name = "Modern", FolderName = "Modern", CssFileName = "modern.css", Description = "Clean beige & gold premium design", IsActive = true, DisplayOrder = 1 },
            new() { Name = "Premium", FolderName = "Premium", CssFileName = "premium.css", Description = "Luxury dark with gold accents", IsActive = false, DisplayOrder = 2 },
            new() { Name = "Minimal", FolderName = "Minimal", CssFileName = "minimal.css", Description = "Ultra-clean minimalist layout", IsActive = false, DisplayOrder = 3 },
            new() { Name = "Creative", FolderName = "Creative", CssFileName = "creative.css", Description = "Bold and vibrant creative design", IsActive = false, DisplayOrder = 4 },
            new() { Name = "Corporate", FolderName = "Corporate", CssFileName = "corporate.css", Description = "Professional corporate look", IsActive = false, DisplayOrder = 5 },
            new() { Name = "Dark", FolderName = "Dark", CssFileName = "dark.css", Description = "Sleek dark mode portfolio", IsActive = false, DisplayOrder = 6 },
            new() { Name = "Elegant", FolderName = "Elegant", CssFileName = "elegant.css", Description = "Classic elegant typography focus", IsActive = false, DisplayOrder = 7 },
        };

        await context.Themes.AddRangeAsync(themes);
        await context.SaveChangesAsync();
    }

    private static async Task SeedSiteSettingsAsync(PortfolioDbContext context)
    {
        if (await context.SiteSettings.AnyAsync())
            return;

        var activeTheme = await context.Themes.FirstOrDefaultAsync(t => t.IsActive);

        var settings = new SiteSettings
        {
            SiteName = "My Portfolio",
            SiteTitle = "Full-Stack Developer Portfolio",
            SiteDescription = "Professional portfolio showcasing projects, skills and experience.",
            CopyrightText = $"© {DateTime.Now.Year} My Portfolio. All rights reserved.",
            Language = "tr",
            ActiveThemeId = activeTheme?.Id
        };

        await context.SiteSettings.AddAsync(settings);
        await context.SaveChangesAsync();
    }

    private static async Task SeedSeoSettingsAsync(PortfolioDbContext context)
    {
        if (await context.SeoSettings.AnyAsync())
            return;

        var pages = new List<SeoSettings>
        {
            new() { PageSlug = AppConstants.PageSlugs.Home, MetaTitle = "Home | My Portfolio", MetaDescription = "Welcome to my professional portfolio." },
            new() { PageSlug = AppConstants.PageSlugs.Blog, MetaTitle = "Blog | My Portfolio", MetaDescription = "Articles about software development and technology." },
            new() { PageSlug = AppConstants.PageSlugs.Projects, MetaTitle = "Projects | My Portfolio", MetaDescription = "Browse my portfolio of projects." },
            new() { PageSlug = AppConstants.PageSlugs.Contact, MetaTitle = "Contact | My Portfolio", MetaDescription = "Get in touch with me." },
            new() { PageSlug = AppConstants.PageSlugs.About, MetaTitle = "About | My Portfolio", MetaDescription = "Learn more about me." },
        };

        await context.SeoSettings.AddRangeAsync(pages);
        await context.SaveChangesAsync();
    }

    private static async Task SeedContactInfoAsync(PortfolioDbContext context)
    {
        if (await context.ContactInfos.AnyAsync())
            return;

        await context.ContactInfos.AddAsync(new ContactInfo
        {
            Email = "hello@portfolio.com",
            WorkingHours = "Mon-Fri: 09:00 - 18:00",
            IsActive = true
        });

        await context.SaveChangesAsync();
    }

    private static async Task SeedHeroSectionAsync(PortfolioDbContext context)
    {
        if (await context.HeroSections.AnyAsync())
            return;

        await context.HeroSections.AddAsync(new HeroSection
        {
            Title = "Hi, I'm Your Name",
            Subtitle = "Full-Stack Developer",
            Description = "I build modern, scalable web applications with passion and precision.",
            TypewriterTexts = "[\"Full-Stack Developer\",\"Software Engineer\",\"UI/UX Enthusiast\"]",
            ShowTypewriter = true,
            CtaButtonText = "View My Work",
            CtaButtonUrl = "#portfolio",
            SecondaryButtonText = "Download CV",
            SecondaryButtonUrl = "#",
            IsActive = true
        });

        await context.SaveChangesAsync();
    }
}
