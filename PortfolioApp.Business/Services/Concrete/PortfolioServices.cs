using AutoMapper;
using PortfolioApp.Business.Security;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.Core.Results;
using PortfolioApp.DataAccess.UnitOfWork;
using PortfolioApp.DTO.DTOs.Portfolio;
using PortfolioApp.DTO.DTOs.Site;
using PortfolioApp.Entity.Concrete;
using Microsoft.EntityFrameworkCore;

namespace PortfolioApp.Business.Services.Concrete;

public class HeroService : IHeroService
{
    private readonly UnitOfWork _uow;
    private readonly IMapper _mapper;

    public HeroService(UnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

    public async Task<IDataResult<HeroSectionDto?>> GetActiveAsync()
    {
        var hero = await _uow.GetRepository<HeroSection>().FirstOrDefaultAsync(h => h.IsActive);
        return DataResult<HeroSectionDto?>.Ok(_mapper.Map<HeroSectionDto?>(hero));
    }

    public async Task<IResult> UpdateAsync(HeroSectionUpdateDto dto)
    {
        var hero = await _uow.GetRepository<HeroSection>().GetByIdAsync(dto.Id);
        if (hero is null) return Result.Fail("Hero bölümü bulunamadı.");
        _mapper.Map(dto, hero);
        _uow.GetRepository<HeroSection>().Update(hero);
        await _uow.SaveChangesAsync();
        return Result.Ok("Hero bölümü güncellendi.");
    }
}

public class AboutService : IAboutService
{
    private readonly UnitOfWork _uow;
    private readonly IMapper _mapper;

    public AboutService(UnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

    public async Task<IDataResult<AboutDto?>> GetActiveAsync()
    {
        var about = await _uow.GetRepository<About>().FirstOrDefaultAsync(a => a.IsActive);
        return DataResult<AboutDto?>.Ok(_mapper.Map<AboutDto?>(about));
    }

    public async Task<IResult> UpdateAsync(AboutUpdateDto dto)
    {
        var about = await _uow.GetRepository<About>().GetByIdAsync(dto.Id);
        if (about is null) return Result.Fail("Hakkımda bölümü bulunamadı.");
        _mapper.Map(dto, about);
        about.Content = RichTextSanitizer.Sanitize(dto.Content);
        _uow.GetRepository<About>().Update(about);
        await _uow.SaveChangesAsync();
        return Result.Ok("Hakkımda bölümü güncellendi.");
    }
}

public class ExperienceService : IExperienceService
{
    private readonly UnitOfWork _uow;
    private readonly IMapper _mapper;

    public ExperienceService(UnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

    public async Task<IDataResult<IList<ExperienceDto>>> GetAllActiveAsync()
    {
        var items = await _uow.GetRepository<Experience>().FindAsync(e => e.IsActive);
        return DataResult<IList<ExperienceDto>>.Ok(_mapper.Map<IList<ExperienceDto>>(items.OrderBy(e => e.DisplayOrder).ToList()));
    }

    public async Task<IDataResult<ExperienceDto>> GetByIdAsync(int id)
    {
        var item = await _uow.GetRepository<Experience>().GetByIdAsync(id);
        return item is null ? DataResult<ExperienceDto>.Fail("Deneyim bulunamadı.") : DataResult<ExperienceDto>.Ok(_mapper.Map<ExperienceDto>(item));
    }

    public async Task<IResult> CreateAsync(ExperienceCreateDto dto)
    {
        await _uow.GetRepository<Experience>().AddAsync(_mapper.Map<Experience>(dto));
        await _uow.SaveChangesAsync();
        return Result.Ok("Deneyim eklendi.");
    }

    public async Task<IResult> UpdateAsync(int id, ExperienceUpdateDto dto)
    {
        var item = await _uow.GetRepository<Experience>().GetByIdAsync(id);
        if (item is null) return Result.Fail("Deneyim bulunamadı.");
        _mapper.Map(dto, item);
        _uow.GetRepository<Experience>().Update(item);
        await _uow.SaveChangesAsync();
        return Result.Ok("Deneyim güncellendi.");
    }

    public async Task<IResult> DeleteAsync(int id)
    {
        await _uow.GetRepository<Experience>().SoftDeleteAsync(id);
        await _uow.SaveChangesAsync();
        return Result.Ok("Deneyim silindi.");
    }
}

public class EducationService : IEducationService
{
    private readonly UnitOfWork _uow;
    private readonly IMapper _mapper;

