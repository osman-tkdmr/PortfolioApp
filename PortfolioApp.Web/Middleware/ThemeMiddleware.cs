using Microsoft.Extensions.Caching.Memory;
using PortfolioApp.Core.Constants;
using PortfolioApp.DataAccess.Context;
using Microsoft.EntityFrameworkCore;

namespace PortfolioApp.Web.Middleware;

public class ThemeMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;

    public ThemeMiddleware(RequestDelegate next, IMemoryCache cache)
    {
        _next = next;
        _cache = cache;
    }

    public async Task InvokeAsync(HttpContext context, PortfolioDbContext db)
    {
        // Skip admin and static files
        var path = context.Request.Path.Value ?? "";
        if (!path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase) &&
            !path.StartsWith("/uploads", StringComparison.OrdinalIgnoreCase))
        {
            if (!_cache.TryGetValue(AppConstants.CacheKeys.ActiveTheme, out (string Folder, string Css, string Name) theme))
            {
                var activeTheme = await db.Themes
                    .AsNoTracking()
                    .Where(t => t.IsActive && !t.IsDeleted)
                    .Select(t => new { t.FolderName, t.CssFileName, t.Name })
                    .FirstOrDefaultAsync();

                theme = activeTheme is not null
                    ? (activeTheme.FolderName, activeTheme.CssFileName, activeTheme.Name)
                    : ("Modern", "modern.css", "Modern");

                _cache.Set(AppConstants.CacheKeys.ActiveTheme, theme,
                    TimeSpan.FromMinutes(30));
            }

            context.Items["CurrentThemeFolder"] = theme.Folder;
            context.Items["CurrentThemeCss"] = theme.Css;
            context.Items["CurrentThemeName"] = theme.Name;
        }

        await _next(context);
    }
}
