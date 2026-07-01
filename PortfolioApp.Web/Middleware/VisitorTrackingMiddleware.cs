using Microsoft.Extensions.DependencyInjection;
using PortfolioApp.Business.Services.Interfaces;

namespace PortfolioApp.Web.Middleware;

public class VisitorTrackingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceScopeFactory _scopeFactory;

    public VisitorTrackingMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
    {
        _next = next;
        _scopeFactory = scopeFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? "";

        // Only track public page requests — skip admin, static files, API calls
        if (!path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase) &&
            !path.StartsWith("/uploads", StringComparison.OrdinalIgnoreCase) &&
            !path.StartsWith("/css", StringComparison.OrdinalIgnoreCase) &&
            !path.StartsWith("/js", StringComparison.OrdinalIgnoreCase) &&
            !path.StartsWith("/lib", StringComparison.OrdinalIgnoreCase) &&
            !path.StartsWith("/favicon", StringComparison.OrdinalIgnoreCase))
        {
            var userAgent = context.Request.Headers.UserAgent.ToString();
            var isBot = IsKnownBot(userAgent);
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var referrer = context.Request.Headers.Referer.ToString();
            var sessionId = context.Session.Id;

            // Fire-and-forget with its own scope so it gets an independent DbContext
            _ = Task.Run(async () =>
            {
                using var scope = _scopeFactory.CreateScope();
                var svc = scope.ServiceProvider.GetRequiredService<IVisitorLogService>();
                try
                {
                    await svc.LogVisitAsync(ipAddress, userAgent, path, referrer, sessionId, isBot);
                }
                catch { /* best-effort logging, never fail the request */ }
            });
        }

        await _next(context);
    }

    private static bool IsKnownBot(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent)) return true;
        var ua = userAgent.ToLowerInvariant();
        return ua.Contains("bot") || ua.Contains("crawler") || ua.Contains("spider") ||
               ua.Contains("googlebot") || ua.Contains("bingbot") || ua.Contains("slurp") ||
               ua.Contains("duckduckbot") || ua.Contains("baiduspider") || ua.Contains("yandexbot") ||
               ua.Contains("sogou") || ua.Contains("exabot") || ua.Contains("facebot") ||
               ua.Contains("ia_archiver") || ua.Contains("semrushbot") || ua.Contains("ahrefsbot");
    }
}
