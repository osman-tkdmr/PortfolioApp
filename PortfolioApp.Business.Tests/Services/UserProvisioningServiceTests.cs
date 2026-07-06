using PortfolioApp.Business.Services.Concrete;
using PortfolioApp.Business.Tests.TestSupport;
using Xunit;

namespace PortfolioApp.Business.Tests.Services;

public class UserProvisioningServiceTests : ServiceTestBase
{
    private UserProvisioningService CreateSut() => new(Uow);

    [Fact]
    public async Task ProvisionDefaultsAsync_CreatesOneRowPerDefault()
    {
        await CreateSut().ProvisionDefaultsAsync(TestTenants.TenantA);

        Assert.Single(Context.SiteSettings.Where(s => s.UserId == TestTenants.TenantA));
        Assert.Equal(5, Context.SeoSettings.Count(s => s.UserId == TestTenants.TenantA));
        Assert.Single(Context.ContactInfos.Where(c => c.UserId == TestTenants.TenantA));
        Assert.Single(Context.HeroSections.Where(h => h.UserId == TestTenants.TenantA));
    }

    [Fact]
    public async Task ProvisionDefaultsAsync_CalledTwice_IsIdempotent()
    {
        var sut = CreateSut();
        await sut.ProvisionDefaultsAsync(TestTenants.TenantA);
        await sut.ProvisionDefaultsAsync(TestTenants.TenantA);

        Assert.Single(Context.SiteSettings.Where(s => s.UserId == TestTenants.TenantA));
        Assert.Equal(5, Context.SeoSettings.Count(s => s.UserId == TestTenants.TenantA));
        Assert.Single(Context.ContactInfos.Where(c => c.UserId == TestTenants.TenantA));
        Assert.Single(Context.HeroSections.Where(h => h.UserId == TestTenants.TenantA));
    }

    [Fact]
    public async Task ProvisionDefaultsAsync_DoesNotAffectOtherTenants()
    {
        await CreateSut().ProvisionDefaultsAsync(TestTenants.TenantA);
        await CreateSut().ProvisionDefaultsAsync(TestTenants.TenantB);

        Assert.Single(Context.SiteSettings.Where(s => s.UserId == TestTenants.TenantA));
        Assert.Single(Context.SiteSettings.Where(s => s.UserId == TestTenants.TenantB));
    }
}
