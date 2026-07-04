using Microsoft.AspNetCore.Mvc;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.DTO.DTOs.Site;

namespace PortfolioApp.Web.Areas.Admin.Controllers;

public class ContactController : AdminBaseController
{
    private readonly IContactService _contactService;

    public ContactController(IContactService contactService)
    {
        _contactService = contactService;
    }

    // ── Messages inbox ─────────────────────────────────────────────────────────

    public async Task<IActionResult> Index()
    {
        var result = await _contactService.GetMessagesAsync();
        return View(result.Data);
    }

    public async Task<IActionResult> Detail(int id)
    {
        var result = await _contactService.GetMessageByIdAsync(id);
        if (!result.Success) { Error(result.Message); return RedirectToAction(nameof(Index)); }

        await _contactService.MarkAsReadAsync(id);
        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _contactService.DeleteMessageAsync(id);
        return JsonOk(result.Message);
    }

    [HttpPost]
    public async Task<IActionResult> MarkSpam(int id)
    {
        var result = await _contactService.MarkAsSpamAsync(id);
        return JsonOk(result.Message);
    }

    // ── Contact Info ───────────────────────────────────────────────────────────

    public async Task<IActionResult> Info()
    {
        var result = await _contactService.GetContactInfoAsync(CurrentUserId);
        var dto = result.Data is null ? new ContactInfoUpdateDto() : new ContactInfoUpdateDto
        {
            Id = result.Data.Id,
            Email = result.Data.Email,
            Phone = result.Data.Phone,
            Address = result.Data.Address,
            City = result.Data.City,
            Country = result.Data.Country,
            MapEmbedUrl = result.Data.MapEmbedUrl,
            WorkingHours = result.Data.WorkingHours
        };
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateInfo(ContactInfoUpdateDto dto)
    {
        var result = await _contactService.UpdateContactInfoAsync(dto);
        if (result.Success) Success(result.Message); else Error(result.Message);
        return RedirectToAction(nameof(Info));
    }
}