    public EducationService(UnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

    public async Task<IDataResult<IList<EducationDto>>> GetAllActiveAsync()
    {
        var items = await _uow.GetRepository<Education>().FindAsync(e => e.IsActive);
        return DataResult<IList<EducationDto>>.Ok(_mapper.Map<IList<EducationDto>>(items.OrderBy(e => e.DisplayOrder).ToList()));
    }

    public async Task<IDataResult<EducationDto>> GetByIdAsync(int id)
    {
        var item = await _uow.GetRepository<Education>().GetByIdAsync(id);
        return item is null ? DataResult<EducationDto>.Fail("Eğitim bulunamadı.") : DataResult<EducationDto>.Ok(_mapper.Map<EducationDto>(item));
    }

    public async Task<IResult> CreateAsync(EducationCreateDto dto)
    {
        await _uow.GetRepository<Education>().AddAsync(_mapper.Map<Education>(dto));
        await _uow.SaveChangesAsync();
        return Result.Ok("Eğitim eklendi.");
    }

    public async Task<IResult> UpdateAsync(int id, EducationUpdateDto dto)
    {
        var item = await _uow.GetRepository<Education>().GetByIdAsync(id);
        if (item is null) return Result.Fail("Eğitim bulunamadı.");
        _mapper.Map(dto, item);
        _uow.GetRepository<Education>().Update(item);
        await _uow.SaveChangesAsync();
        return Result.Ok("Eğitim güncellendi.");
    }

    public async Task<IResult> DeleteAsync(int id)
    {
        await _uow.GetRepository<Education>().SoftDeleteAsync(id);
        await _uow.SaveChangesAsync();
        return Result.Ok("Eğitim silindi.");
    }
}

public class CertificateService : ICertificateService
{
    private readonly UnitOfWork _uow;
    private readonly IMapper _mapper;

    public CertificateService(UnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

    public async Task<IDataResult<IList<CertificateDto>>> GetAllActiveAsync()
    {
        var items = await _uow.GetRepository<Certificate>().FindAsync(c => c.IsActive);
        return DataResult<IList<CertificateDto>>.Ok(_mapper.Map<IList<CertificateDto>>(items.OrderBy(c => c.DisplayOrder).ToList()));
    }

    public async Task<IDataResult<CertificateDto>> GetByIdAsync(int id)
    {
        var item = await _uow.GetRepository<Certificate>().GetByIdAsync(id);
        return item is null ? DataResult<CertificateDto>.Fail("Sertifika bulunamadı.") : DataResult<CertificateDto>.Ok(_mapper.Map<CertificateDto>(item));
    }

    public async Task<IResult> CreateAsync(CertificateCreateDto dto)
    {
        await _uow.GetRepository<Certificate>().AddAsync(_mapper.Map<Certificate>(dto));
        await _uow.SaveChangesAsync();
        return Result.Ok("Sertifika eklendi.");
    }

    public async Task<IResult> UpdateAsync(int id, CertificateUpdateDto dto)
    {
        var item = await _uow.GetRepository<Certificate>().GetByIdAsync(id);
        if (item is null) return Result.Fail("Sertifika bulunamadı.");
        _mapper.Map(dto, item);
        _uow.GetRepository<Certificate>().Update(item);
        await _uow.SaveChangesAsync();
        return Result.Ok("Sertifika güncellendi.");
    }

    public async Task<IResult> DeleteAsync(int id)
    {
        await _uow.GetRepository<Certificate>().SoftDeleteAsync(id);
        await _uow.SaveChangesAsync();
        return Result.Ok("Sertifika silindi.");
    }
}

public class SkillService : ISkillService
{
    private readonly UnitOfWork _uow;
    private readonly IMapper _mapper;

