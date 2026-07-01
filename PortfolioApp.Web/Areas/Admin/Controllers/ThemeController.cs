using Microsoft.AspNetCore.Mvc;
using PortfolioApp.Business.Services.Interfaces;

namespace PortfolioApp.Web.Areas.Admin.Controllers;

public class ThemeController : AdminBaseController
{
    private readonly IThemeService _themeService;

    public ThemeController(IThemeService themeService)
    {
        _themeService = themeService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _themeService.GetAllAsync();
        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Activate(int id)
    {
        var result = await _themeService.ActivateThemeAsync(id);
        if (result.Success) Success(result.Message); else Error(result.Message);
        return RedirectToAction(nameof(Index));
    }
}
