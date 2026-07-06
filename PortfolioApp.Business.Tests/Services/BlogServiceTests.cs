using PortfolioApp.Business.Services.Concrete;
using PortfolioApp.Business.Tests.TestSupport;
using PortfolioApp.Business.Validators;
using PortfolioApp.DTO.DTOs.Blog;
using PortfolioApp.Entity.Concrete;
using Xunit;

namespace PortfolioApp.Business.Tests.Services;

public class BlogServiceTests : ServiceTestBase
{
    private readonly BlogPostCreateValidator _createValidator = new();
    private readonly BlogPostUpdateValidator _updateValidator = new();

    private BlogService CreateSut() => new(Uow, Mapper, _createValidator, _updateValidator, CurrentUser);

    private async Task<BlogCategory> SeedCategoryAsync(string ownerId, string name = "Tech")
    {
        var cat = new BlogCategory { UserId = ownerId, Name = name, Slug = name.ToLowerInvariant() };
        await SeedAsync(cat);
        return cat;
    }

    // BlogPost.AuthorId is a real FK to AspNetUsers (unlike the bare UserId string on IUserOwnedEntity
    // types) — a matching ApplicationUser row must exist before a BlogPost referencing it can be saved.
    private async Task SeedAuthorAsync(string userId) =>
        await ApplicationUserSeeder.SeedAsync(Context, userId, userId);

    // ---- CreateAsync: authorId is an explicit trust-boundary parameter, not read from ICurrentUserService ----

    [Fact]
    public async Task CreateAsync_ValidDto_StampsGivenAuthorId()
    {
        await SeedAuthorAsync(TestTenants.TenantA);
        var category = await SeedCategoryAsync(TestTenants.TenantA);
        var dto = new BlogPostCreateDto { Title = "Hello World", Content = "Some content", BlogCategoryId = category.Id };

        var result = await CreateSut().CreateAsync(dto, TestTenants.TenantA);

        Assert.True(result.Success);
        var saved = Assert.Single(Context.BlogPosts);
        Assert.Equal(TestTenants.TenantA, saved.AuthorId);
        Assert.Equal("hello-world", saved.Slug);
    }

    [Fact]
    public async Task CreateAsync_InvalidDto_ReturnsFailureWithoutPersisting()
    {
        var dto = new BlogPostCreateDto { Title = "", Content = "", BlogCategoryId = 0 };

        var result = await CreateSut().CreateAsync(dto, TestTenants.TenantA);

        Assert.False(result.Success);
        Assert.Empty(Context.BlogPosts);
    }

    // ---- GetByIdAsync (tenant isolation) ----

    [Fact]
    public async Task GetByIdAsync_PostBelongsToDifferentAuthor_ReturnsFailure()
    {
        await SeedAuthorAsync(TestTenants.TenantB);
        var category = await SeedCategoryAsync(TestTenants.TenantB);
        var post = new BlogPost { AuthorId = TestTenants.TenantB, Title = "B Post", Slug = "b-post", Content = "x", BlogCategoryId = category.Id };
        await SeedAsync(post);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().GetByIdAsync(post.Id);

        Assert.False(result.Success);
    }

    // ---- GetPublishedAsync: regression test for the fixed cross-tenant leak ----

    [Fact]
    public async Task GetPublishedAsync_OnlyReturnsPublishedPostsForGivenOwner()
    {
        await SeedAuthorAsync(TestTenants.TenantA);
        await SeedAuthorAsync(TestTenants.TenantB);
        var catA = await SeedCategoryAsync(TestTenants.TenantA);
        var catB = await SeedCategoryAsync(TestTenants.TenantB);
        await SeedAsync(
            new BlogPost { AuthorId = TestTenants.TenantA, IsPublished = true, Title = "A1", Slug = "a1", Content = "x", BlogCategoryId = catA.Id },
            new BlogPost { AuthorId = TestTenants.TenantA, IsPublished = false, Title = "A2-draft", Slug = "a2", Content = "x", BlogCategoryId = catA.Id },
            new BlogPost { AuthorId = TestTenants.TenantB, IsPublished = true, Title = "B1", Slug = "b1", Content = "x", BlogCategoryId = catB.Id });

        var result = await CreateSut().GetPublishedAsync(TestTenants.TenantA);

        Assert.True(result.Success);
        var post = Assert.Single(result.Data!);
        Assert.Equal("A1", post.Title);
    }

