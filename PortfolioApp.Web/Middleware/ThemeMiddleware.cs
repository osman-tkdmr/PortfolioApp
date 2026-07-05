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
            var username = context.GetRouteValue("username")?.ToString();
            var cacheKey = string.IsNullOrEmpty(username)
                ? AppConstants.CacheKeys.ActiveTheme
                : $"{AppConstants.CacheKeys.ActiveTheme}:{username}";

            if (!_cache.TryGetValue(cacheKey, out (string Folder, string Css, string Name) theme))
            {
                var ownerId = string.IsNullOrEmpty(username)
                    ? null
                    : await db.Users.Where(u => u.Handle == username).Select(u => u.Id).FirstOrDefaultAsync();

                var activeTheme = ownerId is null
                    ? null
                    : await db.SiteSettings
                        .Where(s => s.UserId == ownerId)
                        .Select(s => s.ActiveTheme)
                        .Where(t => t != null && t.IsActive && !t.IsDeleted)
                        .Select(t => new { t!.FolderName, t.CssFileName, t.Name })
                        .FirstOrDefaultAsync();

                theme = activeTheme is not null
                    ? (activeTheme.FolderName, activeTheme.CssFileName, activeTheme.Name)
                    : ("Modern", "modern.css", "Modern");

                _cache.Set(cacheKey, theme, TimeSpan.FromMinutes(30));
            }

            context.Items["CurrentThemeFolder"] = theme.Folder;
            context.Items["CurrentThemeCss"] = theme.Css;
            context.Items["CurrentThemeName"] = theme.Name;
        }

        await _next(context);
    }
}
