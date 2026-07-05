using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.DTO.ViewModels.Public;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.Web.Controllers;

public class BlogController : Controller
{
    private readonly IBlogService _blogService;
    private readonly ISeoService _seoService;
    private readonly UserManager<ApplicationUser> _userManager;

    public BlogController(IBlogService blogService, ISeoService seoService, UserManager<ApplicationUser> userManager)
    {
        _blogService = blogService;
        _seoService = seoService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string username, int page = 1, string? category = null, string? tag = null, string? q = null)
    {
        var owner = await _userManager.Users.FirstOrDefaultAsync(u => u.Handle == username && u.IsActive);
        if (owner is null) return NotFound();

        var pagedResult = await _blogService.GetPagedAsync(owner.Id, page, 9, category, tag, q);
        var categoriesResult = await _blogService.GetCategoriesAsync(owner.Id);
        var seoResult = await _seoService.GetByPageSlugAsync(owner.Id, "blog");

        var vm = new BlogListViewModel
        {
            Posts = pagedResult.Data ?? new(),
            Categories = categoriesResult.Data ?? [],
            CurrentCategory = category,
            CurrentTag = tag,
            SearchQuery = q,
            Seo = seoResult.Data,
            ActiveThemeFolder = HttpContext.Items["CurrentThemeFolder"]?.ToString() ?? "Modern"
        };

        return View(vm);
    }

    public async Task<IActionResult> Detail(string username, string slug)
    {
        var owner = await _userManager.Users.FirstOrDefaultAsync(u => u.Handle == username && u.IsActive);
        if (owner is null) return NotFound();

        var postResult = await _blogService.GetBySlugAsync(owner.Id, slug);
        if (!postResult.Success) return NotFound();

        await _blogService.IncrementViewCountAsync(postResult.Data!.Id);

        var vm = new BlogDetailViewModel
        {
            Post = postResult.Data,
            ActiveThemeFolder = HttpContext.Items["CurrentThemeFolder"]?.ToString() ?? "Modern"
        };

        return View(vm);
    }
}
