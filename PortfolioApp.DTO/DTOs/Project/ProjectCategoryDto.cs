namespace PortfolioApp.DTO.DTOs.Project;

public class ProjectCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconClass { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public int ProjectCount { get; set; }
}

public class ProjectCategoryCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconClass { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public class ProjectCategoryUpdateDto : ProjectCategoryCreateDto
{
    public int Id { get; set; }
}
