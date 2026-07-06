using PortfolioApp.Business.Services.Concrete;
using PortfolioApp.Business.Tests.TestSupport;
using PortfolioApp.DTO.DTOs.Site;
using PortfolioApp.Entity.Concrete;
using Xunit;

namespace PortfolioApp.Business.Tests.Services;

public class FooterServiceTests : ServiceTestBase
{
    private FooterService CreateSut() => new(Uow, Mapper, CurrentUser);

    [Fact]
    public async Task GetAllActiveAsync_ScopesToCurrentUser()
    {
        await SeedAsync(
            new FooterContent { UserId = TestTenants.TenantA, SectionTitle = "A Section", IsActive = true },
            new FooterContent { UserId = TestTenants.TenantB, SectionTitle = "B Section", IsActive = true });

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().GetAllActiveAsync();

        Assert.True(result.Success);
        var item = Assert.Single(result.Data!);
        Assert.Equal("A Section", item.SectionTitle);
    }

    [Fact]
    public async Task GetByIdAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var fc = new FooterContent { UserId = TestTenants.TenantB, SectionTitle = "B Section" };
        await SeedAsync(fc);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().GetByIdAsync(fc.Id);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task CreateAsync_StampsCurrentUserId()
    {
        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().CreateAsync(new FooterContentCreateDto { SectionTitle = "New Section" });

        Assert.True(result.Success);
        Assert.Equal(TestTenants.TenantA, Context.FooterContents.Single().UserId);
    }

    [Fact]
    public async Task UpdateAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var fc = new FooterContent { UserId = TestTenants.TenantB, SectionTitle = "B Section" };
        await SeedAsync(fc);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().UpdateAsync(fc.Id, new FooterContentUpdateDto { Id = fc.Id, SectionTitle = "Hijacked" });

        Assert.False(result.Success);
    }

    [Fact]
    public async Task DeleteAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var fc = new FooterContent { UserId = TestTenants.TenantB, SectionTitle = "B Section" };
        await SeedAsync(fc);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().DeleteAsync(fc.Id);

        Assert.False(result.Success);
        Assert.False(Context.FooterContents.Single(f => f.Id == fc.Id).IsDeleted);
    }
}
