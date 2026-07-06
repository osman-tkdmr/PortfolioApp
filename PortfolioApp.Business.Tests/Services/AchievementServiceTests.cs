using PortfolioApp.Business.Services.Concrete;
using PortfolioApp.Business.Tests.TestSupport;
using PortfolioApp.DTO.DTOs.Portfolio;
using PortfolioApp.Entity.Concrete;
using Xunit;

namespace PortfolioApp.Business.Tests.Services;

public class AchievementServiceTests : ServiceTestBase
{
    private AchievementService CreateSut() => new(Uow, Mapper, CurrentUser);

    [Fact]
    public async Task GetAllActiveAsync_ScopesToOwner()
    {
        await SeedAsync(
            new Achievement { UserId = TestTenants.TenantA, Title = "A Achievement", IsActive = true },
            new Achievement { UserId = TestTenants.TenantB, Title = "B Achievement", IsActive = true });

        var result = await CreateSut().GetAllActiveAsync(TestTenants.TenantA);

        Assert.True(result.Success);
        var item = Assert.Single(result.Data!);
        Assert.Equal("A Achievement", item.Title);
    }

    [Fact]
    public async Task GetByIdAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var ach = new Achievement { UserId = TestTenants.TenantB, Title = "B Achievement" };
        await SeedAsync(ach);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().GetByIdAsync(ach.Id);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task CreateAsync_StampsCurrentUserId()
    {
        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().CreateAsync(new AchievementCreateDto { Title = "New Achievement" });

        Assert.True(result.Success);
        Assert.Equal(TestTenants.TenantA, Context.Achievements.Single().UserId);
    }

    [Fact]
    public async Task UpdateAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var ach = new Achievement { UserId = TestTenants.TenantB, Title = "B Achievement" };
        await SeedAsync(ach);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().UpdateAsync(ach.Id, new AchievementUpdateDto { Id = ach.Id, Title = "Hijacked" });

        Assert.False(result.Success);
    }

    [Fact]
    public async Task DeleteAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var ach = new Achievement { UserId = TestTenants.TenantB, Title = "B Achievement" };
        await SeedAsync(ach);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().DeleteAsync(ach.Id);

        Assert.False(result.Success);
        Assert.False(Context.Achievements.Single(a => a.Id == ach.Id).IsDeleted);
    }
}