    public SkillService(UnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

    public async Task<IDataResult<IList<SkillCategoryDto>>> GetCategoriesWithSkillsAsync()
    {
        var categories = await _uow.GetRepository<SkillCategory>()
            .GetQueryable()
            .Where(c => c.IsActive && !c.IsDeleted)
            .Include(c => c.Skills.Where(s => s.IsActive && !s.IsDeleted))
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
        return DataResult<IList<SkillCategoryDto>>.Ok(_mapper.Map<IList<SkillCategoryDto>>(categories));
    }

    public async Task<IDataResult<IList<SkillDto>>> GetAllSkillsAsync()
    {
        var skills = await _uow.GetRepository<Skill>().GetAllAsync();
        return DataResult<IList<SkillDto>>.Ok(_mapper.Map<IList<SkillDto>>(skills));
    }

    public async Task<IDataResult<SkillDto>> GetSkillByIdAsync(int id)
    {
        var skill = await _uow.GetRepository<Skill>().GetByIdAsync(id);
        return skill is null ? DataResult<SkillDto>.Fail("Yetenek bulunamadı.") : DataResult<SkillDto>.Ok(_mapper.Map<SkillDto>(skill));
    }

    public async Task<IResult> CreateSkillAsync(SkillCreateDto dto)
    {
        await _uow.GetRepository<Skill>().AddAsync(_mapper.Map<Skill>(dto));
        await _uow.SaveChangesAsync();
        return Result.Ok("Yetenek eklendi.");
    }

    public async Task<IResult> UpdateSkillAsync(int id, SkillUpdateDto dto)
    {
        var skill = await _uow.GetRepository<Skill>().GetByIdAsync(id);
        if (skill is null) return Result.Fail("Yetenek bulunamadı.");
        _mapper.Map(dto, skill);
        _uow.GetRepository<Skill>().Update(skill);
        await _uow.SaveChangesAsync();
        return Result.Ok("Yetenek güncellendi.");
    }

    public async Task<IResult> DeleteSkillAsync(int id)
    {
        await _uow.GetRepository<Skill>().SoftDeleteAsync(id);
        await _uow.SaveChangesAsync();
        return Result.Ok("Yetenek silindi.");
    }

    public async Task<IDataResult<IList<SkillCategoryDto>>> GetAllCategoriesAsync()
    {
        var cats = await _uow.GetRepository<SkillCategory>().GetAllAsync();
        return DataResult<IList<SkillCategoryDto>>.Ok(_mapper.Map<IList<SkillCategoryDto>>(cats));
    }

    public async Task<IResult> CreateCategoryAsync(SkillCategoryCreateDto dto)
    {
        await _uow.GetRepository<SkillCategory>().AddAsync(_mapper.Map<SkillCategory>(dto));
        await _uow.SaveChangesAsync();
        return Result.Ok("Kategori eklendi.");
    }

    public async Task<IResult> UpdateCategoryAsync(int id, SkillCategoryUpdateDto dto)
    {
        var cat = await _uow.GetRepository<SkillCategory>().GetByIdAsync(id);
        if (cat is null) return Result.Fail("Kategori bulunamadı.");
        _mapper.Map(dto, cat);
        _uow.GetRepository<SkillCategory>().Update(cat);
        await _uow.SaveChangesAsync();
        return Result.Ok("Kategori güncellendi.");
    }

    public async Task<IResult> DeleteCategoryAsync(int id)
    {
        await _uow.GetRepository<SkillCategory>().SoftDeleteAsync(id);
        await _uow.SaveChangesAsync();
        return Result.Ok("Kategori silindi.");
    }
}

public class LanguageService : ILanguageService
{
    private readonly UnitOfWork _uow;
    private readonly IMapper _mapper;

