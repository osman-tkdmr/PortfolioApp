using PortfolioApp.Business.Services.Concrete;
using PortfolioApp.Business.Tests.TestSupport;
using PortfolioApp.DTO.DTOs.Portfolio;
using PortfolioApp.Entity.Concrete;
using Xunit;

namespace PortfolioApp.Business.Tests.Services;

public class LanguageServiceTests : ServiceTestBase
{
    private LanguageService CreateSut() => new(Uow, Mapper, CurrentUser);

    [Fact]
    public async Task GetAllActiveAsync_ScopesToOwner()
    {
        await SeedAsync(
            new Language { UserId = TestTenants.TenantA, Name = "English", IsActive = true },
            new Language { UserId = TestTenants.TenantB, Name = "German", IsActive = true });

        var result = await CreateSut().GetAllActiveAsync(TestTenants.TenantA);

        Assert.True(result.Success);
        var item = Assert.Single(result.Data!);
        Assert.Equal("English", item.Name);
    }

    [Fact]
    public async Task GetByIdAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var lang = new Language { UserId = TestTenants.TenantB, Name = "German" };
        await SeedAsync(lang);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().GetByIdAsync(lang.Id);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task CreateAsync_StampsCurrentUserId()
    {
        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().CreateAsync(new LanguageCreateDto { Name = "French" });

        Assert.True(result.Success);
        Assert.Equal(TestTenants.TenantA, Context.Languages.Single().UserId);
    }

    [Fact]
    public async Task UpdateAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var lang = new Language { UserId = TestTenants.TenantB, Name = "German" };
        await SeedAsync(lang);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().UpdateAsync(lang.Id, new LanguageUpdateDto { Id = lang.Id, Name = "Hijacked" });

        Assert.False(result.Success);
    }

    [Fact]
    public async Task DeleteAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var lang = new Language { UserId = TestTenants.TenantB, Name = "German" };
        await SeedAsync(lang);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().DeleteAsync(lang.Id);

        Assert.False(result.Success);
        Assert.False(Context.Languages.Single(l => l.Id == lang.Id).IsDeleted);
    }
}
