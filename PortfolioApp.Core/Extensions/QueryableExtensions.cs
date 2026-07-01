namespace PortfolioApp.Core.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int page, int pageSize)
    {
        var normalizedPage = Math.Max(1, page);
        var normalizedPageSize = Math.Max(1, pageSize);
        return query.Skip((normalizedPage - 1) * normalizedPageSize).Take(normalizedPageSize);
    }

    public static IQueryable<T> ApplyOrdering<T, TKey>(
        this IQueryable<T> query,
        System.Linq.Expressions.Expression<Func<T, TKey>> keySelector,
        bool descending = false)
    {
        return descending
            ? query.OrderByDescending(keySelector)
            : query.OrderBy(keySelector);
    }
}
