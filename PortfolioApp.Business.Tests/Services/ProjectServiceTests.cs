using PortfolioApp.Business.Services.Concrete;
using PortfolioApp.Business.Tests.TestSupport;
using PortfolioApp.Business.Validators;
using PortfolioApp.DTO.DTOs.Project;
using PortfolioApp.Entity.Concrete;
using Xunit;

namespace PortfolioApp.Business.Tests.Services;

public class ProjectServiceTests : ServiceTestBase
{
    private readonly ProjectCreateValidator _createValidator = new();

    private ProjectService CreateSut() => new(Uow, Mapper, _createValidator, CurrentUser);

    private async Task<ProjectCategory> SeedCategoryAsync(string ownerId, string name = "Web")
    {
        var cat = new ProjectCategory { UserId = ownerId, Name = name, Slug = name.ToLowerInvariant() };
        await SeedAsync(cat);
        return cat;
    }

    // ---- CreateAsync ----

    [Fact]
    public async Task CreateAsync_ValidDto_StampsCurrentUserIdAndPersists()
    {
        var category = await SeedCategoryAsync(TestTenants.TenantA);
        CurrentUser.UserId = TestTenants.TenantA;
        var dto = new ProjectCreateDto { Title = "My Project", ProjectCategoryId = category.Id };

        var result = await CreateSut().CreateAsync(dto);

        Assert.True(result.Success);
        var saved = Assert.Single(Context.Projects);
        Assert.Equal(TestTenants.TenantA, saved.UserId);
        Assert.Equal("my-project", saved.Slug);
    }

    [Fact]
    public async Task CreateAsync_InvalidDto_ReturnsFailureWithoutPersisting()
    {
        CurrentUser.UserId = TestTenants.TenantA;
        var dto = new ProjectCreateDto { Title = "", ProjectCategoryId = 0 };

        var result = await CreateSut().CreateAsync(dto);

        Assert.False(result.Success);
        Assert.Empty(Context.Projects);
    }

    // ---- GetByIdAsync (tenant isolation) ----

    [Fact]
    public async Task GetByIdAsync_OwnProject_ReturnsData()
    {
        var category = await SeedCategoryAsync(TestTenants.TenantA);
        var project = new Project { UserId = TestTenants.TenantA, Title = "A1", Slug = "a1", ProjectCategoryId = category.Id };
        await SeedAsync(project);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().GetByIdAsync(project.Id);

        Assert.True(result.Success);
        Assert.Equal("A1", result.Data!.Title);
    }

    [Fact]
    public async Task GetByIdAsync_ProjectBelongsToDifferentTenant_ReturnsFailure()
    {
        var category = await SeedCategoryAsync(TestTenants.TenantB);
        var project = new Project { UserId = TestTenants.TenantB, Title = "B's project", Slug = "b-project", ProjectCategoryId = category.Id };
        await SeedAsync(project);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().GetByIdAsync(project.Id);

        Assert.False(result.Success);
    }

    // ---- GetBySlugAsync ----

    [Fact]
    public async Task GetBySlugAsync_WrongOwner_ReturnsFailure()
    {
        var category = await SeedCategoryAsync(TestTenants.TenantB);
        var project = new Project { UserId = TestTenants.TenantB, Title = "B1", Slug = "b1", ProjectCategoryId = category.Id };
        await SeedAsync(project);

        var result = await CreateSut().GetBySlugAsync(TestTenants.TenantA, "b1");

        Assert.False(result.Success);
    }

    // ---- GetAllAsync (admin listing, scoped internally to current user) ----

    [Fact]
    public async Task GetAllAsync_OnlyReturnsCurrentUsersProjects()
    {
        var catA = await SeedCategoryAsync(TestTenants.TenantA);
        var catB = await SeedCategoryAsync(TestTenants.TenantB);
        await SeedAsync(
            new Project { UserId = TestTenants.TenantA, Title = "A1", Slug = "a1", ProjectCategoryId = catA.Id },
            new Project { UserId = TestTenants.TenantB, Title = "B1", Slug = "b1", ProjectCategoryId = catB.Id });

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().GetAllAsync();

        Assert.True(result.Success);
        var project = Assert.Single(result.Data!);
        Assert.Equal("A1", project.Title);
    }

    // ---- GetAllActiveAsync: regression test for the fixed cross-tenant leak ----

