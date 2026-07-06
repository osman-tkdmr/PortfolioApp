using Microsoft.Extensions.Caching.Memory;
using PortfolioApp.Business.Services.Concrete;
using PortfolioApp.Business.Tests.TestSupport;
using PortfolioApp.DTO.DTOs.Site;
using PortfolioApp.Entity.Concrete;
using Xunit;

namespace PortfolioApp.Business.Tests.Services;

public class SiteSettingsServiceTests : ServiceTestBase
{
    private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

    private SiteSettingsService CreateSut() => new(Uow, Mapper, _cache, CurrentUser);

    [Fact]
    public async Task GetAsync_ScopesToOwner()
    {
        await SeedAsync(
            new SiteSettings { UserId = TestTenants.TenantA, SiteName = "A Site" },
            new SiteSettings { UserId = TestTenants.TenantB, SiteName = "B Site" });

        var result = await CreateSut().GetAsync(TestTenants.TenantA);

        Assert.True(result.Success);
        Assert.Equal("A Site", result.Data!.SiteName);
    }

    [Fact]
    public async Task UpdateAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var settings = new SiteSettings { UserId = TestTenants.TenantB, SiteName = "B Site" };
        await SeedAsync(settings);

        CurrentUser.UserId = TestTenants.TenantA;
        var dto = new SiteSettingsUpdateDto { Id = settings.Id, SiteName = "Hijacked" };
        var result = await CreateSut().UpdateAsync(dto);

        Assert.False(result.Success);
        Assert.Equal("B Site", Context.SiteSettings.Single(s => s.Id == settings.Id).SiteName);
    }

    [Fact]
    public async Task UpdateAsync_OwnSettings_UpdatesFields()
    {
        var settings = new SiteSettings { UserId = TestTenants.TenantA, SiteName = "Old" };
        await SeedAsync(settings);

        CurrentUser.UserId = TestTenants.TenantA;
        var dto = new SiteSettingsUpdateDto { Id = settings.Id, SiteName = "New" };
        var result = await CreateSut().UpdateAsync(dto);

        Assert.True(result.Success);
        Assert.Equal("New", Context.SiteSettings.Single(s => s.Id == settings.Id).SiteName);
    }
}
