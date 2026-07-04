using Microsoft.AspNetCore.Mvc;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.DTO.DTOs.Portfolio;
using PortfolioApp.Web.Infrastructure;

namespace PortfolioApp.Web.Areas.Admin.Controllers;

public class AboutController : AdminBaseController
{
    private readonly IAboutService _aboutService;
    private readonly IFileUploadService _fileUpload;

    public AboutController(IAboutService aboutService, IFileUploadService fileUpload)
    {
        _aboutService = aboutService;
        _fileUpload = fileUpload;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _aboutService.GetActiveAsync(CurrentUserId);
        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(AboutUpdateDto dto, IFormFile? photo)
    {
        if (photo is not null)
            dto.ProfileImageUrl = await _fileUpload.UploadImageAsync(photo, "about");

        var result = await _aboutService.UpdateAsync(dto);
        if (result.Success) Success(result.Message); else Error(result.Message);
        return RedirectToAction(nameof(Index));
    }
}