    public LanguageService(UnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

    public async Task<IDataResult<IList<LanguageDto>>> GetAllActiveAsync()
    {
        var items = await _uow.GetRepository<Language>().FindAsync(l => l.IsActive);
        return DataResult<IList<LanguageDto>>.Ok(_mapper.Map<IList<LanguageDto>>(items.OrderBy(l => l.DisplayOrder).ToList()));
    }

    public async Task<IDataResult<LanguageDto>> GetByIdAsync(int id)
    {
        var item = await _uow.GetRepository<Language>().GetByIdAsync(id);
        return item is null ? DataResult<LanguageDto>.Fail("Dil bulunamadı.") : DataResult<LanguageDto>.Ok(_mapper.Map<LanguageDto>(item));
    }

    public async Task<IResult> CreateAsync(LanguageCreateDto dto)
    {
        await _uow.GetRepository<Language>().AddAsync(_mapper.Map<Language>(dto));
        await _uow.SaveChangesAsync();
        return Result.Ok("Dil eklendi.");
    }

    public async Task<IResult> UpdateAsync(int id, LanguageUpdateDto dto)
    {
        var item = await _uow.GetRepository<Language>().GetByIdAsync(id);
        if (item is null) return Result.Fail("Dil bulunamadı.");
        _mapper.Map(dto, item);
        _uow.GetRepository<Language>().Update(item);
        await _uow.SaveChangesAsync();
        return Result.Ok("Dil güncellendi.");
    }

    public async Task<IResult> DeleteAsync(int id)
    {
        await _uow.GetRepository<Language>().SoftDeleteAsync(id);
        await _uow.SaveChangesAsync();
        return Result.Ok("Dil silindi.");
    }
}

public class AchievementService : IAchievementService
{
    private readonly UnitOfWork _uow;
    private readonly IMapper _mapper;

    public AchievementService(UnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

    public async Task<IDataResult<IList<AchievementDto>>> GetAllActiveAsync()
    {
        var items = await _uow.GetRepository<Achievement>().FindAsync(a => a.IsActive);
        return DataResult<IList<AchievementDto>>.Ok(_mapper.Map<IList<AchievementDto>>(items.OrderBy(a => a.DisplayOrder).ToList()));
    }

    public async Task<IDataResult<AchievementDto>> GetByIdAsync(int id)
    {
        var item = await _uow.GetRepository<Achievement>().GetByIdAsync(id);
        return item is null ? DataResult<AchievementDto>.Fail("Başarı bulunamadı.") : DataResult<AchievementDto>.Ok(_mapper.Map<AchievementDto>(item));
    }

    public async Task<IResult> CreateAsync(AchievementCreateDto dto)
    {
        await _uow.GetRepository<Achievement>().AddAsync(_mapper.Map<Achievement>(dto));
        await _uow.SaveChangesAsync();
        return Result.Ok("Başarı eklendi.");
    }

    public async Task<IResult> UpdateAsync(int id, AchievementUpdateDto dto)
    {
        var item = await _uow.GetRepository<Achievement>().GetByIdAsync(id);
        if (item is null) return Result.Fail("Başarı bulunamadı.");
        _mapper.Map(dto, item);
        _uow.GetRepository<Achievement>().Update(item);
        await _uow.SaveChangesAsync();
        return Result.Ok("Başarı güncellendi.");
    }

    public async Task<IResult> DeleteAsync(int id)
    {
        await _uow.GetRepository<Achievement>().SoftDeleteAsync(id);
        await _uow.SaveChangesAsync();
        return Result.Ok("Başarı silindi.");
    }
}

public class TestimonialService : ITestimonialService
{
    private readonly UnitOfWork _uow;
    private readonly IMapper _mapper;

    public TestimonialService(UnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

    public async Task<IDataResult<IList<TestimonialDto>>> GetAllActiveAsync()
    {
        var items = await _uow.GetRepository<Testimonial>().FindAsync(t => t.IsActive);
        return DataResult<IList<TestimonialDto>>.Ok(_mapper.Map<IList<TestimonialDto>>(items));
    }

    public async Task<IDataResult<IList<TestimonialDto>>> GetApprovedAsync()
    {
        var items = await _uow.GetRepository<Testimonial>().FindAsync(t => t.IsActive && t.IsApproved);
        return DataResult<IList<TestimonialDto>>.Ok(_mapper.Map<IList<TestimonialDto>>(items.OrderBy(t => t.DisplayOrder).ToList()));
    }

    public async Task<IDataResult<TestimonialDto>> GetByIdAsync(int id)
    {
        var item = await _uow.GetRepository<Testimonial>().GetByIdAsync(id);
        return item is null ? DataResult<TestimonialDto>.Fail("Referans bulunamadı.") : DataResult<TestimonialDto>.Ok(_mapper.Map<TestimonialDto>(item));
    }

    public async Task<IResult> CreateAsync(TestimonialCreateDto dto)
    {
        await _uow.GetRepository<Testimonial>().AddAsync(_mapper.Map<Testimonial>(dto));
        await _uow.SaveChangesAsync();
        return Result.Ok("Referans eklendi.");
    }

    public async Task<IResult> UpdateAsync(int id, TestimonialUpdateDto dto)
    {
        var item = await _uow.GetRepository<Testimonial>().GetByIdAsync(id);
        if (item is null) return Result.Fail("Referans bulunamadı.");
        _mapper.Map(dto, item);
        _uow.GetRepository<Testimonial>().Update(item);
        await _uow.SaveChangesAsync();
        return Result.Ok("Referans güncellendi.");
    }

    public async Task<IResult> DeleteAsync(int id)
    {
        await _uow.GetRepository<Testimonial>().SoftDeleteAsync(id);
        await _uow.SaveChangesAsync();
        return Result.Ok("Referans silindi.");
    }
}

public class SocialMediaService : ISocialMediaService
{
    private readonly UnitOfWork _uow;
    private readonly IMapper _mapper;

