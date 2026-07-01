using Microsoft.AspNetCore.Mvc;
using PortfolioApp.Business.Services.Interfaces;

namespace PortfolioApp.Web.Areas.Admin.Controllers;

public class DashboardController : AdminBaseController
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _dashboardService.GetStatsAsync();
        return View(result.Data);
    }
}
