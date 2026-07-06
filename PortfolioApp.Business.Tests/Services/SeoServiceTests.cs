using PortfolioApp.Business.Services.Concrete;
using PortfolioApp.Business.Tests.TestSupport;
using PortfolioApp.DTO.DTOs.Site;
using PortfolioApp.Entity.Concrete;
using Xunit;

namespace PortfolioApp.Business.Tests.Services;

public class SeoServiceTests : ServiceTestBase
{
    private SeoService CreateSut() => new(Uow, Mapper, CurrentUser);

    [Fact]
    public async Task GetByPageSlugAsync_ScopesToOwner()
    {
        await SeedAsync(
            new SeoSettings { UserId = TestTenants.TenantA, PageSlug = "home", MetaTitle = "A Home" },
            new SeoSettings { UserId = TestTenants.TenantB, PageSlug = "home", MetaTitle = "B Home" });

        var result = await CreateSut().GetByPageSlugAsync(TestTenants.TenantA, "home");

        Assert.True(result.Success);
        Assert.Equal("A Home", result.Data!.MetaTitle);
    }

    [Fact]
    public async Task GetAllAsync_ScopesToCurrentUser()
    {
        await SeedAsync(
            new SeoSettings { UserId = TestTenants.TenantA, PageSlug = "home", MetaTitle = "A Home" },
            new SeoSettings { UserId = TestTenants.TenantB, PageSlug = "home", MetaTitle = "B Home" });

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().GetAllAsync();

        Assert.True(result.Success);
        var item = Assert.Single(result.Data!);
        Assert.Equal("A Home", item.MetaTitle);
    }

    [Fact]
    public async Task UpdateAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var seo = new SeoSettings { UserId = TestTenants.TenantB, PageSlug = "home", MetaTitle = "B Home" };
        await SeedAsync(seo);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().UpdateAsync(new SeoSettingsUpdateDto { Id = seo.Id, PageSlug = "home", MetaTitle = "Hijacked" });

        Assert.False(result.Success);
        Assert.Equal("B Home", Context.SeoSettings.Single(s => s.Id == seo.Id).MetaTitle);
    }

    [Fact]
    public async Task UpdateAsync_OwnSeoSettings_UpdatesFields()
    {
        var seo = new SeoSettings { UserId = TestTenants.TenantA, PageSlug = "home", MetaTitle = "Old" };
        await SeedAsync(seo);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().UpdateAsync(new SeoSettingsUpdateDto { Id = seo.Id, PageSlug = "home", MetaTitle = "New" });

        Assert.True(result.Success);
        Assert.Equal("New", Context.SeoSettings.Single(s => s.Id == seo.Id).MetaTitle);
    }
}
