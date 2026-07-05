using PortfolioApp.Core.Entities;

namespace PortfolioApp.Entity.Concrete;

public class Certificate : BaseEntity, IUserOwnedEntity
{
    public string UserId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public string? IssuingOrganization { get; set; }
    public string? CredentialId { get; set; }
    public string? CredentialUrl { get; set; }
    public string? BadgeImageUrl { get; set; }
    public DateOnly IssuedDate { get; set; }
    public DateOnly? ExpirationDate { get; set; }
    public bool HasExpiration { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
