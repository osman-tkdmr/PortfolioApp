using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using PortfolioApp.Business.Mappings;

namespace PortfolioApp.Business.Tests.TestSupport;

public static class TestMapperFactory
{
    // These destination properties are always stamped server-side (ICurrentUserService, PortfolioDbContext.SaveChangesAsync,
    // SoftDeleteAsync) rather than via AutoMapper, by this codebase's own convention — ignore them for validation purposes
    // only (they have no matching DTO source member anyway, so this doesn't change any real mapping behavior).
    private static readonly string[] ServerStampedProperties = ["Id", "UserId", "AuthorId", "CreatedAt", "UpdatedAt", "IsDeleted", "DeletedAt"];

    public static MapperConfiguration CreateConfiguration() =>
        new(cfg =>
        {
            foreach (var prop in ServerStampedProperties)
                cfg.AddGlobalIgnore(prop);
            cfg.AddMaps(typeof(BlogMappingProfile).Assembly);
        }, NullLoggerFactory.Instance);

    public static IMapper Create() => CreateConfiguration().CreateMapper();
}
