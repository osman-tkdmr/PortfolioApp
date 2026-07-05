using Microsoft.AspNetCore.Mvc;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.DTO.DTOs.Portfolio;

namespace PortfolioApp.Web.Areas.Admin.Controllers;

public class AchievementController : AdminBaseController
{
    private readonly IAchievementService _achievementService;

    public AchievementController(IAchievementService achievementService)
    {
        _achievementService = achievementService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _achievementService.GetAllActiveAsync(CurrentUserId);
        return View(result.Data);
    }

    [HttpGet]
    public IActionResult Create() => View(new AchievementCreateDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AchievementCreateDto dto)
    {
        var result = await _achievementService.CreateAsync(dto);
        if (result.Success) { Success(result.Message); return RedirectToAction(nameof(Index)); }
        Error(result.Message);
        return View(dto);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _achievementService.GetByIdAsync(id);
        if (!result.Success) { Error(result.Message); return RedirectToAction(nameof(Index)); }
        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AchievementUpdateDto dto)
    {
        var result = await _achievementService.UpdateAsync(id, dto);
        if (result.Success) { Success(result.Message); return RedirectToAction(nameof(Index)); }
        Error(result.Message);
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _achievementService.DeleteAsync(id);
        return JsonOk(result.Message);
    }
}
