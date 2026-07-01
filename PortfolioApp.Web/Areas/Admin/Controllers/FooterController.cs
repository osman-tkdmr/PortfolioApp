using Microsoft.AspNetCore.Mvc;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.DTO.DTOs.Site;

namespace PortfolioApp.Web.Areas.Admin.Controllers;

public class FooterController : AdminBaseController
{
    private readonly IFooterService _footerService;

    public FooterController(IFooterService footerService)
    {
        _footerService = footerService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _footerService.GetAllActiveAsync();
        return View(result.Data);
    }

    [HttpGet]
    public IActionResult Create() => View(new FooterContentCreateDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FooterContentCreateDto dto)
    {
        var result = await _footerService.CreateAsync(dto);
        if (result.Success) { Success(result.Message); return RedirectToAction(nameof(Index)); }
        Error(result.Message);
        return View(dto);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _footerService.GetByIdAsync(id);
        if (!result.Success) { Error(result.Message); return RedirectToAction(nameof(Index)); }
        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, FooterContentUpdateDto dto)
    {
        var result = await _footerService.UpdateAsync(id, dto);
        if (result.Success) { Success(result.Message); return RedirectToAction(nameof(Index)); }
        Error(result.Message);
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _footerService.DeleteAsync(id);
        return JsonOk(result.Message);
    }
}
