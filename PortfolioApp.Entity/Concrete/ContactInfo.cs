using PortfolioApp.Core.Entities;

namespace PortfolioApp.Entity.Concrete;

public class ContactInfo : BaseEntity
{
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? MapEmbedUrl { get; set; }
    public string? WorkingHours { get; set; }
    public bool IsActive { get; set; } = true;
}