    // ---- GetFeaturedAsync: regression test for the fixed cross-tenant leak ----

    [Fact]
    public async Task GetFeaturedAsync_OnlyReturnsFeaturedPublishedPostsForGivenOwner()
    {
        await SeedAuthorAsync(TestTenants.TenantA);
        await SeedAuthorAsync(TestTenants.TenantB);
        var catA = await SeedCategoryAsync(TestTenants.TenantA);
        var catB = await SeedCategoryAsync(TestTenants.TenantB);
        await SeedAsync(
            new BlogPost { AuthorId = TestTenants.TenantA, IsPublished = true, IsFeatured = true, Title = "A-Feat", Slug = "a-feat", Content = "x", BlogCategoryId = catA.Id, PublishedAt = DateTime.UtcNow },
            new BlogPost { AuthorId = TestTenants.TenantB, IsPublished = true, IsFeatured = true, Title = "B-Feat", Slug = "b-feat", Content = "x", BlogCategoryId = catB.Id, PublishedAt = DateTime.UtcNow });

        var result = await CreateSut().GetFeaturedAsync(TestTenants.TenantA, 10);

        Assert.True(result.Success);
        var post = Assert.Single(result.Data!);
        Assert.Equal("A-Feat", post.Title);
    }

    // ---- GetRecentAsync / GetPagedAsync (already correctly scoped via BlogPostRepository) ----

    [Fact]
    public async Task GetRecentAsync_ScopesToOwner()
    {
        await SeedAuthorAsync(TestTenants.TenantA);
        await SeedAuthorAsync(TestTenants.TenantB);
        var catA = await SeedCategoryAsync(TestTenants.TenantA);
        var catB = await SeedCategoryAsync(TestTenants.TenantB);
        await SeedAsync(
            new BlogPost { AuthorId = TestTenants.TenantA, IsPublished = true, Title = "A1", Slug = "a1", Content = "x", BlogCategoryId = catA.Id, PublishedAt = DateTime.UtcNow },
            new BlogPost { AuthorId = TestTenants.TenantB, IsPublished = true, Title = "B1", Slug = "b1", Content = "x", BlogCategoryId = catB.Id, PublishedAt = DateTime.UtcNow });

        var result = await CreateSut().GetRecentAsync(TestTenants.TenantA, 5);

        Assert.True(result.Success);
        var post = Assert.Single(result.Data!);
        Assert.Equal("A1", post.Title);
    }

    [Fact]
    public async Task GetPagedAsync_ScopesToOwner()
    {
        await SeedAuthorAsync(TestTenants.TenantA);
        await SeedAuthorAsync(TestTenants.TenantB);
        var catA = await SeedCategoryAsync(TestTenants.TenantA);
        var catB = await SeedCategoryAsync(TestTenants.TenantB);
        await SeedAsync(
            new BlogPost { AuthorId = TestTenants.TenantA, IsPublished = true, Title = "A1", Slug = "a1", Content = "x", BlogCategoryId = catA.Id, PublishedAt = DateTime.UtcNow },
            new BlogPost { AuthorId = TestTenants.TenantB, IsPublished = true, Title = "B1", Slug = "b1", Content = "x", BlogCategoryId = catB.Id, PublishedAt = DateTime.UtcNow });

        var result = await CreateSut().GetPagedAsync(TestTenants.TenantA, page: 1, pageSize: 10);

        Assert.True(result.Success);
        Assert.Equal(1, result.Data!.TotalCount);
    }

    // ---- UpdateAsync ----

    [Fact]
    public async Task UpdateAsync_PostBelongsToDifferentAuthor_ReturnsFailure()
    {
        await SeedAuthorAsync(TestTenants.TenantB);
        var catB = await SeedCategoryAsync(TestTenants.TenantB);
        var post = new BlogPost { AuthorId = TestTenants.TenantB, Title = "B1", Slug = "b1", Content = "x", BlogCategoryId = catB.Id };
        await SeedAsync(post);

        CurrentUser.UserId = TestTenants.TenantA;
        var dto = new BlogPostUpdateDto { Id = post.Id, Title = "Hijacked", Content = "y", BlogCategoryId = catB.Id };

        var result = await CreateSut().UpdateAsync(post.Id, dto);

        Assert.False(result.Success);
        Assert.Equal("B1", Context.BlogPosts.Single(p => p.Id == post.Id).Title);
    }

