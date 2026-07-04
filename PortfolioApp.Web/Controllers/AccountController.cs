using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using PortfolioApp.Core.Constants;
using PortfolioApp.Core.Interfaces;
using PortfolioApp.DTO.ViewModels.Account;
using PortfolioApp.Entity.Concrete;
using PortfolioApp.Web.Infrastructure;

namespace PortfolioApp.Web.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserProvisioningService _provisioningService;
    private readonly IEmailService _emailService;

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IUserProvisioningService provisioningService,
        IEmailService emailService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _provisioningService = provisioningService;
        _emailService = emailService;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        // Only bounce to the dashboard if the user actually holds the User role —
        // otherwise an authenticated-but-unauthorized cookie traps the user in a
        // Login -> Dashboard -> AccessDenied -> Login loop with no way out.
        if (User.Identity?.IsAuthenticated == true && User.IsInRole(AppConstants.Roles.User))
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting(RateLimitPolicies.Login)]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        ViewData["ReturnUrl"] = model.ReturnUrl;

        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user is null || !user.IsActive)
        {
            ModelState.AddModelError("", "Geçersiz e-posta veya şifre.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }

        if (result.IsLockedOut)
            ModelState.AddModelError("", "Hesabınız geçici olarak kilitlendi. Lütfen daha sonra tekrar deneyin.");
        else
            ModelState.AddModelError("", "Geçersiz e-posta veya şifre.");

        return View(model);
    }

    [HttpGet]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting(RateLimitPolicies.AccountAction)]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (await _userManager.Users.AnyAsync(u => u.Handle == model.Handle))
        {
            ModelState.AddModelError(nameof(model.Handle), "Bu kullanıcı adı zaten kullanılıyor.");
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            Handle = model.Handle,
            FirstName = model.FirstName,
            LastName = model.LastName,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
            return View(model);
        }

        await _userManager.AddToRoleAsync(user, AppConstants.Roles.User);
        await _provisioningService.ProvisionDefaultsAsync(user.Id);

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmUrl = Url.Action(nameof(ConfirmEmail), "Account", new { userId = user.Id, token }, Request.Scheme)!;
        await _emailService.SendAsync(user.Email!, user.FullName, "E-posta Adresinizi Doğrulayın",
            $"""<p>Merhaba {user.FirstName},</p><p>Hesabınızı doğrulamak için <a href="{confirmUrl}">buraya tıklayın</a>.</p>""");

        await _signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            return RedirectToAction(nameof(Login));

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return RedirectToAction(nameof(Login));

        var result = await _userManager.ConfirmEmailAsync(user, token);
        ViewData["Success"] = result.Succeeded;
        return View();
    }

    [HttpGet]
    public IActionResult ForgotPassword() => View(new ForgotPasswordViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting(RateLimitPolicies.AccountAction)]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        // Always show the same confirmation regardless of whether the email exists,
        // so this endpoint can't be used to enumerate registered accounts.
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user is not null && user.IsActive)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetUrl = Url.Action(nameof(ResetPassword), "Account", new { email = user.Email, token }, Request.Scheme)!;
            await _emailService.SendAsync(user.Email!, user.FullName, "Şifre Sıfırlama",
                $"""<p>Merhaba {user.FirstName},</p><p>Şifrenizi sıfırlamak için <a href="{resetUrl}">buraya tıklayın</a>.</p>""");
        }

        return View("ForgotPasswordConfirmation");
    }

    [HttpGet]
    public IActionResult ResetPassword(string email, string token) => View(new ResetPasswordViewModel { Email = email, Token = token });

    [HttpPost]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting(RateLimitPolicies.AccountAction)]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user is null)
            return View("ResetPasswordConfirmation");

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
            return View(model);
        }

        return View("ResetPasswordConfirmation");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    public IActionResult AccessDenied() => View();
}
