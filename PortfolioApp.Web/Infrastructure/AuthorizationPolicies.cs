namespace PortfolioApp.Web.Infrastructure;

public static class AuthorizationPolicies
{
    /// <summary>Gates the Admin area — any registered tenant can manage their own portfolio.</summary>
    public const string RequireTenantUser = "RequireTenantUser";

    /// <summary>Gates cross-tenant/platform-wide screens (e.g. the user list).</summary>
    public const string RequireSuperAdmin = "RequireSuperAdmin";
}