    [Fact]
    public async Task GetAllActiveAsync_OnlyReturnsActiveProjectsForGivenOwner()
    {
        var catA = await SeedCategoryAsync(TestTenants.TenantA);
        var catB = await SeedCategoryAsync(TestTenants.TenantB);
        await SeedAsync(
            new Project { UserId = TestTenants.TenantA, IsActive = true, Title = "A1", Slug = "a1", ProjectCategoryId = catA.Id },
            new Project { UserId = TestTenants.TenantA, IsActive = false, Title = "A2-inactive", Slug = "a2", ProjectCategoryId = catA.Id },
            new Project { UserId = TestTenants.TenantB, IsActive = true, Title = "B1", Slug = "b1", ProjectCategoryId = catB.Id });

        var result = await CreateSut().GetAllActiveAsync(TestTenants.TenantA);

        Assert.True(result.Success);
        var project = Assert.Single(result.Data!);
        Assert.Equal("A1", project.Title);
    }

    // ---- GetFeaturedAsync / GetPagedAsync (already correctly scoped via ProjectRepository) ----

    [Fact]
    public async Task GetFeaturedAsync_OnlyReturnsFeaturedActiveProjectsForGivenOwner()
    {
        var catA = await SeedCategoryAsync(TestTenants.TenantA);
        var catB = await SeedCategoryAsync(TestTenants.TenantB);
        await SeedAsync(
            new Project { UserId = TestTenants.TenantA, IsActive = true, IsFeatured = true, Title = "A-Feat", Slug = "a-feat", ProjectCategoryId = catA.Id },
            new Project { UserId = TestTenants.TenantA, IsActive = true, IsFeatured = false, Title = "A-Plain", Slug = "a-plain", ProjectCategoryId = catA.Id },
            new Project { UserId = TestTenants.TenantB, IsActive = true, IsFeatured = true, Title = "B-Feat", Slug = "b-feat", ProjectCategoryId = catB.Id });

        var result = await CreateSut().GetFeaturedAsync(TestTenants.TenantA, 10);

        Assert.True(result.Success);
        var project = Assert.Single(result.Data!);
        Assert.Equal("A-Feat", project.Title);
    }

    [Fact]
    public async Task GetPagedAsync_ScopesToOwnerAndRespectsPageSize()
    {
        var catA = await SeedCategoryAsync(TestTenants.TenantA);
        var catB = await SeedCategoryAsync(TestTenants.TenantB);
        await SeedAsync(
            new Project { UserId = TestTenants.TenantA, IsActive = true, Title = "A1", Slug = "a1", ProjectCategoryId = catA.Id, DisplayOrder = 1 },
            new Project { UserId = TestTenants.TenantA, IsActive = true, Title = "A2", Slug = "a2", ProjectCategoryId = catA.Id, DisplayOrder = 2 },
            new Project { UserId = TestTenants.TenantB, IsActive = true, Title = "B1", Slug = "b1", ProjectCategoryId = catB.Id, DisplayOrder = 1 });

        var result = await CreateSut().GetPagedAsync(TestTenants.TenantA, page: 1, pageSize: 1);

        Assert.True(result.Success);
        Assert.Equal(2, result.Data!.TotalCount);
        var project = Assert.Single(result.Data!.Items);
        Assert.Equal("A1", project.Title);
    }

    // ---- UpdateAsync ----

    [Fact]
    public async Task UpdateAsync_ProjectBelongsToDifferentTenant_ReturnsFailureAndLeavesDataUnchanged()
    {
        var catB = await SeedCategoryAsync(TestTenants.TenantB);
        var project = new Project { UserId = TestTenants.TenantB, Title = "B1", Slug = "b1", ProjectCategoryId = catB.Id };
        await SeedAsync(project);

        CurrentUser.UserId = TestTenants.TenantA;
        var dto = new ProjectUpdateDto { Id = project.Id, Title = "Hijacked", ProjectCategoryId = catB.Id };

        var result = await CreateSut().UpdateAsync(project.Id, dto);

        Assert.False(result.Success);
        Assert.Equal("B1", Context.Projects.Single(p => p.Id == project.Id).Title);
    }

    [Fact]
    public async Task UpdateAsync_OwnProject_UpdatesFields()
    {
        var catA = await SeedCategoryAsync(TestTenants.TenantA);
        var project = new Project { UserId = TestTenants.TenantA, Title = "Old", Slug = "old", ProjectCategoryId = catA.Id };
        await SeedAsync(project);

        CurrentUser.UserId = TestTenants.TenantA;
        var dto = new ProjectUpdateDto { Id = project.Id, Title = "New Title", ProjectCategoryId = catA.Id };

        var result = await CreateSut().UpdateAsync(project.Id, dto);

        Assert.True(result.Success);
        Assert.Equal("New Title", Context.Projects.Single(p => p.Id == project.Id).Title);
    }

