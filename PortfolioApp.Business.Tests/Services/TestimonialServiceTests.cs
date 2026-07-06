using PortfolioApp.Business.Services.Concrete;
using PortfolioApp.Business.Tests.TestSupport;
using PortfolioApp.DTO.DTOs.Site;
using PortfolioApp.Entity.Concrete;
using Xunit;

namespace PortfolioApp.Business.Tests.Services;

public class TestimonialServiceTests : ServiceTestBase
{
    private TestimonialService CreateSut() => new(Uow, Mapper, CurrentUser);

    [Fact]
    public async Task GetAllActiveAsync_AdminListing_ScopesToCurrentUser()
    {
        await SeedAsync(
            new Testimonial { UserId = TestTenants.TenantA, AuthorName = "A Author", Content = "x", IsActive = true, IsApproved = false },
            new Testimonial { UserId = TestTenants.TenantB, AuthorName = "B Author", Content = "x", IsActive = true, IsApproved = false });

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().GetAllActiveAsync();

        Assert.True(result.Success);
        var item = Assert.Single(result.Data!);
        Assert.Equal("A Author", item.AuthorName);
    }

    [Fact]
    public async Task GetApprovedAsync_PublicListing_ScopesToOwnerAndExcludesUnapproved()
    {
        await SeedAsync(
            new Testimonial { UserId = TestTenants.TenantA, AuthorName = "A Approved", Content = "x", IsActive = true, IsApproved = true },
            new Testimonial { UserId = TestTenants.TenantA, AuthorName = "A Pending", Content = "x", IsActive = true, IsApproved = false },
            new Testimonial { UserId = TestTenants.TenantB, AuthorName = "B Approved", Content = "x", IsActive = true, IsApproved = true });

        var result = await CreateSut().GetApprovedAsync(TestTenants.TenantA);

        Assert.True(result.Success);
        var item = Assert.Single(result.Data!);
        Assert.Equal("A Approved", item.AuthorName);
    }

    [Fact]
    public async Task GetByIdAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var t = new Testimonial { UserId = TestTenants.TenantB, AuthorName = "B Author", Content = "x" };
        await SeedAsync(t);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().GetByIdAsync(t.Id);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task CreateAsync_StampsCurrentUserId()
    {
        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().CreateAsync(new TestimonialCreateDto { AuthorName = "New Author", Content = "Great work!" });

        Assert.True(result.Success);
        Assert.Equal(TestTenants.TenantA, Context.Testimonials.Single().UserId);
    }

    [Fact]
    public async Task UpdateAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var t = new Testimonial { UserId = TestTenants.TenantB, AuthorName = "B Author", Content = "x" };
        await SeedAsync(t);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().UpdateAsync(t.Id, new TestimonialUpdateDto { Id = t.Id, AuthorName = "Hijacked", Content = "y" });

        Assert.False(result.Success);
    }

    [Fact]
    public async Task DeleteAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var t = new Testimonial { UserId = TestTenants.TenantB, AuthorName = "B Author", Content = "x" };
        await SeedAsync(t);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().DeleteAsync(t.Id);

        Assert.False(result.Success);
        Assert.False(Context.Testimonials.Single(x => x.Id == t.Id).IsDeleted);
    }
}
