using Microsoft.AspNetCore.Localization;
using PortfolioApp.Business.Services.Interfaces;

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

        var siteSettingsService = httpContext.RequestServices.GetRequiredService<ISiteSettingsService>();
        var settings = await siteSettingsService.GetAsync();

        var culture = settings.Success && settings.Data?.Language == "en" ? "en-US" : "tr-TR";
        return new ProviderCultureResult(culture, culture);
    }
}
