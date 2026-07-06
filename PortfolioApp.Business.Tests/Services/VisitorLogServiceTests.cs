using PortfolioApp.Business.Services.Concrete;
using PortfolioApp.Business.Tests.TestSupport;
using PortfolioApp.Entity.Concrete;
using Xunit;

namespace PortfolioApp.Business.Tests.Services;

public class VisitorLogServiceTests : ServiceTestBase
{
    private VisitorLogService CreateSut() => new(Context);

    [Fact]
    public async Task LogVisitAsync_PersistsUnderGivenOwner()
    {
        await CreateSut().LogVisitAsync(TestTenants.TenantA, "127.0.0.1", "Mozilla/5.0 Chrome", "/u/a", null, "session-1", isBot: false);

        var log = Assert.Single(Context.VisitorLogs);
        Assert.Equal(TestTenants.TenantA, log.UserId);
        Assert.Equal("Chrome", log.Browser);
    }

    [Fact]
    public async Task GetTotalCountAsync_ScopesToOwnerAndExcludesBots()
    {
        Context.VisitorLogs.AddRange(
            new VisitorLog { UserId = TestTenants.TenantA, IsBot = false },
            new VisitorLog { UserId = TestTenants.TenantA, IsBot = true },
            new VisitorLog { UserId = TestTenants.TenantB, IsBot = false });
        await Context.SaveChangesAsync();

        var count = await CreateSut().GetTotalCountAsync(TestTenants.TenantA);

        Assert.Equal(1, count);
    }

    [Fact]
    public async Task GetTodayCountAsync_ScopesToOwnerAndToday()
    {
        Context.VisitorLogs.AddRange(
            new VisitorLog { UserId = TestTenants.TenantA, IsBot = false, VisitedAt = DateTime.UtcNow },
            new VisitorLog { UserId = TestTenants.TenantA, IsBot = false, VisitedAt = DateTime.UtcNow.AddDays(-2) },
            new VisitorLog { UserId = TestTenants.TenantB, IsBot = false, VisitedAt = DateTime.UtcNow });
        await Context.SaveChangesAsync();

        var count = await CreateSut().GetTodayCountAsync(TestTenants.TenantA);

        Assert.Equal(1, count);
    }

    [Fact]
    public async Task GetLast30DaysAsync_ScopesToOwner()
    {
        Context.VisitorLogs.AddRange(
            new VisitorLog { UserId = TestTenants.TenantA, IsBot = false, VisitedAt = DateTime.UtcNow },
            new VisitorLog { UserId = TestTenants.TenantB, IsBot = false, VisitedAt = DateTime.UtcNow });
        await Context.SaveChangesAsync();

        var result = await CreateSut().GetLast30DaysAsync(TestTenants.TenantA);

        Assert.True(result.Success);
        Assert.Single(result.Data!);
        Assert.Equal(1, result.Data!.Single().Count);
    }
}