    [Fact]
    public async Task UpdateAsync_OwnPost_UpdatesFields()
    {
        await SeedAuthorAsync(TestTenants.TenantA);
        var catA = await SeedCategoryAsync(TestTenants.TenantA);
        var post = new BlogPost { AuthorId = TestTenants.TenantA, Title = "Old", Slug = "old", Content = "x", BlogCategoryId = catA.Id };
        await SeedAsync(post);

        CurrentUser.UserId = TestTenants.TenantA;
        var dto = new BlogPostUpdateDto { Id = post.Id, Title = "New", Content = "y", BlogCategoryId = catA.Id };

        var result = await CreateSut().UpdateAsync(post.Id, dto);

        Assert.True(result.Success);
        Assert.Equal("New", Context.BlogPosts.Single(p => p.Id == post.Id).Title);
    }

    // ---- DeleteAsync / PublishAsync / UnpublishAsync ----

    [Fact]
    public async Task DeleteAsync_PostBelongsToDifferentAuthor_ReturnsFailure()
    {
        await SeedAuthorAsync(TestTenants.TenantB);
        var catB = await SeedCategoryAsync(TestTenants.TenantB);
        var post = new BlogPost { AuthorId = TestTenants.TenantB, Title = "B1", Slug = "b1", Content = "x", BlogCategoryId = catB.Id };
        await SeedAsync(post);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().DeleteAsync(post.Id);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task PublishAsync_OwnPost_SetsPublishedFlagAndTimestamp()
    {
        await SeedAuthorAsync(TestTenants.TenantA);
        var catA = await SeedCategoryAsync(TestTenants.TenantA);
        var post = new BlogPost { AuthorId = TestTenants.TenantA, Title = "A1", Slug = "a1", Content = "x", BlogCategoryId = catA.Id, IsPublished = false };
        await SeedAsync(post);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().PublishAsync(post.Id);

        Assert.True(result.Success);
        var stored = Context.BlogPosts.Single(p => p.Id == post.Id);
        Assert.True(stored.IsPublished);
        Assert.NotNull(stored.PublishedAt);
    }

    [Fact]
    public async Task UnpublishAsync_PostBelongsToDifferentAuthor_ReturnsFailure()
    {
        await SeedAuthorAsync(TestTenants.TenantB);
        var catB = await SeedCategoryAsync(TestTenants.TenantB);
        var post = new BlogPost { AuthorId = TestTenants.TenantB, Title = "B1", Slug = "b1", Content = "x", BlogCategoryId = catB.Id, IsPublished = true };
        await SeedAsync(post);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().UnpublishAsync(post.Id);

        Assert.False(result.Success);
        Assert.True(Context.BlogPosts.Single(p => p.Id == post.Id).IsPublished);
    }

    // ---- IncrementViewCountAsync (intentionally unscoped — public view counter) ----

    [Fact]
    public async Task IncrementViewCountAsync_IncrementsRegardlessOfCurrentUser()
    {
        await SeedAuthorAsync(TestTenants.TenantA);
        var catA = await SeedCategoryAsync(TestTenants.TenantA);
        var post = new BlogPost { AuthorId = TestTenants.TenantA, Title = "A1", Slug = "a1", Content = "x", BlogCategoryId = catA.Id, ViewCount = 0 };
        await SeedAsync(post);

        await CreateSut().IncrementViewCountAsync(post.Id);

        Assert.Equal(1, Context.BlogPosts.Single(p => p.Id == post.Id).ViewCount);
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
    public async Task DeleteCategoryAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var catB = await SeedCategoryAsync(TestTenants.TenantB);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().DeleteCategoryAsync(catB.Id);

        Assert.False(result.Success);
    }

    // ---- Tags ----

    [Fact]
    public async Task GetTagsAsync_ScopesToCurrentUser()
    {
        await SeedAsync(
            new BlogTag { UserId = TestTenants.TenantA, Name = "A-Tag", Slug = "a-tag" },
            new BlogTag { UserId = TestTenants.TenantB, Name = "B-Tag", Slug = "b-tag" });

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().GetTagsAsync();

        Assert.True(result.Success);
        var tag = Assert.Single(result.Data!);
        Assert.Equal("A-Tag", tag.Name);
    }

    [Fact]
    public async Task DeleteTagAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var tag = new BlogTag { UserId = TestTenants.TenantB, Name = "B-Tag", Slug = "b-tag" };
        await SeedAsync(tag);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().DeleteTagAsync(tag.Id);

        Assert.False(result.Success);
    }
}
