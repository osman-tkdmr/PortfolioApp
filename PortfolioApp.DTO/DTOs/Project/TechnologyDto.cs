namespace PortfolioApp.DTO.DTOs.Project;

public class TechnologyDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    public string? IconClass { get; set; }
    public string? Color { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

public class TechnologyCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    public string? IconClass { get; set; }
    public string? Color { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public class TechnologyUpdateDto : TechnologyCreateDto
{
    public int Id { get; set; }
}
