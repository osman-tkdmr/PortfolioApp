using PortfolioApp.Business.Services.Concrete;
using PortfolioApp.Business.Tests.TestSupport;
using PortfolioApp.DTO.DTOs.Portfolio;
using PortfolioApp.Entity.Concrete;
using Xunit;

namespace PortfolioApp.Business.Tests.Services;

public class EducationServiceTests : ServiceTestBase
{
    private EducationService CreateSut() => new(Uow, Mapper, CurrentUser);

    [Fact]
    public async Task GetAllActiveAsync_ScopesToOwner()
    {
        await SeedAsync(
            new Education { UserId = TestTenants.TenantA, Degree = "A Degree", School = "A School", IsActive = true },
            new Education { UserId = TestTenants.TenantB, Degree = "B Degree", School = "B School", IsActive = true });

        var result = await CreateSut().GetAllActiveAsync(TestTenants.TenantA);

        Assert.True(result.Success);
        var item = Assert.Single(result.Data!);
        Assert.Equal("A Degree", item.Degree);
    }

    [Fact]
    public async Task GetByIdAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var edu = new Education { UserId = TestTenants.TenantB, Degree = "B Degree", School = "B School" };
        await SeedAsync(edu);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().GetByIdAsync(edu.Id);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task CreateAsync_StampsCurrentUserId()
    {
        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().CreateAsync(new EducationCreateDto { Degree = "New Degree", School = "New School" });

        Assert.True(result.Success);
        Assert.Equal(TestTenants.TenantA, Context.Educations.Single().UserId);
    }

    [Fact]
    public async Task UpdateAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var edu = new Education { UserId = TestTenants.TenantB, Degree = "B Degree", School = "B School" };
        await SeedAsync(edu);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().UpdateAsync(edu.Id, new EducationUpdateDto { Id = edu.Id, Degree = "Hijacked", School = "X" });

        Assert.False(result.Success);
    }

    [Fact]
    public async Task DeleteAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var edu = new Education { UserId = TestTenants.TenantB, Degree = "B Degree", School = "B School" };
        await SeedAsync(edu);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().DeleteAsync(edu.Id);

        Assert.False(result.Success);
        Assert.False(Context.Educations.Single(e => e.Id == edu.Id).IsDeleted);
    }
}
