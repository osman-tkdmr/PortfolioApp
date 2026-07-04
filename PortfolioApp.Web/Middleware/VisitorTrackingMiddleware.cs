using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.DataAccess.Context;

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
            var ipAddress = AnonymizeIp(context.Connection.RemoteIpAddress);
            var referrer = context.Request.Headers.Referer.ToString();
            var sessionId = context.Session.Id;
            var username = context.GetRouteValue("username")?.ToString();

            // Fire-and-forget with its own scope so it gets an independent DbContext
            _ = Task.Run(async () =>
            {
                using var scope = _scopeFactory.CreateScope();
                try
                {
                    if (string.IsNullOrEmpty(username)) return;

                    var db = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
                    var ownerId = await db.Users.Where(u => u.Handle == username).Select(u => u.Id).FirstOrDefaultAsync();
                    if (ownerId is null) return;

                    var svc = scope.ServiceProvider.GetRequiredService<IVisitorLogService>();
                    await svc.LogVisitAsync(ownerId, ipAddress, userAgent, path, referrer, sessionId, isBot);
                }
                catch { /* best-effort logging, never fail the request */ }
            });
        }

        await _next(context);
    }

    // Analytics only need approximate origin, not a stored PII-grade identifier: mask the
    // last IPv4 octet / last 80 bits of IPv6 before anything reaches the database.
    private static string? AnonymizeIp(IPAddress? address)
    {
        if (address is null) return null;

        var bytes = address.GetAddressBytes();
        if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
        {
            bytes[3] = 0;
        }
        else
        {
            for (var i = 6; i < bytes.Length; i++)
                bytes[i] = 0;
        }

        return new IPAddress(bytes).ToString();
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
