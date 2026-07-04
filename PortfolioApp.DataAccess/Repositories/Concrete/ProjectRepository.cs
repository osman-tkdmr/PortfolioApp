using Microsoft.EntityFrameworkCore;
using PortfolioApp.DataAccess.Context;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.DataAccess.Repositories.Concrete;

public class ProjectRepository : GenericRepository<Project>
{
    public ProjectRepository(PortfolioDbContext context) : base(context) { }

    public async Task<Project?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default) =>
        await _dbSet.AsNoTracking()
            .Include(p => p.ProjectCategory)
            .Include(p => p.Images.Where(i => !i.IsDeleted).OrderBy(i => i.DisplayOrder))
            .Include(p => p.ProjectTechnologies).ThenInclude(pt => pt.Technology)
            .FirstOrDefaultAsync(p => p.Slug == slug, cancellationToken);

    public async Task<IList<Project>> GetFeaturedAsync(int count, CancellationToken cancellationToken = default) =>
        await _dbSet.AsNoTracking()
            .Where(p => p.IsFeatured && p.IsActive)
            .Include(p => p.ProjectCategory)
            .Include(p => p.ProjectTechnologies).ThenInclude(pt => pt.Technology)
            .OrderBy(p => p.DisplayOrder)
            .Take(count)
            .ToListAsync(cancellationToken);

    public async Task<(IList<Project> Projects, int TotalCount)> GetPagedAsync(int page, int pageSize, string? categorySlug = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking()
            .Where(p => p.IsActive)
            .Include(p => p.ProjectCategory)
            .Include(p => p.ProjectTechnologies).ThenInclude(pt => pt.Technology)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(categorySlug))
            query = query.Where(p => p.ProjectCategory.Slug == categorySlug);

        var totalCount = await query.CountAsync(cancellationToken);
        var projects = await query
            .OrderBy(p => p.DisplayOrder)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (projects, totalCount);
    }

    public async Task<Project?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default) =>
        await _dbSet.AsNoTracking()
            .Include(p => p.ProjectCategory)
            .Include(p => p.Images.Where(i => !i.IsDeleted).OrderBy(i => i.DisplayOrder))
            .Include(p => p.ProjectTechnologies).ThenInclude(pt => pt.Technology)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
}
