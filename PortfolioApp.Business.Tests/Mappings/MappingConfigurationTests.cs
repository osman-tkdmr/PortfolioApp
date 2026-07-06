using PortfolioApp.Business.Tests.TestSupport;
using Xunit;

namespace PortfolioApp.Business.Tests.Mappings;

public class MappingConfigurationTests
{
    // TestMapperFactory globally ignores the server-stamped properties (Id/UserId/AuthorId/CreatedAt/UpdatedAt/
    // IsDeleted/DeletedAt) for validation purposes — entities intentionally leave those unmapped from Create/Update
    // DTOs (they're stamped by ICurrentUserService / PortfolioDbContext.SaveChangesAsync / SoftDeleteAsync instead).
    // This still catches genuinely broken profiles: unresolvable nested type maps, missing CreateMap registrations,
    // bad ForMember expressions, or any other destination member with no matching source.
    [Fact]
    public void AutoMapperConfiguration_IsValid()
    {
        var config = TestMapperFactory.CreateConfiguration();
        config.AssertConfigurationIsValid();
    }
}
