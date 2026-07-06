using Microsoft.Extensions.Caching.Memory;
using PortfolioApp.Business.Services.Concrete;
using PortfolioApp.Business.Tests.TestSupport;
using PortfolioApp.Entity.Concrete;
using Xunit;

namespace PortfolioApp.Business.Tests.Services;

public class DashboardServiceTests : ServiceTestBase
{
    private DashboardService CreateSut()
    {
        var themeService = new ThemeService(Uow, Mapper, new MemoryCache(new MemoryCacheOptions()), CurrentUser);
        return new DashboardService(Uow, themeService, CurrentUser);
    }

    [Fact]
    public async Task GetStatsAsync_OnlyCountsCurrentUsersData()
    {
        var catA = new ProjectCategory { UserId = TestTenants.TenantA, Name = "Web", Slug = "web" };
        var catB = new ProjectCategory { UserId = TestTenants.TenantB, Name = "Web", Slug = "web" };
        await SeedAsync(catA, catB);
        await SeedAsync(
            new Project { UserId = TestTenants.TenantA, Title = "A1", Slug = "a1", ProjectCategoryId = catA.Id },
            new Project { UserId = TestTenants.TenantB, Title = "B1", Slug = "b1", ProjectCategoryId = catB.Id },
            new ContactMessage { UserId = TestTenants.TenantA, SenderName = "A1", SenderEmail = "a@x.com", Subject = "s", Message = "m", IsRead = false },
            new ContactMessage { UserId = TestTenants.TenantB, SenderName = "B1", SenderEmail = "b@x.com", Subject = "s", Message = "m", IsRead = false });

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().GetStatsAsync();

        Assert.True(result.Success);
        Assert.Equal(1, result.Data!.TotalProjects);
        Assert.Equal(1, result.Data!.TotalMessages);
        Assert.Equal(1, result.Data!.UnreadMessages);
    }
}
