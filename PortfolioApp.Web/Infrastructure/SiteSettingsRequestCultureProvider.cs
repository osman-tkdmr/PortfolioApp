using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.DataAccess.Context;

namespace PortfolioApp.Web.Infrastructure;

public class SiteSettingsRequestCultureProvider : IRequestCultureProvider
{
    public async Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
    {
        var path = httpContext.Request.Path.Value ?? "";
        if (path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var username = httpContext.GetRouteValue("username")?.ToString();
        if (string.IsNullOrEmpty(username))
        {
            return null;
        }

        var db = httpContext.RequestServices.GetRequiredService<PortfolioDbContext>();
        var ownerId = await db.Users.Where(u => u.Handle == username).Select(u => u.Id).FirstOrDefaultAsync();
        if (ownerId is null)
        {
            return null;
        }

        var siteSettingsService = httpContext.RequestServices.GetRequiredService<ISiteSettingsService>();
        var settings = await siteSettingsService.GetAsync(ownerId);

        var culture = settings.Success && settings.Data?.Language == "en" ? "en-US" : "tr-TR";
        return new ProviderCultureResult(culture, culture);
    }
}
