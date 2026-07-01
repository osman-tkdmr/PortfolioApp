using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.DTO.DTOs.Blog;
using PortfolioApp.Entity.Concrete;
using PortfolioApp.Web.Infrastructure;

namespace PortfolioApp.Web.Areas.Admin.Controllers;

public class BlogController : AdminBaseController
{
    private readonly IBlogService _blogService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IFileUploadService _fileUpload;

    public BlogController(IBlogService blogService, UserManager<ApplicationUser> userManager, IFileUploadService fileUpload)
    {
        _blogService = blogService;
        _userManager = userManager;
        _fileUpload = fileUpload;
    }

    // ── Posts ──────────────────────────────────────────────────────────────────

    public async Task<IActionResult> Index()
    {
        var result = await _blogService.GetAllAsync();
        return View(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadViewBagAsync();
        return View(new BlogPostCreateDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BlogPostCreateDto dto, IFormFile? coverImage)
    {
        if (coverImage is not null)
            dto.CoverImageUrl = await _fileUpload.UploadImageAsync(coverImage, "blog");

        var authorId = _userManager.GetUserId(User)!;
        var result = await _blogService.CreateAsync(dto, authorId);

        if (result.Success) { Success(result.Message); return RedirectToAction(nameof(Index)); }

        Error(result.Message);
        await LoadViewBagAsync();
        return View(dto);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _blogService.GetByIdAsync(id);
        if (!result.Success) { Error(result.Message); return RedirectToAction(nameof(Index)); }

        var dto = new BlogPostUpdateDto
        {
            Title = result.Data!.Title,
            Summary = result.Data.Summary,
            Content = result.Data.Content,
            CoverImageUrl = result.Data.CoverImageUrl,
            BlogCategoryId = result.Data.BlogCategoryId,
            TagIds = result.Data.Tags?.Select(t => t.Id).ToList() ?? [],
            IsPublished = result.Data.IsPublished,
            IsFeatured = result.Data.IsFeatured
        };

        ViewData["PostId"] = id;
        await LoadViewBagAsync();
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BlogPostUpdateDto dto, IFormFile? coverImage)
    {
        if (coverImage is not null)
            dto.CoverImageUrl = await _fileUpload.UploadImageAsync(coverImage, "blog");

        var result = await _blogService.UpdateAsync(id, dto);

        if (result.Success) { Success(result.Message); return RedirectToAction(nameof(Index)); }

        Error(result.Message);
        ViewData["PostId"] = id;
        await LoadViewBagAsync();
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _blogService.DeleteAsync(id);
        return JsonOk(result.Message);
    }

    [HttpPost]
    public async Task<IActionResult> Publish(int id)
    {
        var result = await _blogService.PublishAsync(id);
        return JsonOk(result.Message);
    }

    [HttpPost]
    public async Task<IActionResult> Unpublish(int id)
    {
        var result = await _blogService.UnpublishAsync(id);
        return JsonOk(result.Message);
    }

    // ── Categories ─────────────────────────────────────────────────────────────

    public async Task<IActionResult> Categories()
    {
        var result = await _blogService.GetCategoriesAsync();
        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(BlogCategoryCreateDto dto)
    {
        var result = await _blogService.CreateCategoryAsync(dto);
        if (result.Success) Success(result.Message); else Error(result.Message);
        return RedirectToAction(nameof(Categories));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateCategory(int id, BlogCategoryUpdateDto dto)
    {
        var result = await _blogService.UpdateCategoryAsync(id, dto);
        if (result.Success) Success(result.Message); else Error(result.Message);
        return RedirectToAction(nameof(Categories));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var result = await _blogService.DeleteCategoryAsync(id);
        return JsonOk(result.Message);
    }

    // ── Tags ───────────────────────────────────────────────────────────────────

    public async Task<IActionResult> Tags()
    {
        var result = await _blogService.GetTagsAsync();
        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTag(BlogTagCreateDto dto)
    {
        var result = await _blogService.CreateTagAsync(dto);
        if (result.Success) Success(result.Message); else Error(result.Message);
        return RedirectToAction(nameof(Tags));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteTag(int id)
    {
        var result = await _blogService.DeleteTagAsync(id);
        return JsonOk(result.Message);
    }

    private async Task LoadViewBagAsync()
    {
        var cats = await _blogService.GetCategoriesAsync();
        var tags = await _blogService.GetTagsAsync();
        ViewBag.Categories = cats.Data ?? [];
        ViewBag.Tags = tags.Data ?? [];
    }
}
