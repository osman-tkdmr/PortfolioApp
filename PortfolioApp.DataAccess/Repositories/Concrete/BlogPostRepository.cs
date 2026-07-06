using Microsoft.EntityFrameworkCore;
using PortfolioApp.Core.Results;
using PortfolioApp.DataAccess.Context;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.DataAccess.Repositories.Concrete;

public class BlogPostRepository : GenericRepository<BlogPost>
{
    public BlogPostRepository(PortfolioDbContext context) : base(context) { }

    public async Task<BlogPost?> GetBySlugAsync(string ownerId, string slug, CancellationToken cancellationToken = default) =>
        await _dbSet.AsNoTracking()
            .Include(p => p.BlogCategory)
            .Include(p => p.Author)
            .Include(p => p.BlogPostTags).ThenInclude(t => t.BlogTag)
            .Include(p => p.Comments.Where(c => c.IsApproved && !c.IsDeleted))
                .ThenInclude(c => c.Replies.Where(r => r.IsApproved && !r.IsDeleted))
            .FirstOrDefaultAsync(p => p.AuthorId == ownerId && p.Slug == slug, cancellationToken);

    public async Task<IList<BlogPost>> GetFeaturedAsync(string ownerId, int count, CancellationToken cancellationToken = default) =>
        await _dbSet.AsNoTracking()
            .Where(p => p.AuthorId == ownerId && p.IsFeatured && p.IsPublished)
            .Include(p => p.BlogCategory)
            .Include(p => p.Author)
            .OrderByDescending(p => p.PublishedAt)
            .Take(count)
            .ToListAsync(cancellationToken);

    public async Task<IList<BlogPost>> GetRecentAsync(string ownerId, int count, CancellationToken cancellationToken = default) =>
        await _dbSet.AsNoTracking()
            .Where(p => p.AuthorId == ownerId && p.IsPublished)
            .Include(p => p.BlogCategory)
            .Include(p => p.Author)
            .OrderByDescending(p => p.PublishedAt)
            .Take(count)
            .ToListAsync(cancellationToken);

    public async Task<(IList<BlogPost> Posts, int TotalCount)> GetPagedAsync(string ownerId, int page, int pageSize, string? categorySlug = null, string? tagSlug = null, string? search = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking()
            .Where(p => p.AuthorId == ownerId && p.IsPublished)
            .Include(p => p.BlogCategory)
            .Include(p => p.Author)
            .Include(p => p.BlogPostTags).ThenInclude(t => t.BlogTag)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(categorySlug))
            query = query.Where(p => p.BlogCategory.Slug == categorySlug);

        if (!string.IsNullOrWhiteSpace(tagSlug))
            query = query.Where(p => p.BlogPostTags.Any(t => t.BlogTag.Slug == tagSlug));

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Title.Contains(search) || (p.Summary != null && p.Summary.Contains(search)));

        var totalCount = await query.CountAsync(cancellationToken);
        var posts = await query
            .OrderByDescending(p => p.PublishedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (posts, totalCount);
    }

    public async Task IncrementViewCountAsync(int id, CancellationToken cancellationToken = default)
    {
        await _dbSet.Where(p => p.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.ViewCount, p => p.ViewCount + 1), cancellationToken);
    }
}
