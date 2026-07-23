using Microsoft.AspNetCore.Mvc;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.DTO.DTOs.Portfolio;
using PortfolioApp.Web.Infrastructure;

namespace PortfolioApp.Web.Areas.Admin.Controllers;

public class EducationController : AdminBaseController
{
    private readonly IEducationService _educationService;
    private readonly IFileUploadService _fileUpload;

    public EducationController(IEducationService educationService, IFileUploadService fileUpload)
    {
        _educationService = educationService;
        _fileUpload = fileUpload;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _educationService.GetAllActiveAsync(CurrentUserId);
        return View(result.Data);
    }

    [HttpGet]
    public IActionResult Create() => View(new EducationCreateDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EducationCreateDto dto, IFormFile? logo)
    {
        var result = await _educationService.CreateAsync(dto);
        if (result.Success) { Success(result.Message); return RedirectToAction(nameof(Index)); }
        Error(result.Message);
        return View(dto);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _educationService.GetByIdAsync(id);
        if (!result.Success) { Error(result.Message); return RedirectToAction(nameof(Index)); }
        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EducationUpdateDto dto, IFormFile? logo)
    {
        var result = await _educationService.UpdateAsync(id, dto);
        if (result.Success) { Success(result.Message); return RedirectToAction(nameof(Index)); }
        Error(result.Message);
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _educationService.DeleteAsync(id);
        return result.Success ? JsonOk(result.Message) : JsonFail(result.Message);
    }
}
