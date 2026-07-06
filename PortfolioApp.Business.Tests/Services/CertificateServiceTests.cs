using PortfolioApp.Business.Services.Concrete;
using PortfolioApp.Business.Tests.TestSupport;
using PortfolioApp.DTO.DTOs.Portfolio;
using PortfolioApp.Entity.Concrete;
using Xunit;

namespace PortfolioApp.Business.Tests.Services;

public class CertificateServiceTests : ServiceTestBase
{
    private CertificateService CreateSut() => new(Uow, Mapper, CurrentUser);

    [Fact]
    public async Task GetAllActiveAsync_ScopesToOwner()
    {
        await SeedAsync(
            new Certificate { UserId = TestTenants.TenantA, Name = "A Cert", IsActive = true },
            new Certificate { UserId = TestTenants.TenantB, Name = "B Cert", IsActive = true });

        var result = await CreateSut().GetAllActiveAsync(TestTenants.TenantA);

        Assert.True(result.Success);
        var item = Assert.Single(result.Data!);
        Assert.Equal("A Cert", item.Name);
    }

    [Fact]
    public async Task GetByIdAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var cert = new Certificate { UserId = TestTenants.TenantB, Name = "B Cert" };
        await SeedAsync(cert);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().GetByIdAsync(cert.Id);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task CreateAsync_StampsCurrentUserId()
    {
        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().CreateAsync(new CertificateCreateDto { Name = "New Cert" });

        Assert.True(result.Success);
        Assert.Equal(TestTenants.TenantA, Context.Certificates.Single().UserId);
    }

    [Fact]
    public async Task UpdateAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var cert = new Certificate { UserId = TestTenants.TenantB, Name = "B Cert" };
        await SeedAsync(cert);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().UpdateAsync(cert.Id, new CertificateUpdateDto { Id = cert.Id, Name = "Hijacked" });

        Assert.False(result.Success);
    }

    [Fact]
    public async Task DeleteAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var cert = new Certificate { UserId = TestTenants.TenantB, Name = "B Cert" };
        await SeedAsync(cert);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().DeleteAsync(cert.Id);

        Assert.False(result.Success);
        Assert.False(Context.Certificates.Single(c => c.Id == cert.Id).IsDeleted);
    }
}
