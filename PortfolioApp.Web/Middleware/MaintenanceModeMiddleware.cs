using Microsoft.EntityFrameworkCore;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.DataAccess.Context;

namespace PortfolioApp.Web.Middleware;

public class MaintenanceModeMiddleware
{
    private readonly RequestDelegate _next;

    public MaintenanceModeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ISiteSettingsService siteSettingsService, PortfolioDbContext db)
    {
        var path = context.Request.Path.Value ?? "";

        // Always allow admin access and static files through
        if (path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/uploads", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/css", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/js", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/lib", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var username = context.GetRouteValue("username")?.ToString();
        if (string.IsNullOrEmpty(username))
        {
            await _next(context);
            return;
        }

        var ownerId = await db.Users.Where(u => u.Handle == username).Select(u => u.Id).FirstOrDefaultAsync();
        if (ownerId is null)
        {
            await _next(context);
            return;
        }

        // A tenant previewing their own (possibly maintenance-mode) site should never be locked out of it.
        var isOwnerViewingOwnSite = context.User.Identity?.IsAuthenticated == true &&
            context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value == ownerId;

        var settings = await siteSettingsService.GetAsync(ownerId);
        if (settings.Success && settings.Data?.IsMaintenanceMode == true && !isOwnerViewingOwnSite)
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.WriteAsync(@"<!DOCTYPE html>
<html lang='tr'>
<head><meta charset='utf-8'><title>Bakım Modu</title>
<style>body{font-family:sans-serif;display:flex;align-items:center;justify-content:center;min-height:100vh;margin:0;background:#f5f5f5}
.box{text-align:center;padding:3rem;background:#fff;border-radius:12px;box-shadow:0 4px 24px rgba(0,0,0,.1)}
h1{font-size:2rem;color:#333}p{color:#666}</style></head>
<body><div class='box'>
<h1>&#x1F6E0; Bakım Modu</h1>
<p>Site şu anda bakım nedeniyle geçici olarak kapalı.<br>Lütfen daha sonra tekrar ziyaret edin.</p>
</div></body></html>");
            return;
        }

        await _next(context);
    }
}
