using Microsoft.AspNetCore.Mvc;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.DTO.ViewModels.Public;

namespace PortfolioApp.Web.Controllers;

public class BlogController : Controller
{
    private readonly IBlogService _blogService;
    private readonly ISeoService _seoService;

    public BlogController(IBlogService blogService, ISeoService seoService)
    {
        _blogService = blogService;
        _seoService = seoService;
    }

    public async Task<IActionResult> Index(int page = 1, string? category = null, string? tag = null, string? q = null)
    {
        var pagedResult = await _blogService.GetPagedAsync(page, 9, category, tag, q);
        var categoriesResult = await _blogService.GetCategoriesAsync();
        var seoResult = await _seoService.GetByPageSlugAsync("blog");

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

    public async Task<IActionResult> Detail(string slug)
    {
        var postResult = await _blogService.GetBySlugAsync(slug);
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
