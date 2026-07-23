using Microsoft.AspNetCore.Identity;

namespace PortfolioApp.Entity.Concrete;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Title { get; set; }
    public string? Bio { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? CvFileUrl { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>URL-safe public handle used for /u/{Handle} routing — kept distinct from Identity's UserName (the login email) and deliberately not named "Username" to avoid confusion with that existing property.</summary>
    public string Handle { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}".Trim();

    public virtual ICollection<BlogPost> BlogPosts { get; set; } = [];
}
