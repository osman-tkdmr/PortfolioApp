using PortfolioApp.Core.Enums;

namespace PortfolioApp.DTO.DTOs.Portfolio;

public class AboutDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public string? CvFileUrl { get; set; }
    public int YearsOfExperience { get; set; }
    public int ProjectsCompleted { get; set; }
    public int ClientsSatisfied { get; set; }
    public bool ShowStatistics { get; set; }
    public bool IsActive { get; set; }
}

public class AboutUpdateDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public string? CvFileUrl { get; set; }
    public int YearsOfExperience { get; set; }
    public int ProjectsCompleted { get; set; }
    public int ClientsSatisfied { get; set; }
    public bool ShowStatistics { get; set; }
    public bool IsActive { get; set; }
}

public class ExperienceDto
{
    public int Id { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? CompanyLogoUrl { get; set; }
    public string? Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

public class ExperienceCreateDto
{
    public string JobTitle { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? CompanyLogoUrl { get; set; }
    public string? Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public class ExperienceUpdateDto : ExperienceCreateDto { public int Id { get; set; } }

public class EducationDto
{
    public int Id { get; set; }
    public string Degree { get; set; } = string.Empty;
    public string School { get; set; } = string.Empty;
    public string? FieldOfStudy { get; set; }
    public string? Location { get; set; }
    public string? Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public decimal? GPA { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

public class EducationCreateDto
{
    public string Degree { get; set; } = string.Empty;
    public string School { get; set; } = string.Empty;
    public string? FieldOfStudy { get; set; }
    public string? Location { get; set; }
    public string? Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public decimal? GPA { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public class EducationUpdateDto : EducationCreateDto { public int Id { get; set; } }

public class CertificateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? IssuingOrganization { get; set; }
    public string? CredentialId { get; set; }
    public string? CredentialUrl { get; set; }
    public string? BadgeImageUrl { get; set; }
    public DateOnly IssuedDate { get; set; }
    public DateOnly? ExpirationDate { get; set; }
    public bool HasExpiration { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

public class CertificateCreateDto
{
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

public class CertificateUpdateDto : CertificateCreateDto { public int Id { get; set; } }

public class SkillCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? IconClass { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public IList<SkillDto> Skills { get; set; } = [];
}

public class SkillCategoryCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? IconClass { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public class SkillCategoryUpdateDto : SkillCategoryCreateDto { public int Id { get; set; } }

public class SkillDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SkillCategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int Percentage { get; set; }
    public string? IconClass { get; set; }
    public SkillLevel Level { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

public class SkillCreateDto
{
    public string Name { get; set; } = string.Empty;
    public int SkillCategoryId { get; set; }
    public int Percentage { get; set; }
    public string? IconClass { get; set; }
    public SkillLevel Level { get; set; } = SkillLevel.Intermediate;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public class SkillUpdateDto : SkillCreateDto { public int Id { get; set; } }

public class LanguageDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? FlagEmoji { get; set; }
    public LanguageLevel Level { get; set; }
    public int Percentage { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

public class LanguageCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? FlagEmoji { get; set; }
    public LanguageLevel Level { get; set; } = LanguageLevel.Intermediate;
    public int Percentage { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public class LanguageUpdateDto : LanguageCreateDto { public int Id { get; set; } }

public class AchievementDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconClass { get; set; }
    public string? ImageUrl { get; set; }
    public DateOnly? AchievedDate { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

public class AchievementCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconClass { get; set; }
    public string? ImageUrl { get; set; }
    public DateOnly? AchievedDate { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public class AchievementUpdateDto : AchievementCreateDto { public int Id { get; set; } }
