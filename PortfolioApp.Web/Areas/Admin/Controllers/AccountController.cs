using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioApp.DTO.ViewModels.Account;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.Web.Areas.Admin.Controllers;

public class AccountController : AdminBaseController
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public async Task<IActionResult> Settings()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        return View(new AccountSettingsViewModel
        {
            FirstName = user.FirstName ?? "",
            LastName = user.LastName ?? "",
            Handle = user.Handle,
            Title = user.Title,
            Bio = user.Bio,
            ProfileImageUrl = user.ProfileImageUrl,
            CvFileUrl = user.CvFileUrl
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Settings(AccountSettingsViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        if (await _userManager.Users.AnyAsync(u => u.Handle == model.Handle && u.Id != user.Id))
        {
            ModelState.AddModelError(nameof(model.Handle), "Bu kullanıcı adı zaten kullanılıyor.");
            return View(model);
        }

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Handle = model.Handle;
        user.Title = model.Title;
        user.Bio = model.Bio;
        user.ProfileImageUrl = model.ProfileImageUrl;
        user.CvFileUrl = model.CvFileUrl;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
            return View(model);
        }

        Success("Profil bilgileriniz güncellendi.");
        return RedirectToAction(nameof(Settings));
    }

    [HttpGet]
    public IActionResult ChangePassword() => View(new ChangePasswordViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
            return View(model);
        }

        await _signInManager.RefreshSignInAsync(user);
        Success("Şifreniz güncellendi.");
        return RedirectToAction(nameof(ChangePassword));
    }
}
