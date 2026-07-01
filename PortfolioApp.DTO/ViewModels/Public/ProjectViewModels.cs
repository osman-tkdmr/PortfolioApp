using PortfolioApp.Core.Results;
using PortfolioApp.DTO.DTOs.Project;
using PortfolioApp.DTO.DTOs.Site;

namespace PortfolioApp.DTO.ViewModels.Public;

public class ProjectListViewModel
{
    public PaginatedResult<ProjectDto> Projects { get; set; } = new();
    public IList<ProjectCategoryDto> Categories { get; set; } = [];
    public string? CurrentCategory { get; set; }
    public SeoSettingsDto? Seo { get; set; }
    public string ActiveThemeFolder { get; set; } = "Modern";
    public string ActiveThemeCss { get; set; } = "modern.css";
}

public class ProjectDetailViewModel
{
    public ProjectDto Project { get; set; } = new();
    public IList<ProjectDto> RelatedProjects { get; set; } = [];
    public SeoSettingsDto? Seo { get; set; }
    public string ActiveThemeFolder { get; set; } = "Modern";
    public string ActiveThemeCss { get; set; } = "modern.css";
}
