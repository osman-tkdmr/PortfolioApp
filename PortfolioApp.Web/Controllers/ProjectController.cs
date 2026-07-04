using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.DTO.ViewModels.Public;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.Web.Controllers;

public class ProjectController : Controller
{
    private readonly IProjectService _projectService;
    private readonly ISeoService _seoService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProjectController(IProjectService projectService, ISeoService seoService, UserManager<ApplicationUser> userManager)
    {
        _projectService = projectService;
        _seoService = seoService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string username, int page = 1, string? category = null)
    {
        var owner = await _userManager.Users.FirstOrDefaultAsync(u => u.Handle == username && u.IsActive);
        if (owner is null) return NotFound();

        var pagedResult = await _projectService.GetPagedAsync(owner.Id, page, 12, category);
        var categoriesResult = await _projectService.GetCategoriesAsync(owner.Id);
        var seoResult = await _seoService.GetByPageSlugAsync(owner.Id, "projects");

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

    public async Task<IActionResult> Detail(string username, string slug)
    {
        var owner = await _userManager.Users.FirstOrDefaultAsync(u => u.Handle == username && u.IsActive);
        if (owner is null) return NotFound();

        var result = await _projectService.GetBySlugAsync(owner.Id, slug);
        if (!result.Success) return NotFound();

        var vm = new ProjectDetailViewModel
        {
            Project = result.Data!,
            ActiveThemeFolder = HttpContext.Items["CurrentThemeFolder"]?.ToString() ?? "Modern"
        };

        return View(vm);
    }
}
