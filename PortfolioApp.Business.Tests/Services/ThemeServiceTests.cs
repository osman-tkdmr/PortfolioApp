using Microsoft.Extensions.Caching.Memory;
using PortfolioApp.Business.Services.Concrete;
using PortfolioApp.Business.Tests.TestSupport;
using PortfolioApp.Entity.Concrete;
using Xunit;

namespace PortfolioApp.Business.Tests.Services;

public class ThemeServiceTests : ServiceTestBase
{
    private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

    private ThemeService CreateSut() => new(Uow, Mapper, _cache, CurrentUser);

    [Fact]
    public async Task GetActiveThemeAsync_ResolvesViaCurrentUsersSiteSettings()
    {
        var theme = new Theme { Name = "Dark", FolderName = "dark", CssFileName = "dark.css", IsActive = true };
        await SeedAsync(theme);
        await SeedAsync(new SiteSettings { UserId = TestTenants.TenantA, SiteName = "A Site", ActiveThemeId = theme.Id });

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().GetActiveThemeAsync();

        Assert.True(result.Success);
        Assert.Equal("Dark", result.Data!.Name);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllThemesRegardlessOfCurrentUser()
    {
        await SeedAsync(
            new Theme { Name = "Light", FolderName = "light", CssFileName = "light.css" },
            new Theme { Name = "Dark", FolderName = "dark", CssFileName = "dark.css" });

        var result = await CreateSut().GetAllAsync();

        Assert.True(result.Success);
        Assert.Equal(2, result.Data!.Count);
    }

    [Fact]
    public async Task ActivateThemeAsync_UpdatesOnlyCurrentUsersSiteSettings()
    {
        var theme = new Theme { Name = "Dark", FolderName = "dark", CssFileName = "dark.css", IsActive = true };
        await SeedAsync(theme);
        await SeedAsync(
            new SiteSettings { UserId = TestTenants.TenantA, SiteName = "A Site" },
            new SiteSettings { UserId = TestTenants.TenantB, SiteName = "B Site" });

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().ActivateThemeAsync(theme.Id);

        Assert.True(result.Success);
        Assert.Equal(theme.Id, Context.SiteSettings.Single(s => s.UserId == TestTenants.TenantA).ActiveThemeId);
        Assert.Null(Context.SiteSettings.Single(s => s.UserId == TestTenants.TenantB).ActiveThemeId);
    }

    [Fact]
    public async Task ActivateThemeAsync_NoSiteSettingsForCurrentUser_ReturnsFailure()
    {
        var theme = new Theme { Name = "Dark", FolderName = "dark", CssFileName = "dark.css", IsActive = true };
        await SeedAsync(theme);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().ActivateThemeAsync(theme.Id);

        Assert.False(result.Success);
    }
}
