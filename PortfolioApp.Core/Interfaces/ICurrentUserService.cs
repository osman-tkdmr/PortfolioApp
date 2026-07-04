namespace PortfolioApp.Core.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    bool IsAuthenticated { get; }

    /// <summary>Returns the current user's Id, throwing if there is no authenticated user — used as a fail-fast guard in admin-side service methods.</summary>
    string RequireUserId();

    bool IsInRole(string role);
}
