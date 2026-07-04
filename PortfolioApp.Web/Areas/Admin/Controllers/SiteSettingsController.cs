using Microsoft.AspNetCore.Mvc;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.DTO.DTOs.Site;

namespace PortfolioApp.Web.Areas.Admin.Controllers;

public class SiteSettingsController : AdminBaseController
{
    private readonly ISiteSettingsService _siteSettingsService;

    public SiteSettingsController(ISiteSettingsService siteSettingsService)
    {
        _siteSettingsService = siteSettingsService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _siteSettingsService.GetAsync(CurrentUserId);
        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(SiteSettingsUpdateDto dto)
    {
        var result = await _siteSettingsService.UpdateAsync(dto);
        if (result.Success) Success(result.Message); else Error(result.Message);
        return RedirectToAction(nameof(Index));
    }
}
