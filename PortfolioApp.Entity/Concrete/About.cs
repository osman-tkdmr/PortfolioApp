using PortfolioApp.Core.Entities;

namespace PortfolioApp.Entity.Concrete;

public class About : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public string? CvFileUrl { get; set; }
    public int YearsOfExperience { get; set; }
    public int ProjectsCompleted { get; set; }
    public int ClientsSatisfied { get; set; }
    public bool ShowStatistics { get; set; } = true;
    public bool IsActive { get; set; } = true;
}
