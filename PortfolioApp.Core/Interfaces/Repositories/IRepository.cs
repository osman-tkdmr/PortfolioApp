using PortfolioApp.Core.Entities;
using System.Linq.Expressions;

namespace PortfolioApp.Core.Interfaces.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IList<T>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<IList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    IQueryable<T> GetQueryable();
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Delete(T entity);
    Task SoftDeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
}
