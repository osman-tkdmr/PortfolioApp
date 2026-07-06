using PortfolioApp.Business.Services.Concrete;
using PortfolioApp.Business.Tests.TestSupport;
using PortfolioApp.DTO.DTOs.Portfolio;
using PortfolioApp.Entity.Concrete;
using Xunit;

namespace PortfolioApp.Business.Tests.Services;

public class ExperienceServiceTests : ServiceTestBase
{
    private ExperienceService CreateSut() => new(Uow, Mapper, CurrentUser);

    [Fact]
    public async Task GetAllActiveAsync_ScopesToOwner()
    {
        await SeedAsync(
            new Experience { UserId = TestTenants.TenantA, JobTitle = "A Job", Company = "A Co", IsActive = true },
            new Experience { UserId = TestTenants.TenantB, JobTitle = "B Job", Company = "B Co", IsActive = true });

        var result = await CreateSut().GetAllActiveAsync(TestTenants.TenantA);

        Assert.True(result.Success);
        var item = Assert.Single(result.Data!);
        Assert.Equal("A Job", item.JobTitle);
    }

    [Fact]
    public async Task GetByIdAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var exp = new Experience { UserId = TestTenants.TenantB, JobTitle = "B Job", Company = "B Co" };
        await SeedAsync(exp);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().GetByIdAsync(exp.Id);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task CreateAsync_StampsCurrentUserId()
    {
        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().CreateAsync(new ExperienceCreateDto { JobTitle = "New Job", Company = "New Co" });

        Assert.True(result.Success);
        Assert.Equal(TestTenants.TenantA, Context.Experiences.Single().UserId);
    }

    [Fact]
    public async Task UpdateAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var exp = new Experience { UserId = TestTenants.TenantB, JobTitle = "B Job", Company = "B Co" };
        await SeedAsync(exp);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().UpdateAsync(exp.Id, new ExperienceUpdateDto { Id = exp.Id, JobTitle = "Hijacked", Company = "X" });

        Assert.False(result.Success);
    }

    [Fact]
    public async Task DeleteAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var exp = new Experience { UserId = TestTenants.TenantB, JobTitle = "B Job", Company = "B Co" };
        await SeedAsync(exp);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().DeleteAsync(exp.Id);

        Assert.False(result.Success);
        Assert.False(Context.Experiences.Single(e => e.Id == exp.Id).IsDeleted);
    }
}
