using PortfolioApp.Core.Results;
using PortfolioApp.DTO.DTOs.Blog;
using PortfolioApp.DTO.DTOs.Site;

namespace PortfolioApp.DTO.ViewModels.Public;

public class BlogListViewModel
{
    public PaginatedResult<BlogPostDto> Posts { get; set; } = new();
    public IList<BlogCategoryDto> Categories { get; set; } = [];
    public IList<BlogTagDto> Tags { get; set; } = [];
    public string? CurrentCategory { get; set; }
    public string? CurrentTag { get; set; }
    public string? SearchQuery { get; set; }
    public SeoSettingsDto? Seo { get; set; }
    public string ActiveThemeFolder { get; set; } = "Modern";
    public string ActiveThemeCss { get; set; } = "modern.css";
}

public class BlogDetailViewModel
{
    public BlogPostDto Post { get; set; } = new();
    public IList<BlogPostDto> RelatedPosts { get; set; } = [];
    public IList<BlogCategoryDto> Categories { get; set; } = [];
    public SeoSettingsDto? Seo { get; set; }
    public string ActiveThemeFolder { get; set; } = "Modern";
    public string ActiveThemeCss { get; set; } = "modern.css";
}
