using Microsoft.AspNetCore.Mvc;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.DTO.DTOs.Site;
using PortfolioApp.Web.Infrastructure;

namespace PortfolioApp.Web.Areas.Admin.Controllers;

public class HeroController : AdminBaseController
{
    private readonly IHeroService _heroService;
    private readonly IFileUploadService _fileUpload;

    public HeroController(IHeroService heroService, IFileUploadService fileUpload)
    {
        _heroService = heroService;
        _fileUpload = fileUpload;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _heroService.GetActiveAsync();
        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(HeroSectionUpdateDto dto, IFormFile? profileImage, IFormFile? backgroundImage)
    {
        if (profileImage is not null)
            dto.ProfileImageUrl = await _fileUpload.UploadImageAsync(profileImage, "hero");
        if (backgroundImage is not null)
            dto.BackgroundImageUrl = await _fileUpload.UploadImageAsync(backgroundImage, "hero");

        var result = await _heroService.UpdateAsync(dto);
        if (result.Success) Success(result.Message); else Error(result.Message);
        return RedirectToAction(nameof(Index));
    }
}
