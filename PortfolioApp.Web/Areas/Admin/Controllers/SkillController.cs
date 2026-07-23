using Microsoft.AspNetCore.Mvc;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.DTO.DTOs.Portfolio;

namespace PortfolioApp.Web.Areas.Admin.Controllers;

public class SkillController : AdminBaseController
{
    private readonly ISkillService _skillService;

    public SkillController(ISkillService skillService)
    {
        _skillService = skillService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _skillService.GetCategoriesWithSkillsAsync(CurrentUserId);
        return View(result.Data);
    }

    // ── Categories ─────────────────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(SkillCategoryCreateDto dto)
    {
        var result = await _skillService.CreateCategoryAsync(dto);
        if (result.Success) Success(result.Message); else Error(result.Message);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateCategory(int id, SkillCategoryUpdateDto dto)
    {
        var result = await _skillService.UpdateCategoryAsync(id, dto);
        if (result.Success) Success(result.Message); else Error(result.Message);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var result = await _skillService.DeleteCategoryAsync(id);
        return result.Success ? JsonOk(result.Message) : JsonFail(result.Message);
    }

    // ── Skills ─────────────────────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSkill(SkillCreateDto dto)
    {
        var result = await _skillService.CreateSkillAsync(dto);
        if (result.Success) Success(result.Message); else Error(result.Message);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateSkill(int id, SkillUpdateDto dto)
    {
        var result = await _skillService.UpdateSkillAsync(id, dto);
        if (result.Success) Success(result.Message); else Error(result.Message);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSkill(int id)
    {
        var result = await _skillService.DeleteSkillAsync(id);
        return result.Success ? JsonOk(result.Message) : JsonFail(result.Message);
    }
}
