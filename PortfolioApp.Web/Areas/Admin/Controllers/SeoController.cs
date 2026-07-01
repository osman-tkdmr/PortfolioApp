using Microsoft.AspNetCore.Mvc;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.DTO.DTOs.Site;

namespace PortfolioApp.Web.Areas.Admin.Controllers;

public class SeoController : AdminBaseController
{
    private readonly ISeoService _seoService;

    public SeoController(ISeoService seoService)
    {
        _seoService = seoService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _seoService.GetAllAsync();
        return View(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var allResult = await _seoService.GetAllAsync();
        var dto = allResult.Data?.FirstOrDefault(s => s.Id == id);
        if (dto is null) { Error("SEO ayarı bulunamadı."); return RedirectToAction(nameof(Index)); }
        return View(new SeoSettingsUpdateDto
        {
            Id = dto.Id, PageSlug = dto.PageSlug, MetaTitle = dto.MetaTitle,
            MetaDescription = dto.MetaDescription, MetaKeywords = dto.MetaKeywords,
            OgTitle = dto.OgTitle, OgDescription = dto.OgDescription, OgImageUrl = dto.OgImageUrl,
            CanonicalUrl = dto.CanonicalUrl, NoIndex = dto.NoIndex, NoFollow = dto.NoFollow
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SeoSettingsUpdateDto dto)
    {
        var result = await _seoService.UpdateAsync(dto);
        if (result.Success) { Success(result.Message); return RedirectToAction(nameof(Index)); }

        Error(result.Message);
        return View(dto);
    }
}
