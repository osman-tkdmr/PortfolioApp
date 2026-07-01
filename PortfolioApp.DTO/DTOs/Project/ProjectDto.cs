namespace PortfolioApp.DTO.DTOs.Project;

public class ProjectDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? DemoUrl { get; set; }
    public string? SourceUrl { get; set; }
    public int ProjectCategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
    public DateOnly? CompletedDate { get; set; }
    public string? ClientName { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public IList<ProjectImageDto> Images { get; set; } = [];
    public IList<TechnologyDto> Technologies { get; set; } = [];
}

public class ProjectImageDto
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public bool IsCover { get; set; }
    public int DisplayOrder { get; set; }
}

public class ProjectCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? DemoUrl { get; set; }
    public string? SourceUrl { get; set; }
    public int ProjectCategoryId { get; set; }
    public bool IsFeatured { get; set; }
    public DateOnly? CompletedDate { get; set; }
    public string? ClientName { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public IList<int> TechnologyIds { get; set; } = [];
}

public class ProjectUpdateDto : ProjectCreateDto
{
    public int Id { get; set; }
}
