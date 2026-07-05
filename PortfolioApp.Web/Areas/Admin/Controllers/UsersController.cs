using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PortfolioApp.DTO.ViewModels.Admin;
using PortfolioApp.Entity.Concrete;
using PortfolioApp.Web.Infrastructure;

namespace PortfolioApp.Web.Areas.Admin.Controllers;

[Authorize(Policy = AuthorizationPolicies.RequireSuperAdmin)]
public class UsersController : AdminBaseController
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = _userManager.Users.OrderBy(u => u.CreatedAt).ToList();
        var items = new List<UserListItemViewModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            items.Add(new UserListItemViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Handle = user.Handle,
                Email = user.Email ?? "",
                IsActive = user.IsActive,
                EmailConfirmed = user.EmailConfirmed,
                CreatedAt = user.CreatedAt,
                Roles = roles
            });
        }

        return View(items);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return JsonFail("Kullanıcı bulunamadı.");

        if (user.Id == CurrentUserId)
            return JsonFail("Kendi hesabınızı devre dışı bırakamazsınız.");

        user.IsActive = !user.IsActive;
        await _userManager.UpdateAsync(user);
        return JsonOk(user.IsActive ? "Kullanıcı aktif edildi." : "Kullanıcı devre dışı bırakıldı.");
    }
}
