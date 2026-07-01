using PortfolioApp.Core.Entities;

namespace PortfolioApp.Entity.Concrete;

public class SocialMedia : BaseEntity
{
    public string Platform { get; set; } = string.Empty;
    public string? IconClass { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Color { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
