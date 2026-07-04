using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PortfolioApp.Core.Constants;
using PortfolioApp.Core.Interfaces;
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
        var provisioningService = serviceProvider.GetRequiredService<IUserProvisioningService>();
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(DataSeeder));

        await context.Database.MigrateAsync();

        await SeedRolesAsync(roleManager);
        var admin = await SeedAdminUserAsync(userManager, configuration, logger);
        await SeedThemesAsync(context);

        if (admin is not null)
            await provisioningService.ProvisionDefaultsAsync(admin.Id);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = [AppConstants.Roles.Admin, AppConstants.Roles.SuperAdmin, AppConstants.Roles.User];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    private static async Task<ApplicationUser?> SeedAdminUserAsync(UserManager<ApplicationUser> userManager, IConfiguration configuration, ILogger logger)
    {
        var adminEmail = configuration["AdminSettings:Email"];
        if (string.IsNullOrWhiteSpace(adminEmail))
            adminEmail = AppConstants.SeedData.AdminEmail;

        var existing = await userManager.FindByEmailAsync(adminEmail);
        if (existing is not null)
        {
            await EnsureRolesAsync(userManager, existing, AppConstants.Roles.Admin, AppConstants.Roles.SuperAdmin, AppConstants.Roles.User);
            return existing;
        }

        var firstName = configuration["AdminSettings:FirstName"];
        var lastName = configuration["AdminSettings:LastName"];

        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            Handle = "admin",
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
            await EnsureRolesAsync(userManager, admin, AppConstants.Roles.Admin, AppConstants.Roles.SuperAdmin, AppConstants.Roles.User);

            if (isGeneratedPassword)
            {
                logger.LogWarning(
                    "AdminSettings:Password yapılandırılmamış olduğu için '{Email}' hesabı için rastgele bir şifre üretildi: {Password} — lütfen ilk girişten sonra bu şifreyi değiştirin ve appsettings/user-secrets üzerinden kalıcı bir şifre tanımlayın.",
                    adminEmail, adminPassword);
            }

            return admin;
        }

        logger.LogError("Admin kullanıcısı oluşturulamadı: {Errors}", string.Join("; ", result.Errors.Select(e => e.Description)));
        return null;
    }

    private static async Task EnsureRolesAsync(UserManager<ApplicationUser> userManager, ApplicationUser user, params string[] roles)
    {
        foreach (var role in roles)
        {
            if (!await userManager.IsInRoleAsync(user, role))
                await userManager.AddToRoleAsync(user, role);
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
}
