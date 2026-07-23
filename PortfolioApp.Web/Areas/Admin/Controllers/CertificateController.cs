using Microsoft.AspNetCore.Mvc;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.DTO.DTOs.Portfolio;
using PortfolioApp.Web.Infrastructure;

namespace PortfolioApp.Web.Areas.Admin.Controllers;

public class CertificateController : AdminBaseController
{
    private readonly ICertificateService _certificateService;
    private readonly IFileUploadService _fileUpload;

    public CertificateController(ICertificateService certificateService, IFileUploadService fileUpload)
    {
        _certificateService = certificateService;
        _fileUpload = fileUpload;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _certificateService.GetAllActiveAsync(CurrentUserId);
        return View(result.Data);
    }

    [HttpGet]
    public IActionResult Create() => View(new CertificateCreateDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CertificateCreateDto dto, IFormFile? image)
    {
        if (image is not null)
            dto.BadgeImageUrl = await _fileUpload.UploadImageAsync(image, "certificates");

        var result = await _certificateService.CreateAsync(dto);
        if (result.Success) { Success(result.Message); return RedirectToAction(nameof(Index)); }
        Error(result.Message);
        return View(dto);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _certificateService.GetByIdAsync(id);
        if (!result.Success) { Error(result.Message); return RedirectToAction(nameof(Index)); }
        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CertificateUpdateDto dto, IFormFile? image)
    {
        if (image is not null)
            dto.BadgeImageUrl = await _fileUpload.UploadImageAsync(image, "certificates");

        var result = await _certificateService.UpdateAsync(id, dto);
        if (result.Success) { Success(result.Message); return RedirectToAction(nameof(Index)); }
        Error(result.Message);
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _certificateService.DeleteAsync(id);
        return result.Success ? JsonOk(result.Message) : JsonFail(result.Message);
    }
}
