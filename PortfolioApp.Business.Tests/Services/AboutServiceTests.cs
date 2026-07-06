using PortfolioApp.Business.Services.Concrete;
using PortfolioApp.Business.Tests.TestSupport;
using PortfolioApp.DTO.DTOs.Portfolio;
using PortfolioApp.Entity.Concrete;
using Xunit;

namespace PortfolioApp.Business.Tests.Services;

public class AboutServiceTests : ServiceTestBase
{
    private AboutService CreateSut() => new(Uow, Mapper, CurrentUser);

    [Fact]
    public async Task GetActiveAsync_ReturnsOnlyTheGivenOwnersActiveAbout()
    {
        await SeedAsync(
            new About { UserId = TestTenants.TenantA, Title = "A About", Content = "x", IsActive = true },
            new About { UserId = TestTenants.TenantB, Title = "B About", Content = "x", IsActive = true });

        var result = await CreateSut().GetActiveAsync(TestTenants.TenantA);

        Assert.True(result.Success);
        Assert.Equal("A About", result.Data!.Title);
    }

    [Fact]
    public async Task UpdateAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var about = new About { UserId = TestTenants.TenantB, Title = "B About", Content = "x", IsActive = true };
        await SeedAsync(about);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().UpdateAsync(new AboutUpdateDto { Id = about.Id, Title = "Hijacked", Content = "y" });

        Assert.False(result.Success);
        Assert.Equal("B About", Context.Abouts.Single(a => a.Id == about.Id).Title);
    }

    [Fact]
    public async Task UpdateAsync_OwnAbout_UpdatesFields()
    {
        var about = new About { UserId = TestTenants.TenantA, Title = "Old", Content = "x", IsActive = true };
        await SeedAsync(about);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().UpdateAsync(new AboutUpdateDto { Id = about.Id, Title = "New", Content = "y" });

        Assert.True(result.Success);
        Assert.Equal("New", Context.Abouts.Single(a => a.Id == about.Id).Title);
    }
}
