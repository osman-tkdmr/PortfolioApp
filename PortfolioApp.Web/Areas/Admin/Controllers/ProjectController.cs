using Microsoft.AspNetCore.Mvc;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.DTO.DTOs.Project;
using PortfolioApp.Web.Infrastructure;

namespace PortfolioApp.Web.Areas.Admin.Controllers;

public class ProjectController : AdminBaseController
{
    private readonly IProjectService _projectService;
    private readonly IFileUploadService _fileUpload;

    public ProjectController(IProjectService projectService, IFileUploadService fileUpload)
    {
        _projectService = projectService;
        _fileUpload = fileUpload;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _projectService.GetAllAsync();
        return View(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadViewBagAsync();
        return View(new ProjectCreateDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProjectCreateDto dto, IFormFile? coverImage)
    {
        if (coverImage is not null)
            dto.CoverImageUrl = await _fileUpload.UploadImageAsync(coverImage, "projects");

        var result = await _projectService.CreateAsync(dto);
        if (result.Success) { Success(result.Message); return RedirectToAction(nameof(Index)); }

        Error(result.Message);
        await LoadViewBagAsync();
        return View(dto);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _projectService.GetByIdAsync(id);
        if (!result.Success) { Error(result.Message); return RedirectToAction(nameof(Index)); }

        var dto = new ProjectUpdateDto
        {
            Title = result.Data!.Title,
            ShortDescription = result.Data.ShortDescription,
            Description = result.Data.Description,
            CoverImageUrl = result.Data.CoverImageUrl,
            DemoUrl = result.Data.DemoUrl,
            SourceUrl = result.Data.SourceUrl,
            ProjectCategoryId = result.Data.ProjectCategoryId,
            TechnologyIds = result.Data.Technologies?.Select(t => t.Id).ToList() ?? [],
            IsActive = result.Data.IsActive,
            IsFeatured = result.Data.IsFeatured,
            DisplayOrder = result.Data.DisplayOrder
        };

        ViewData["ProjectId"] = id;
        await LoadViewBagAsync();
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProjectUpdateDto dto, IFormFile? coverImage)
    {
        if (coverImage is not null)
            dto.CoverImageUrl = await _fileUpload.UploadImageAsync(coverImage, "projects");

        var result = await _projectService.UpdateAsync(id, dto);
        if (result.Success) { Success(result.Message); return RedirectToAction(nameof(Index)); }

        Error(result.Message);
        ViewData["ProjectId"] = id;
        await LoadViewBagAsync();
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _projectService.DeleteAsync(id);
        return JsonOk(result.Message);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleFeatured(int id)
    {
        var result = await _projectService.ToggleFeaturedAsync(id);
        return JsonOk(result.Message);
    }

    // ── Categories ─────────────────────────────────────────────────────────────

    public async Task<IActionResult> Categories()
    {
        var result = await _projectService.GetCategoriesAsync();
        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(ProjectCategoryCreateDto dto)
    {
        var result = await _projectService.CreateCategoryAsync(dto);
        if (result.Success) Success(result.Message); else Error(result.Message);
        return RedirectToAction(nameof(Categories));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateCategory(int id, ProjectCategoryUpdateDto dto)
    {
        var result = await _projectService.UpdateCategoryAsync(id, dto);
        if (result.Success) Success(result.Message); else Error(result.Message);
        return RedirectToAction(nameof(Categories));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var result = await _projectService.DeleteCategoryAsync(id);
        return JsonOk(result.Message);
    }

    // ── Technologies ───────────────────────────────────────────────────────────

    public async Task<IActionResult> Technologies()
    {
        var result = await _projectService.GetTechnologiesAsync();
        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTechnology(TechnologyCreateDto dto)
    {
        var result = await _projectService.CreateTechnologyAsync(dto);
        if (result.Success) Success(result.Message); else Error(result.Message);
        return RedirectToAction(nameof(Technologies));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateTechnology(int id, TechnologyUpdateDto dto)
    {
        var result = await _projectService.UpdateTechnologyAsync(id, dto);
        if (result.Success) Success(result.Message); else Error(result.Message);
        return RedirectToAction(nameof(Technologies));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteTechnology(int id)
    {
        var result = await _projectService.DeleteTechnologyAsync(id);
        return JsonOk(result.Message);
    }

    private async Task LoadViewBagAsync()
    {
        var cats = await _projectService.GetCategoriesAsync();
        var techs = await _projectService.GetTechnologiesAsync();
        ViewBag.Categories = cats.Data ?? [];
        ViewBag.Technologies = techs.Data ?? [];
    }
}
