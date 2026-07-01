using Microsoft.AspNetCore.Mvc;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.DTO.DTOs.Site;
using PortfolioApp.Web.Infrastructure;

namespace PortfolioApp.Web.Areas.Admin.Controllers;

public class TestimonialController : AdminBaseController
{
    private readonly ITestimonialService _testimonialService;
    private readonly IFileUploadService _fileUpload;

    public TestimonialController(ITestimonialService testimonialService, IFileUploadService fileUpload)
    {
        _testimonialService = testimonialService;
        _fileUpload = fileUpload;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _testimonialService.GetAllActiveAsync();
        return View(result.Data);
    }

    [HttpGet]
    public IActionResult Create() => View(new TestimonialCreateDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TestimonialCreateDto dto, IFormFile? avatar)
    {
        if (avatar is not null)
            dto.AuthorImageUrl = await _fileUpload.UploadImageAsync(avatar, "testimonials", 400);

        var result = await _testimonialService.CreateAsync(dto);
        if (result.Success) { Success(result.Message); return RedirectToAction(nameof(Index)); }
        Error(result.Message);
        return View(dto);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _testimonialService.GetByIdAsync(id);
        if (!result.Success) { Error(result.Message); return RedirectToAction(nameof(Index)); }
        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TestimonialUpdateDto dto, IFormFile? avatar)
    {
        if (avatar is not null)
            dto.AuthorImageUrl = await _fileUpload.UploadImageAsync(avatar, "testimonials", 400);

        var result = await _testimonialService.UpdateAsync(id, dto);
        if (result.Success) { Success(result.Message); return RedirectToAction(nameof(Index)); }
        Error(result.Message);
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _testimonialService.DeleteAsync(id);
        return JsonOk(result.Message);
    }

}
