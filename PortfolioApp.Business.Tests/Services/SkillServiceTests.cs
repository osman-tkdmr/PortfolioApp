using PortfolioApp.Business.Services.Concrete;
using PortfolioApp.Business.Tests.TestSupport;
using PortfolioApp.DTO.DTOs.Portfolio;
using PortfolioApp.Entity.Concrete;
using Xunit;

namespace PortfolioApp.Business.Tests.Services;

public class SkillServiceTests : ServiceTestBase
{
    private SkillService CreateSut() => new(Uow, Mapper, CurrentUser);

    private async Task<SkillCategory> SeedCategoryAsync(string ownerId, string name = "Languages")
    {
        var cat = new SkillCategory { UserId = ownerId, Name = name };
        await SeedAsync(cat);
        return cat;
    }

    [Fact]
    public async Task GetCategoriesWithSkillsAsync_ScopesToOwner()
    {
        var catA = await SeedCategoryAsync(TestTenants.TenantA, "A-Cat");
        var catB = await SeedCategoryAsync(TestTenants.TenantB, "B-Cat");
        await SeedAsync(
            new Skill { UserId = TestTenants.TenantA, Name = "C#", SkillCategoryId = catA.Id },
            new Skill { UserId = TestTenants.TenantB, Name = "Go", SkillCategoryId = catB.Id });

        var result = await CreateSut().GetCategoriesWithSkillsAsync(TestTenants.TenantA);

        Assert.True(result.Success);
        var cat = Assert.Single(result.Data!);
        Assert.Equal("A-Cat", cat.Name);
        var skill = Assert.Single(cat.Skills);
        Assert.Equal("C#", skill.Name);
    }

    // ---- GetAllSkillsAsync: regression test for the fixed cross-tenant leak ----
    [Fact]
    public async Task GetAllSkillsAsync_OnlyReturnsCurrentUsersSkills()
    {
        var catA = await SeedCategoryAsync(TestTenants.TenantA, "A-Cat");
        var catB = await SeedCategoryAsync(TestTenants.TenantB, "B-Cat");
        await SeedAsync(
            new Skill { UserId = TestTenants.TenantA, Name = "C#", SkillCategoryId = catA.Id },
            new Skill { UserId = TestTenants.TenantB, Name = "Go", SkillCategoryId = catB.Id });

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().GetAllSkillsAsync();

        Assert.True(result.Success);
        var skill = Assert.Single(result.Data!);
        Assert.Equal("C#", skill.Name);
    }

    // ---- GetSkillByIdAsync: regression test for the fixed cross-tenant leak ----
    [Fact]
    public async Task GetSkillByIdAsync_SkillBelongsToDifferentTenant_ReturnsFailure()
    {
        var catB = await SeedCategoryAsync(TestTenants.TenantB, "B-Cat");
        var skill = new Skill { UserId = TestTenants.TenantB, Name = "Go", SkillCategoryId = catB.Id };
        await SeedAsync(skill);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().GetSkillByIdAsync(skill.Id);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task GetSkillByIdAsync_OwnSkill_ReturnsData()
    {
        var catA = await SeedCategoryAsync(TestTenants.TenantA, "A-Cat");
        var skill = new Skill { UserId = TestTenants.TenantA, Name = "C#", SkillCategoryId = catA.Id };
        await SeedAsync(skill);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().GetSkillByIdAsync(skill.Id);

        Assert.True(result.Success);
        Assert.Equal("C#", result.Data!.Name);
    }

    [Fact]
    public async Task CreateSkillAsync_StampsCurrentUserId()
    {
        var catA = await SeedCategoryAsync(TestTenants.TenantA, "A-Cat");
        CurrentUser.UserId = TestTenants.TenantA;

        var result = await CreateSut().CreateSkillAsync(new SkillCreateDto { Name = "C#", SkillCategoryId = catA.Id });

        Assert.True(result.Success);
        Assert.Equal(TestTenants.TenantA, Context.Skills.Single().UserId);
    }

    [Fact]
    public async Task UpdateSkillAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var catB = await SeedCategoryAsync(TestTenants.TenantB, "B-Cat");
        var skill = new Skill { UserId = TestTenants.TenantB, Name = "Go", SkillCategoryId = catB.Id };
        await SeedAsync(skill);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().UpdateSkillAsync(skill.Id, new SkillUpdateDto { Id = skill.Id, Name = "Hijacked", SkillCategoryId = catB.Id });

        Assert.False(result.Success);
        Assert.Equal("Go", Context.Skills.Single(s => s.Id == skill.Id).Name);
    }

    [Fact]
    public async Task DeleteSkillAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var catB = await SeedCategoryAsync(TestTenants.TenantB, "B-Cat");
        var skill = new Skill { UserId = TestTenants.TenantB, Name = "Go", SkillCategoryId = catB.Id };
        await SeedAsync(skill);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().DeleteSkillAsync(skill.Id);

        Assert.False(result.Success);
        Assert.False(Context.Skills.Single(s => s.Id == skill.Id).IsDeleted);
    }

    // ---- GetAllCategoriesAsync: regression test for the fixed cross-tenant leak ----
    [Fact]
    public async Task GetAllCategoriesAsync_OnlyReturnsCurrentUsersCategories()
    {
        await SeedCategoryAsync(TestTenants.TenantA, "A-Cat");
        await SeedCategoryAsync(TestTenants.TenantB, "B-Cat");

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().GetAllCategoriesAsync();

        Assert.True(result.Success);
        var cat = Assert.Single(result.Data!);
        Assert.Equal("A-Cat", cat.Name);
    }

    [Fact]
    public async Task UpdateCategoryAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var catB = await SeedCategoryAsync(TestTenants.TenantB, "B-Cat");

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().UpdateCategoryAsync(catB.Id, new SkillCategoryUpdateDto { Id = catB.Id, Name = "Hijacked" });

        Assert.False(result.Success);
    }

    [Fact]
    public async Task DeleteCategoryAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var catB = await SeedCategoryAsync(TestTenants.TenantB, "B-Cat");

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().DeleteCategoryAsync(catB.Id);

        Assert.False(result.Success);
        Assert.False(Context.SkillCategories.Single(c => c.Id == catB.Id).IsDeleted);
    }
}
