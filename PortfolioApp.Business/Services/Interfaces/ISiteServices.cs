using PortfolioApp.Core.Results;
using PortfolioApp.DTO.DTOs.Dashboard;
using PortfolioApp.DTO.DTOs.Portfolio;
using PortfolioApp.DTO.DTOs.Site;

namespace PortfolioApp.Business.Services.Interfaces;

public interface ISiteSettingsService
{
    Task<IDataResult<SiteSettingsDto>> GetAsync();
    Task<IResult> UpdateAsync(SiteSettingsUpdateDto dto);
}

public interface IThemeService
{
    Task<IDataResult<ThemeDto?>> GetActiveThemeAsync();
    Task<IDataResult<IList<ThemeDto>>> GetAllAsync();
    Task<IResult> ActivateThemeAsync(int themeId);
}

public interface IHeroService
{
    Task<IDataResult<HeroSectionDto?>> GetActiveAsync();
    Task<IResult> UpdateAsync(HeroSectionUpdateDto dto);
}

public interface IAboutService
{
    Task<IDataResult<AboutDto?>> GetActiveAsync();
    Task<IResult> UpdateAsync(AboutUpdateDto dto);
}

public interface IExperienceService
{
    Task<IDataResult<IList<ExperienceDto>>> GetAllActiveAsync();
    Task<IDataResult<ExperienceDto>> GetByIdAsync(int id);
    Task<IResult> CreateAsync(ExperienceCreateDto dto);
    Task<IResult> UpdateAsync(int id, ExperienceUpdateDto dto);
    Task<IResult> DeleteAsync(int id);
}

public interface IEducationService
{
    Task<IDataResult<IList<EducationDto>>> GetAllActiveAsync();
    Task<IDataResult<EducationDto>> GetByIdAsync(int id);
    Task<IResult> CreateAsync(EducationCreateDto dto);
    Task<IResult> UpdateAsync(int id, EducationUpdateDto dto);
    Task<IResult> DeleteAsync(int id);
}

public interface ICertificateService
{
    Task<IDataResult<IList<CertificateDto>>> GetAllActiveAsync();
    Task<IDataResult<CertificateDto>> GetByIdAsync(int id);
    Task<IResult> CreateAsync(CertificateCreateDto dto);
    Task<IResult> UpdateAsync(int id, CertificateUpdateDto dto);
    Task<IResult> DeleteAsync(int id);
}

public interface ISkillService
{
    Task<IDataResult<IList<SkillCategoryDto>>> GetCategoriesWithSkillsAsync();
    Task<IDataResult<IList<SkillDto>>> GetAllSkillsAsync();
    Task<IDataResult<SkillDto>> GetSkillByIdAsync(int id);
    Task<IResult> CreateSkillAsync(SkillCreateDto dto);
    Task<IResult> UpdateSkillAsync(int id, SkillUpdateDto dto);
    Task<IResult> DeleteSkillAsync(int id);
    Task<IDataResult<IList<SkillCategoryDto>>> GetAllCategoriesAsync();
    Task<IResult> CreateCategoryAsync(SkillCategoryCreateDto dto);
    Task<IResult> UpdateCategoryAsync(int id, SkillCategoryUpdateDto dto);
    Task<IResult> DeleteCategoryAsync(int id);
}

public interface ILanguageService
{
    Task<IDataResult<IList<LanguageDto>>> GetAllActiveAsync();
    Task<IDataResult<LanguageDto>> GetByIdAsync(int id);
    Task<IResult> CreateAsync(LanguageCreateDto dto);
    Task<IResult> UpdateAsync(int id, LanguageUpdateDto dto);
    Task<IResult> DeleteAsync(int id);
}

public interface IAchievementService
{
    Task<IDataResult<IList<AchievementDto>>> GetAllActiveAsync();
    Task<IDataResult<AchievementDto>> GetByIdAsync(int id);
    Task<IResult> CreateAsync(AchievementCreateDto dto);
    Task<IResult> UpdateAsync(int id, AchievementUpdateDto dto);
    Task<IResult> DeleteAsync(int id);
}

public interface ITestimonialService
{
    Task<IDataResult<IList<TestimonialDto>>> GetAllActiveAsync();
    Task<IDataResult<IList<TestimonialDto>>> GetApprovedAsync();
    Task<IDataResult<TestimonialDto>> GetByIdAsync(int id);
    Task<IResult> CreateAsync(TestimonialCreateDto dto);
    Task<IResult> UpdateAsync(int id, TestimonialUpdateDto dto);
    Task<IResult> DeleteAsync(int id);
}

public interface IContactService
{
    Task<IDataResult<ContactInfoDto?>> GetContactInfoAsync();
    Task<IResult> UpdateContactInfoAsync(ContactInfoUpdateDto dto);
    Task<IDataResult<IList<ContactMessageDto>>> GetMessagesAsync();
    Task<IDataResult<ContactMessageDto>> GetMessageByIdAsync(int id);
    Task<IResult> SendMessageAsync(ContactMessageCreateDto dto);
    Task<IResult> MarkAsReadAsync(int id);
    Task<IResult> ReplyAsync(int id, string replyText);
    Task<IResult> DeleteMessageAsync(int id);
    Task<IResult> MarkAsSpamAsync(int id);
}

public interface ISocialMediaService
{
    Task<IDataResult<IList<SocialMediaDto>>> GetAllActiveAsync();
    Task<IDataResult<SocialMediaDto>> GetByIdAsync(int id);
    Task<IResult> CreateAsync(SocialMediaCreateDto dto);
    Task<IResult> UpdateAsync(int id, SocialMediaUpdateDto dto);
    Task<IResult> DeleteAsync(int id);
}

public interface IMenuService
{
    Task<IDataResult<IList<MenuItemDto>>> GetHeaderMenuAsync();
    Task<IDataResult<IList<MenuItemDto>>> GetAllAsync();
    Task<IDataResult<MenuItemDto>> GetByIdAsync(int id);
    Task<IResult> CreateAsync(MenuItemCreateDto dto);
    Task<IResult> UpdateAsync(int id, MenuItemUpdateDto dto);
    Task<IResult> DeleteAsync(int id);
}

public interface IFooterService
{
    Task<IDataResult<IList<FooterContentDto>>> GetAllActiveAsync();
    Task<IDataResult<FooterContentDto>> GetByIdAsync(int id);
    Task<IResult> CreateAsync(FooterContentCreateDto dto);
    Task<IResult> UpdateAsync(int id, FooterContentUpdateDto dto);
    Task<IResult> DeleteAsync(int id);
}

public interface ISeoService
{
    Task<IDataResult<SeoSettingsDto?>> GetByPageSlugAsync(string slug);
    Task<IDataResult<IList<SeoSettingsDto>>> GetAllAsync();
    Task<IResult> UpdateAsync(SeoSettingsUpdateDto dto);
}

public interface IDashboardService
{
    Task<IDataResult<DashboardStatsDto>> GetStatsAsync();
}

public interface IVisitorLogService
{
    Task LogVisitAsync(string? ipAddress, string? userAgent, string? pageUrl, string? referrer, string? sessionId, bool isBot);
    Task<IDataResult<IList<VisitorChartDataDto>>> GetLast30DaysAsync();
    Task<int> GetTodayCountAsync();
    Task<int> GetTotalCountAsync();
}
