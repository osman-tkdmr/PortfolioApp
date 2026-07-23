using Microsoft.AspNetCore.Mvc;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.DTO.DTOs.Site;

namespace PortfolioApp.Web.Areas.Admin.Controllers;

public class MenuController : AdminBaseController
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _menuService.GetAllAsync();
        return View(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.AllItems = (await _menuService.GetAllAsync()).Data ?? [];
        return View(new MenuItemCreateDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MenuItemCreateDto dto)
    {
        var result = await _menuService.CreateAsync(dto);
        if (result.Success) { Success(result.Message); return RedirectToAction(nameof(Index)); }
        Error(result.Message);
        ViewBag.AllItems = (await _menuService.GetAllAsync()).Data ?? [];
        return View(dto);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _menuService.GetByIdAsync(id);
        if (!result.Success) { Error(result.Message); return RedirectToAction(nameof(Index)); }
        ViewBag.AllItems = (await _menuService.GetAllAsync()).Data ?? [];
        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MenuItemUpdateDto dto)
    {
        var result = await _menuService.UpdateAsync(id, dto);
        if (result.Success) { Success(result.Message); return RedirectToAction(nameof(Index)); }
        Error(result.Message);
        ViewBag.AllItems = (await _menuService.GetAllAsync()).Data ?? [];
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _menuService.DeleteAsync(id);
        return result.Success ? JsonOk(result.Message) : JsonFail(result.Message);
    }
}
