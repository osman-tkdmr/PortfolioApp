using Microsoft.AspNetCore.Mvc;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.DTO.DTOs.Site;

namespace PortfolioApp.Web.Areas.Admin.Controllers;

public class SocialMediaController : AdminBaseController
{
    private readonly ISocialMediaService _socialMediaService;

    public SocialMediaController(ISocialMediaService socialMediaService)
    {
        _socialMediaService = socialMediaService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _socialMediaService.GetAllActiveAsync(CurrentUserId);
        return View(result.Data);
    }

    [HttpGet]
    public IActionResult Create() => View(new SocialMediaCreateDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SocialMediaCreateDto dto)
    {
        var result = await _socialMediaService.CreateAsync(dto);
        if (result.Success) { Success(result.Message); return RedirectToAction(nameof(Index)); }
        Error(result.Message);
        return View(dto);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _socialMediaService.GetByIdAsync(id);
        if (!result.Success) { Error(result.Message); return RedirectToAction(nameof(Index)); }
        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SocialMediaUpdateDto dto)
    {
        var result = await _socialMediaService.UpdateAsync(id, dto);
        if (result.Success) { Success(result.Message); return RedirectToAction(nameof(Index)); }
        Error(result.Message);
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _socialMediaService.DeleteAsync(id);
        return result.Success ? JsonOk(result.Message) : JsonFail(result.Message);
    }

}
