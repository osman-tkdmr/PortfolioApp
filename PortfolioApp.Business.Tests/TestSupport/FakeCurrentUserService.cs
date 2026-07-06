using PortfolioApp.Core.Interfaces;

namespace PortfolioApp.Business.Tests.TestSupport;

public class FakeCurrentUserService : ICurrentUserService
{
    public string? UserId { get; set; }
    public bool IsAuthenticated => UserId is not null;
    public string RequireUserId() => UserId ?? throw new InvalidOperationException("No authenticated user is available in the current context.");
    public bool IsInRole(string role) => false;
}
