using PortfolioApp.Business.Services.Concrete;
using PortfolioApp.Business.Tests.TestSupport;
using PortfolioApp.DTO.DTOs.Site;
using PortfolioApp.Entity.Concrete;
using Xunit;

namespace PortfolioApp.Business.Tests.Services;

public class HeroServiceTests : ServiceTestBase
{
    private HeroService CreateSut() => new(Uow, Mapper, CurrentUser);

    [Fact]
    public async Task GetActiveAsync_ReturnsOnlyTheGivenOwnersActiveHero()
    {
        await SeedAsync(
            new HeroSection { UserId = TestTenants.TenantA, Title = "A Hero", IsActive = true },
            new HeroSection { UserId = TestTenants.TenantB, Title = "B Hero", IsActive = true });

        var result = await CreateSut().GetActiveAsync(TestTenants.TenantA);

        Assert.True(result.Success);
        Assert.Equal("A Hero", result.Data!.Title);
    }

    [Fact]
    public async Task GetActiveAsync_NoActiveHeroForOwner_ReturnsNullData()
    {
        var result = await CreateSut().GetActiveAsync(TestTenants.TenantA);

        Assert.True(result.Success);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateAsync_HeroBelongsToDifferentTenant_ReturnsFailure()
    {
        var hero = new HeroSection { UserId = TestTenants.TenantB, Title = "B Hero", IsActive = true };
        await SeedAsync(hero);

        CurrentUser.UserId = TestTenants.TenantA;
        var dto = new HeroSectionUpdateDto { Id = hero.Id, Title = "Hijacked" };

        var result = await CreateSut().UpdateAsync(dto);

        Assert.False(result.Success);
        var stored = Context.HeroSections.Single(h => h.Id == hero.Id);
        Assert.Equal("B Hero", stored.Title);
    }

    [Fact]
    public async Task UpdateAsync_OwnHero_UpdatesFields()
    {
        var hero = new HeroSection { UserId = TestTenants.TenantA, Title = "Old Title", IsActive = true };
        await SeedAsync(hero);

        CurrentUser.UserId = TestTenants.TenantA;
        var dto = new HeroSectionUpdateDto { Id = hero.Id, Title = "New Title" };

        var result = await CreateSut().UpdateAsync(dto);

        Assert.True(result.Success);
        var stored = Context.HeroSections.Single(h => h.Id == hero.Id);
        Assert.Equal("New Title", stored.Title);
    }
}
