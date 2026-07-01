using Microsoft.AspNetCore.Mvc;
using PortfolioApp.Business.Services.Interfaces;

namespace PortfolioApp.Web.Areas.Admin.Controllers;

public class AnalyticsController : AdminBaseController
{
    private readonly IVisitorLogService _visitorLogService;

    public AnalyticsController(IVisitorLogService visitorLogService)
    {
        _visitorLogService = visitorLogService;
    }

    public async Task<IActionResult> Index()
    {
        var chartData = await _visitorLogService.GetLast30DaysAsync();
        var todayCount = await _visitorLogService.GetTodayCountAsync();
        var totalCount = await _visitorLogService.GetTotalCountAsync();

        ViewBag.TodayCount = todayCount;
        ViewBag.TotalCount = totalCount;

        return View(chartData.Data);
    }
}
