using AutoMapper;
using PortfolioApp.DataAccess.Context;
using PortfolioApp.DataAccess.UnitOfWork;

namespace PortfolioApp.Business.Tests.TestSupport;

public abstract class ServiceTestBase : IDisposable
{
    private readonly SqliteTestDbContextFactory _dbFactory = new();

    protected PortfolioDbContext Context { get; }
    protected UnitOfWork Uow { get; }
    protected FakeCurrentUserService CurrentUser { get; } = new();
    protected IMapper Mapper { get; } = TestMapperFactory.Create();

    protected ServiceTestBase()
    {
        Context = _dbFactory.CreateContext();
        Uow = new UnitOfWork(Context);
    }

    /// <summary>
    /// Adds and saves the given entities, then clears the change tracker — simulating a fresh request-scoped
    /// DbContext seeing already-persisted rows. Without this, seeding directly through the shared test Context
    /// leaves the seeded instances tracked, which collides with the AsNoTracking-fetch-then-Attach pattern
    /// GenericRepository.Update uses (a same-key entity gets attached while a different tracked instance
    /// with that key already exists) — a testing artifact only, since production never shares a DbContext
    /// across requests.
    /// </summary>
    protected async Task SeedAsync(params object[] entities)
    {
        foreach (var entity in entities)
            Context.Add(entity);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
    }

    public void Dispose()
    {
        Uow.Dispose();
        _dbFactory.Dispose();
    }
}
