using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using PortfolioApp.Core.Interfaces;

namespace PortfolioApp.Business.Security;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public string? UserId => User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public string RequireUserId() =>
        UserId ?? throw new InvalidOperationException("No authenticated user is available in the current context.");

    public bool IsInRole(string role) => User?.IsInRole(role) ?? false;
}
