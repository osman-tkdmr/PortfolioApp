using PortfolioApp.Core.Entities;

namespace PortfolioApp.Core.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IRepository<T> GetRepository<T>() where T : BaseEntity;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
