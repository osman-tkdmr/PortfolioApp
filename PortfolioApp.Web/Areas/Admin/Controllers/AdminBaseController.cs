using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioApp.Web.Infrastructure;

namespace PortfolioApp.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = AuthorizationPolicies.RequireTenantUser)]
[AutoValidateAntiforgeryToken]
public abstract class AdminBaseController : Controller
{
    /// <summary>The logged-in tenant's own Id — used to scope reads that are shared with the public site (e.g. Hero/About/Contact info) to "my own data".</summary>
    protected string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    protected void Success(string message) => TempData["Success"] = message;
    protected void Error(string message) => TempData["Error"] = message;

    protected IActionResult JsonOk(string message) =>
        Json(new { success = true, message });

    protected IActionResult JsonFail(string message) =>
        Json(new { success = false, message });
}
