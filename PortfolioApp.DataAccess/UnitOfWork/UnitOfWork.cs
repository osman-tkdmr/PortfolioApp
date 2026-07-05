using Microsoft.EntityFrameworkCore.Storage;
using PortfolioApp.Core.Entities;
using PortfolioApp.Core.Interfaces.Repositories;
using PortfolioApp.DataAccess.Context;
using PortfolioApp.DataAccess.Repositories.Concrete;

namespace PortfolioApp.DataAccess.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly PortfolioDbContext _context;
    private IDbContextTransaction? _transaction;
    private readonly Dictionary<Type, object> _repositories = [];

    private BlogPostRepository? _blogPostRepository;
    private ProjectRepository? _projectRepository;

    public UnitOfWork(PortfolioDbContext context)
    {
        _context = context;
    }

    public PortfolioDbContext Context => _context;

    public BlogPostRepository BlogPosts =>
        _blogPostRepository ??= new BlogPostRepository(_context);

    public ProjectRepository Projects =>
        _projectRepository ??= new ProjectRepository(_context);

    public IRepository<T> GetRepository<T>() where T : BaseEntity
    {
        var type = typeof(T);
        if (!_repositories.TryGetValue(type, out var repo))
        {
            repo = new GenericRepository<T>(_context);
            _repositories[type] = repo;
        }
        return (IRepository<T>)repo;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default) =>
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
            await _transaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
            await _transaction.RollbackAsync(cancellationToken);
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