    public SocialMediaService(UnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

    public async Task<IDataResult<IList<SocialMediaDto>>> GetAllActiveAsync()
    {
        var items = await _uow.GetRepository<SocialMedia>().FindAsync(s => s.IsActive);
        return DataResult<IList<SocialMediaDto>>.Ok(_mapper.Map<IList<SocialMediaDto>>(items.OrderBy(s => s.DisplayOrder).ToList()));
    }

    public async Task<IDataResult<SocialMediaDto>> GetByIdAsync(int id)
    {
        var item = await _uow.GetRepository<SocialMedia>().GetByIdAsync(id);
        return item is null ? DataResult<SocialMediaDto>.Fail("Sosyal medya bulunamadı.") : DataResult<SocialMediaDto>.Ok(_mapper.Map<SocialMediaDto>(item));
    }

    public async Task<IResult> CreateAsync(SocialMediaCreateDto dto)
    {
        await _uow.GetRepository<SocialMedia>().AddAsync(_mapper.Map<SocialMedia>(dto));
        await _uow.SaveChangesAsync();
        return Result.Ok("Sosyal medya eklendi.");
    }

    public async Task<IResult> UpdateAsync(int id, SocialMediaUpdateDto dto)
    {
        var item = await _uow.GetRepository<SocialMedia>().GetByIdAsync(id);
        if (item is null) return Result.Fail("Sosyal medya bulunamadı.");
        _mapper.Map(dto, item);
        _uow.GetRepository<SocialMedia>().Update(item);
        await _uow.SaveChangesAsync();
        return Result.Ok("Sosyal medya güncellendi.");
    }

    public async Task<IResult> DeleteAsync(int id)
    {
        await _uow.GetRepository<SocialMedia>().SoftDeleteAsync(id);
        await _uow.SaveChangesAsync();
        return Result.Ok("Sosyal medya silindi.");
    }
}

public class MenuService : IMenuService
{
    private readonly UnitOfWork _uow;
    private readonly IMapper _mapper;

    public MenuService(UnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

    public async Task<IDataResult<IList<MenuItemDto>>> GetHeaderMenuAsync()
    {
        var items = await _uow.GetRepository<MenuItem>()
            .GetQueryable()
            .Where(m => m.IsActive && !m.IsDeleted && m.Location == Core.Enums.MenuLocation.Header && m.ParentMenuItemId == null)
            .Include(m => m.Children.Where(c => c.IsActive && !c.IsDeleted))
            .OrderBy(m => m.DisplayOrder)
            .ToListAsync();
        return DataResult<IList<MenuItemDto>>.Ok(_mapper.Map<IList<MenuItemDto>>(items));
    }

    public async Task<IDataResult<IList<MenuItemDto>>> GetAllAsync()
    {
        var items = await _uow.GetRepository<MenuItem>().GetAllAsync();
        return DataResult<IList<MenuItemDto>>.Ok(_mapper.Map<IList<MenuItemDto>>(items));
    }

    public async Task<IDataResult<MenuItemDto>> GetByIdAsync(int id)
    {
        var item = await _uow.GetRepository<MenuItem>().GetByIdAsync(id);
        return item is null ? DataResult<MenuItemDto>.Fail("Menü öğesi bulunamadı.") : DataResult<MenuItemDto>.Ok(_mapper.Map<MenuItemDto>(item));
    }

    public async Task<IResult> CreateAsync(MenuItemCreateDto dto)
    {
        await _uow.GetRepository<MenuItem>().AddAsync(_mapper.Map<MenuItem>(dto));
        await _uow.SaveChangesAsync();
        return Result.Ok("Menü öğesi eklendi.");
    }

    public async Task<IResult> UpdateAsync(int id, MenuItemUpdateDto dto)
    {
        var item = await _uow.GetRepository<MenuItem>().GetByIdAsync(id);
        if (item is null) return Result.Fail("Menü öğesi bulunamadı.");
        _mapper.Map(dto, item);
        _uow.GetRepository<MenuItem>().Update(item);
        await _uow.SaveChangesAsync();
        return Result.Ok("Menü öğesi güncellendi.");
    }

    public async Task<IResult> DeleteAsync(int id)
    {
        await _uow.GetRepository<MenuItem>().SoftDeleteAsync(id);
        await _uow.SaveChangesAsync();
        return Result.Ok("Menü öğesi silindi.");
    }
}

public class FooterService : IFooterService
{
    private readonly UnitOfWork _uow;
    private readonly IMapper _mapper;

