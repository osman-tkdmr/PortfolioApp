using PortfolioApp.Business.Services.Concrete;
using PortfolioApp.Business.Tests.TestSupport;
using PortfolioApp.DTO.DTOs.Site;
using PortfolioApp.Entity.Concrete;
using Xunit;

namespace PortfolioApp.Business.Tests.Services;

public class MenuServiceTests : ServiceTestBase
{
    private MenuService CreateSut() => new(Uow, Mapper, CurrentUser);

    [Fact]
    public async Task GetHeaderMenuAsync_ScopesToCurrentUser()
    {
        await SeedAsync(
            new MenuItem { UserId = TestTenants.TenantA, Title = "A Item", IsActive = true },
            new MenuItem { UserId = TestTenants.TenantB, Title = "B Item", IsActive = true });

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().GetHeaderMenuAsync();

        Assert.True(result.Success);
        var item = Assert.Single(result.Data!);
        Assert.Equal("A Item", item.Title);
    }

    [Fact]
    public async Task GetAllAsync_ScopesToCurrentUser()
    {
        await SeedAsync(
            new MenuItem { UserId = TestTenants.TenantA, Title = "A Item" },
            new MenuItem { UserId = TestTenants.TenantB, Title = "B Item" });

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().GetAllAsync();

        Assert.True(result.Success);
        var item = Assert.Single(result.Data!);
        Assert.Equal("A Item", item.Title);
    }

    [Fact]
    public async Task GetByIdAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var item = new MenuItem { UserId = TestTenants.TenantB, Title = "B Item" };
        await SeedAsync(item);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().GetByIdAsync(item.Id);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task CreateAsync_StampsCurrentUserId()
    {
        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().CreateAsync(new MenuItemCreateDto { Title = "New Item" });

        Assert.True(result.Success);
        Assert.Equal(TestTenants.TenantA, Context.MenuItems.Single().UserId);
    }

    [Fact]
    public async Task UpdateAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var item = new MenuItem { UserId = TestTenants.TenantB, Title = "B Item" };
        await SeedAsync(item);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().UpdateAsync(item.Id, new MenuItemUpdateDto { Id = item.Id, Title = "Hijacked" });

        Assert.False(result.Success);
    }

    [Fact]
    public async Task DeleteAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var item = new MenuItem { UserId = TestTenants.TenantB, Title = "B Item" };
        await SeedAsync(item);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().DeleteAsync(item.Id);

        Assert.False(result.Success);
        Assert.False(Context.MenuItems.Single(m => m.Id == item.Id).IsDeleted);
    }
}
