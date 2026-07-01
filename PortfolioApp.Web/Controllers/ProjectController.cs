using Microsoft.AspNetCore.Mvc;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.DTO.ViewModels.Public;

namespace PortfolioApp.Web.Controllers;

public class ProjectController : Controller
{
    private readonly IProjectService _projectService;
    private readonly ISeoService _seoService;

    public ProjectController(IProjectService projectService, ISeoService seoService)
    {
        _projectService = projectService;
        _seoService = seoService;
    }

    public async Task<IActionResult> Index(int page = 1, string? category = null)
    {
        var pagedResult = await _projectService.GetPagedAsync(page, 12, category);
        var categoriesResult = await _projectService.GetCategoriesAsync();
        var seoResult = await _seoService.GetByPageSlugAsync("projects");

        var vm = new ProjectListViewModel
        {
            Projects = pagedResult.Data ?? new(),
            Categories = categoriesResult.Data ?? [],
            CurrentCategory = category,
            Seo = seoResult.Data,
            ActiveThemeFolder = HttpContext.Items["CurrentThemeFolder"]?.ToString() ?? "Modern"
        };

        return View(vm);
    }

    public async Task<IActionResult> Detail(string slug)
    {
        var result = await _projectService.GetBySlugAsync(slug);
        if (!result.Success) return NotFound();

        var vm = new ProjectDetailViewModel
        {
            Project = result.Data!,
            ActiveThemeFolder = HttpContext.Items["CurrentThemeFolder"]?.ToString() ?? "Modern"
        };

        return View(vm);
    }
}