    // ---- DeleteAsync ----

    [Fact]
    public async Task DeleteAsync_ProjectBelongsToDifferentTenant_ReturnsFailure()
    {
        var catB = await SeedCategoryAsync(TestTenants.TenantB);
        var project = new Project { UserId = TestTenants.TenantB, Title = "B1", Slug = "b1", ProjectCategoryId = catB.Id };
        await SeedAsync(project);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().DeleteAsync(project.Id);

        Assert.False(result.Success);
        Assert.False(Context.Projects.Single(p => p.Id == project.Id).IsDeleted);
    }

    [Fact]
    public async Task DeleteAsync_OwnProject_SoftDeletes()
    {
        var catA = await SeedCategoryAsync(TestTenants.TenantA);
        var project = new Project { UserId = TestTenants.TenantA, Title = "A1", Slug = "a1", ProjectCategoryId = catA.Id };
        await SeedAsync(project);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().DeleteAsync(project.Id);

        Assert.True(result.Success);
        Assert.Empty(Context.Projects); // global soft-delete query filter excludes it
    }

    // ---- ToggleFeaturedAsync ----

    [Fact]
    public async Task ToggleFeaturedAsync_ProjectBelongsToDifferentTenant_ReturnsFailure()
    {
        var catB = await SeedCategoryAsync(TestTenants.TenantB);
        var project = new Project { UserId = TestTenants.TenantB, Title = "B1", Slug = "b1", ProjectCategoryId = catB.Id, IsFeatured = false };
        await SeedAsync(project);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().ToggleFeaturedAsync(project.Id);

        Assert.False(result.Success);
        Assert.False(Context.Projects.Single(p => p.Id == project.Id).IsFeatured);
    }

    [Fact]
    public async Task ToggleFeaturedAsync_OwnProject_TogglesFlag()
    {
        var catA = await SeedCategoryAsync(TestTenants.TenantA);
        var project = new Project { UserId = TestTenants.TenantA, Title = "A1", Slug = "a1", ProjectCategoryId = catA.Id, IsFeatured = false };
        await SeedAsync(project);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().ToggleFeaturedAsync(project.Id);

        Assert.True(result.Success);
        Assert.True(Context.Projects.Single(p => p.Id == project.Id).IsFeatured);
    }

    // ---- Categories ----

    [Fact]
    public async Task GetCategoriesAsync_ScopesToOwner()
    {
        await SeedCategoryAsync(TestTenants.TenantA, "A-Cat");
        await SeedCategoryAsync(TestTenants.TenantB, "B-Cat");

        var result = await CreateSut().GetCategoriesAsync(TestTenants.TenantA);

        Assert.True(result.Success);
        var cat = Assert.Single(result.Data!);
        Assert.Equal("A-Cat", cat.Name);
    }

    [Fact]
    public async Task UpdateCategoryAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var catB = await SeedCategoryAsync(TestTenants.TenantB);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().UpdateCategoryAsync(catB.Id, new ProjectCategoryUpdateDto { Id = catB.Id, Name = "Hijacked" });

        Assert.False(result.Success);
    }

    [Fact]
    public async Task DeleteCategoryAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var catB = await SeedCategoryAsync(TestTenants.TenantB);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().DeleteCategoryAsync(catB.Id);

        Assert.False(result.Success);
        Assert.False(Context.ProjectCategories.Single(c => c.Id == catB.Id).IsDeleted);
    }

    // ---- Technologies: shared/global reference data — no tenant scoping expected ----

    [Fact]
    public async Task GetTechnologiesAsync_ReturnsAllTechnologiesRegardlessOfCurrentUser()
    {
        await SeedAsync(new Technology { Name = "React" }, new Technology { Name = "Vue" });

        var result = await CreateSut().GetTechnologiesAsync();

        Assert.True(result.Success);
        Assert.Equal(2, result.Data!.Count);
    }

    [Fact]
    public async Task UpdateTechnologyAsync_UpdatesSharedRow()
    {
        var tech = new Technology { Name = "React" };
        await SeedAsync(tech);

        var result = await CreateSut().UpdateTechnologyAsync(tech.Id, new TechnologyUpdateDto { Id = tech.Id, Name = "React 19" });

        Assert.True(result.Success);
        Assert.Equal("React 19", Context.Technologies.Single(t => t.Id == tech.Id).Name);
    }
}
