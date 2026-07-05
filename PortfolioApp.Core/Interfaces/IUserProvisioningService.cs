namespace PortfolioApp.Core.Interfaces;

public interface IUserProvisioningService
{
    /// <summary>Creates the default Hero/About/ContactInfo/SiteSettings/SeoSettings rows a new tenant needs. Safe to call more than once — each step is a no-op if the user already has that row.</summary>
    Task ProvisionDefaultsAsync(string userId);
}
