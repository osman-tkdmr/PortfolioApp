using PortfolioApp.Core.Enums;

namespace PortfolioApp.DTO.DTOs.Site;

public class SiteSettingsDto
{
    public int Id { get; set; }
    public string SiteName { get; set; } = string.Empty;
    public string? SiteTitle { get; set; }
    public string? SiteDescription { get; set; }
    public string? LogoUrl { get; set; }
    public string? FaviconUrl { get; set; }
    public string? FooterText { get; set; }
    public string? CopyrightText { get; set; }
    public string? GoogleAnalyticsId { get; set; }
    public string Language { get; set; } = "tr";
    public bool IsMaintenanceMode { get; set; }
    public string? MaintenanceMessage { get; set; }
    public int? ActiveThemeId { get; set; }
}

public class SiteSettingsUpdateDto : SiteSettingsDto { }

public class SeoSettingsDto
{
    public int Id { get; set; }
    public string PageSlug { get; set; } = string.Empty;
    public string MetaTitle { get; set; } = string.Empty;
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public string? OgTitle { get; set; }
    public string? OgDescription { get; set; }
    public string? OgImageUrl { get; set; }
    public string? CanonicalUrl { get; set; }
    public bool NoIndex { get; set; }
    public bool NoFollow { get; set; }
}

public class SeoSettingsUpdateDto : SeoSettingsDto { }

public class ThemeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FolderName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? PreviewImageUrl { get; set; }
    public string CssFileName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
}

public class HeroSectionDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? Description { get; set; }
    public string? BackgroundImageUrl { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? CtaButtonText { get; set; }
    public string? CtaButtonUrl { get; set; }
    public string? SecondaryButtonText { get; set; }
    public string? SecondaryButtonUrl { get; set; }
    public string? TypewriterTexts { get; set; }
    public bool ShowTypewriter { get; set; }
    public bool IsActive { get; set; }
}

public class HeroSectionUpdateDto : HeroSectionDto { }

public class ContactInfoDto
{
    public int Id { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? MapEmbedUrl { get; set; }
    public string? WorkingHours { get; set; }
}

public class ContactInfoUpdateDto : ContactInfoDto { }

public class ContactMessageDto
{
    public int Id { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string? SenderPhone { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public bool IsReplied { get; set; }
    public string? ReplyText { get; set; }
    public DateTime? RepliedAt { get; set; }
    public bool IsSpam { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ContactMessageCreateDto
{
    public string SenderName { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string? SenderPhone { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class SocialMediaDto
{
    public int Id { get; set; }
    public string Platform { get; set; } = string.Empty;
    public string? IconClass { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Color { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

public class SocialMediaCreateDto
{
    public string Platform { get; set; } = string.Empty;
    public string? IconClass { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Color { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public class SocialMediaUpdateDto : SocialMediaCreateDto { public int Id { get; set; } }

public class MenuItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? IconClass { get; set; }
    public int? ParentMenuItemId { get; set; }
    public MenuLocation Location { get; set; }
    public string? Target { get; set; }
    public string? CssClass { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public IList<MenuItemDto> Children { get; set; } = [];
}

public class MenuItemCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? IconClass { get; set; }
    public int? ParentMenuItemId { get; set; }
    public MenuLocation Location { get; set; } = MenuLocation.Header;
    public string? Target { get; set; }
    public string? CssClass { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public class MenuItemUpdateDto : MenuItemCreateDto { public int Id { get; set; } }

public class FooterContentDto
{
    public int Id { get; set; }
    public string SectionTitle { get; set; } = string.Empty;
    public string? Content { get; set; }
    public FooterSectionType SectionType { get; set; }
    public int ColumnPosition { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

public class FooterContentCreateDto
{
    public string SectionTitle { get; set; } = string.Empty;
    public string? Content { get; set; }
    public FooterSectionType SectionType { get; set; } = FooterSectionType.Text;
    public int ColumnPosition { get; set; } = 1;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public class FooterContentUpdateDto : FooterContentCreateDto { public int Id { get; set; } }

public class TestimonialDto
{
    public int Id { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorTitle { get; set; }
    public string? AuthorCompany { get; set; }
    public string? AuthorImageUrl { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; }
    public bool IsApproved { get; set; }
    public bool IsFeatured { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

public class TestimonialCreateDto
{
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorTitle { get; set; }
    public string? AuthorCompany { get; set; }
    public string? AuthorImageUrl { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; } = 5;
    public bool IsApproved { get; set; } = true;
    public bool IsFeatured { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public class TestimonialUpdateDto : TestimonialCreateDto { public int Id { get; set; } }
