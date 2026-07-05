using Microsoft.EntityFrameworkCore;
using PortfolioApp.Core.Entities;
using PortfolioApp.Core.Interfaces.Repositories;
using PortfolioApp.DataAccess.Context;
using System.Linq.Expressions;

namespace PortfolioApp.DataAccess.Repositories.Concrete;

public class GenericRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly PortfolioDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(PortfolioDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public virtual async Task<IList<T>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _dbSet.AsNoTracking().ToListAsync(cancellationToken);

    public virtual async Task<IList<T>> GetAllActiveAsync(CancellationToken cancellationToken = default) =>
        await _dbSet.AsNoTracking()
            .Where(e => EF.Property<bool>(e, "IsActive"))
            .OrderBy(e => EF.Property<int>(e, "DisplayOrder"))
            .ToListAsync(cancellationToken);

    public virtual async Task<IList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) =>
        await _dbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) =>
        await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);

    public virtual IQueryable<T> GetQueryable() => _dbSet.AsQueryable();

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
        await _dbSet.AddAsync(entity, cancellationToken);

    public virtual void Update(T entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    public virtual void Delete(T entity) => _dbSet.Remove(entity);

    public virtual async Task SoftDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FindAsync([id], cancellationToken);
        if (entity is null) return;
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _context.Entry(entity).State = EntityState.Modified;
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) =>
        await _dbSet.AnyAsync(predicate, cancellationToken);

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default) =>
        predicate is null
            ? await _dbSet.CountAsync(cancellationToken)
            : await _dbSet.CountAsync(predicate, cancellationToken);
}
