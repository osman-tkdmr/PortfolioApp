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

    public async Task<int> SaveChangesAsync() =>
        await _context.SaveChangesAsync();

    public async Task BeginTransactionAsync() =>
        _transaction = await _context.Database.BeginTransactionAsync();

    public async Task CommitTransactionAsync()
    {
        if (_transaction is not null)
            await _transaction.CommitAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction is not null)
            await _transaction.RollbackAsync();
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
