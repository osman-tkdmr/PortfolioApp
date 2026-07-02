using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(DataSeeder));

        await context.Database.MigrateAsync();

        await SeedRolesAsync(roleManager);
        await SeedAdminUserAsync(userManager, configuration, logger);
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

    private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager, IConfiguration configuration, ILogger logger)
    {
        var adminEmail = configuration["AdminSettings:Email"];
        if (string.IsNullOrWhiteSpace(adminEmail))
            adminEmail = AppConstants.SeedData.AdminEmail;

        if (await userManager.FindByEmailAsync(adminEmail) is not null)
            return;

        var firstName = configuration["AdminSettings:FirstName"];
        var lastName = configuration["AdminSettings:LastName"];

        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = string.IsNullOrWhiteSpace(firstName) ? AppConstants.SeedData.AdminFirstName : firstName,
            LastName = string.IsNullOrWhiteSpace(lastName) ? AppConstants.SeedData.AdminLastName : lastName,
            Title = "Full-Stack Developer",
            EmailConfirmed = true,
            IsActive = true
        };

        var configuredPassword = configuration["AdminSettings:Password"];
        var isGeneratedPassword = string.IsNullOrWhiteSpace(configuredPassword);
        var adminPassword = isGeneratedPassword ? GenerateRandomPassword() : configuredPassword!;

        var result = await userManager.CreateAsync(admin, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, AppConstants.Roles.Admin);

            if (isGeneratedPassword)
            {
                logger.LogWarning(
                    "AdminSettings:Password yapılandırılmamış olduğu için '{Email}' hesabı için rastgele bir şifre üretildi: {Password} — lütfen ilk girişten sonra bu şifreyi değiştirin ve appsettings/user-secrets üzerinden kalıcı bir şifre tanımlayın.",
                    adminEmail, adminPassword);
            }
        }
        else
        {
            logger.LogError("Admin kullanıcısı oluşturulamadı: {Errors}", string.Join("; ", result.Errors.Select(e => e.Description)));
        }
    }

    private static string GenerateRandomPassword()
    {
        const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        const string lower = "abcdefghijkmnopqrstuvwxyz";
        const string digits = "23456789";
        const string special = "!@#$%^&*";
        const string all = upper + lower + digits + special;

        Span<char> chars = stackalloc char[16];
        chars[0] = upper[RandomNumberGenerator.GetInt32(upper.Length)];
        chars[1] = lower[RandomNumberGenerator.GetInt32(lower.Length)];
        chars[2] = digits[RandomNumberGenerator.GetInt32(digits.Length)];
        chars[3] = special[RandomNumberGenerator.GetInt32(special.Length)];
        for (var i = 4; i < chars.Length; i++)
            chars[i] = all[RandomNumberGenerator.GetInt32(all.Length)];

        // Shuffle so the guaranteed-category characters aren't always in the same position
        for (var i = chars.Length - 1; i > 0; i--)
        {
            var j = RandomNumberGenerator.GetInt32(i + 1);
            (chars[i], chars[j]) = (chars[j], chars[i]);
        }

        return new string(chars);
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