    public FooterService(UnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

    public async Task<IDataResult<IList<FooterContentDto>>> GetAllActiveAsync()
    {
        var items = await _uow.GetRepository<FooterContent>().FindAsync(f => f.IsActive);
        return DataResult<IList<FooterContentDto>>.Ok(_mapper.Map<IList<FooterContentDto>>(items.OrderBy(f => f.DisplayOrder).ToList()));
    }

    public async Task<IDataResult<FooterContentDto>> GetByIdAsync(int id)
    {
        var item = await _uow.GetRepository<FooterContent>().GetByIdAsync(id);
        return item is null ? DataResult<FooterContentDto>.Fail("Footer içeriği bulunamadı.") : DataResult<FooterContentDto>.Ok(_mapper.Map<FooterContentDto>(item));
    }

    public async Task<IResult> CreateAsync(FooterContentCreateDto dto)
    {
        await _uow.GetRepository<FooterContent>().AddAsync(_mapper.Map<FooterContent>(dto));
        await _uow.SaveChangesAsync();
        return Result.Ok("Footer içeriği eklendi.");
    }

    public async Task<IResult> UpdateAsync(int id, FooterContentUpdateDto dto)
    {
        var item = await _uow.GetRepository<FooterContent>().GetByIdAsync(id);
        if (item is null) return Result.Fail("Footer içeriği bulunamadı.");
        _mapper.Map(dto, item);
        _uow.GetRepository<FooterContent>().Update(item);
        await _uow.SaveChangesAsync();
        return Result.Ok("Footer içeriği güncellendi.");
    }

    public async Task<IResult> DeleteAsync(int id)
    {
        await _uow.GetRepository<FooterContent>().SoftDeleteAsync(id);
        await _uow.SaveChangesAsync();
        return Result.Ok("Footer içeriği silindi.");
    }
}

public class SeoService : ISeoService
{
    private readonly UnitOfWork _uow;
    private readonly IMapper _mapper;

    public SeoService(UnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

    public async Task<IDataResult<SeoSettingsDto?>> GetByPageSlugAsync(string slug)
    {
        var seo = await _uow.GetRepository<SeoSettings>().FirstOrDefaultAsync(s => s.PageSlug == slug);
        return DataResult<SeoSettingsDto?>.Ok(_mapper.Map<SeoSettingsDto?>(seo));
    }

    public async Task<IDataResult<IList<SeoSettingsDto>>> GetAllAsync()
    {
        var items = await _uow.GetRepository<SeoSettings>().GetAllAsync();
        return DataResult<IList<SeoSettingsDto>>.Ok(_mapper.Map<IList<SeoSettingsDto>>(items));
    }

    public async Task<IResult> UpdateAsync(SeoSettingsUpdateDto dto)
    {
        var seo = await _uow.GetRepository<SeoSettings>().GetByIdAsync(dto.Id);
        if (seo is null) return Result.Fail("SEO ayarı bulunamadı.");
        _mapper.Map(dto, seo);
        _uow.GetRepository<SeoSettings>().Update(seo);
        await _uow.SaveChangesAsync();
        return Result.Ok("SEO ayarları güncellendi.");
    }
}
