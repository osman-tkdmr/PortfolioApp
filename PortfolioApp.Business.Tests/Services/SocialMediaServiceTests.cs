using PortfolioApp.Business.Services.Concrete;
using PortfolioApp.Business.Tests.TestSupport;
using PortfolioApp.DTO.DTOs.Site;
using PortfolioApp.Entity.Concrete;
using Xunit;

namespace PortfolioApp.Business.Tests.Services;

public class SocialMediaServiceTests : ServiceTestBase
{
    private SocialMediaService CreateSut() => new(Uow, Mapper, CurrentUser);

    [Fact]
    public async Task GetAllActiveAsync_ScopesToOwner()
    {
        await SeedAsync(
            new SocialMedia { UserId = TestTenants.TenantA, Platform = "GitHub", Url = "https://github.com/a", IsActive = true },
            new SocialMedia { UserId = TestTenants.TenantB, Platform = "GitHub", Url = "https://github.com/b", IsActive = true });

        var result = await CreateSut().GetAllActiveAsync(TestTenants.TenantA);

        Assert.True(result.Success);
        var item = Assert.Single(result.Data!);
        Assert.Equal("https://github.com/a", item.Url);
    }

    [Fact]
    public async Task GetByIdAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var sm = new SocialMedia { UserId = TestTenants.TenantB, Platform = "GitHub", Url = "https://github.com/b" };
        await SeedAsync(sm);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().GetByIdAsync(sm.Id);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task CreateAsync_StampsCurrentUserId()
    {
        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().CreateAsync(new SocialMediaCreateDto { Platform = "LinkedIn", Url = "https://linkedin.com/a" });

        Assert.True(result.Success);
        Assert.Equal(TestTenants.TenantA, Context.SocialMedias.Single().UserId);
    }

    [Fact]
    public async Task UpdateAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var sm = new SocialMedia { UserId = TestTenants.TenantB, Platform = "GitHub", Url = "https://github.com/b" };
        await SeedAsync(sm);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().UpdateAsync(sm.Id, new SocialMediaUpdateDto { Id = sm.Id, Platform = "Hijacked", Url = "https://evil.example" });

        Assert.False(result.Success);
    }

    [Fact]
    public async Task DeleteAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var sm = new SocialMedia { UserId = TestTenants.TenantB, Platform = "GitHub", Url = "https://github.com/b" };
        await SeedAsync(sm);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().DeleteAsync(sm.Id);

        Assert.False(result.Success);
        Assert.False(Context.SocialMedias.Single(s => s.Id == sm.Id).IsDeleted);
    }
}
