using AutoMapper;
using PortfolioApp.Business.Security;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.Core.Interfaces;
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
    private readonly ICurrentUserService _currentUser;

    public HeroService(UnitOfWork uow, IMapper mapper, ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<IDataResult<HeroSectionDto?>> GetActiveAsync(string ownerId)
    {
        var hero = await _uow.GetRepository<HeroSection>().FirstOrDefaultAsync(h => h.UserId == ownerId && h.IsActive);
        return DataResult<HeroSectionDto?>.Ok(_mapper.Map<HeroSectionDto?>(hero));
    }

    public async Task<IResult> UpdateAsync(HeroSectionUpdateDto dto)
    {
        var hero = await _uow.GetRepository<HeroSection>().FirstOrDefaultAsync(h => h.Id == dto.Id && h.UserId == _currentUser.UserId);
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
    private readonly ICurrentUserService _currentUser;

    public AboutService(UnitOfWork uow, IMapper mapper, ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<IDataResult<AboutDto?>> GetActiveAsync(string ownerId)
    {
        var about = await _uow.GetRepository<About>().FirstOrDefaultAsync(a => a.UserId == ownerId && a.IsActive);
        return DataResult<AboutDto?>.Ok(_mapper.Map<AboutDto?>(about));
    }

    public async Task<IResult> UpdateAsync(AboutUpdateDto dto)
    {
        var about = await _uow.GetRepository<About>().FirstOrDefaultAsync(a => a.Id == dto.Id && a.UserId == _currentUser.UserId);
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
    private readonly ICurrentUserService _currentUser;

    public ExperienceService(UnitOfWork uow, IMapper mapper, ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<IDataResult<IList<ExperienceDto>>> GetAllActiveAsync(string ownerId)
    {
        var items = await _uow.GetRepository<Experience>().FindAsync(e => e.UserId == ownerId && e.IsActive);
        return DataResult<IList<ExperienceDto>>.Ok(_mapper.Map<IList<ExperienceDto>>(items.OrderBy(e => e.DisplayOrder).ToList()));
    }

    public async Task<IDataResult<ExperienceDto>> GetByIdAsync(int id)
    {
        var item = await _uow.GetRepository<Experience>().FirstOrDefaultAsync(e => e.Id == id && e.UserId == _currentUser.UserId);
        return item is null ? DataResult<ExperienceDto>.Fail("Deneyim bulunamadı.") : DataResult<ExperienceDto>.Ok(_mapper.Map<ExperienceDto>(item));
    }

    public async Task<IResult> CreateAsync(ExperienceCreateDto dto)
    {
        var entity = _mapper.Map<Experience>(dto);
        entity.UserId = _currentUser.RequireUserId();
        await _uow.GetRepository<Experience>().AddAsync(entity);
        await _uow.SaveChangesAsync();
        return Result.Ok("Deneyim eklendi.");
    }

    public async Task<IResult> UpdateAsync(int id, ExperienceUpdateDto dto)
    {
        var item = await _uow.GetRepository<Experience>().FirstOrDefaultAsync(e => e.Id == id && e.UserId == _currentUser.UserId);
        if (item is null) return Result.Fail("Deneyim bulunamadı.");
        _mapper.Map(dto, item);
        _uow.GetRepository<Experience>().Update(item);
        await _uow.SaveChangesAsync();
        return Result.Ok("Deneyim güncellendi.");
    }

    public async Task<IResult> DeleteAsync(int id)
    {
        var item = await _uow.GetRepository<Experience>().FirstOrDefaultAsync(e => e.Id == id && e.UserId == _currentUser.UserId);
        if (item is null) return Result.Fail("Deneyim bulunamadı.");
        await _uow.GetRepository<Experience>().SoftDeleteAsync(id);
        await _uow.SaveChangesAsync();
        return Result.Ok("Deneyim silindi.");
    }
}

public class EducationService : IEducationService
{
    private readonly UnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public EducationService(UnitOfWork uow, IMapper mapper, ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<IDataResult<IList<EducationDto>>> GetAllActiveAsync(string ownerId)
    {
        var items = await _uow.GetRepository<Education>().FindAsync(e => e.UserId == ownerId && e.IsActive);
        return DataResult<IList<EducationDto>>.Ok(_mapper.Map<IList<EducationDto>>(items.OrderBy(e => e.DisplayOrder).ToList()));
    }

    public async Task<IDataResult<EducationDto>> GetByIdAsync(int id)
    {
        var item = await _uow.GetRepository<Education>().FirstOrDefaultAsync(e => e.Id == id && e.UserId == _currentUser.UserId);
        return item is null ? DataResult<EducationDto>.Fail("Eğitim bulunamadı.") : DataResult<EducationDto>.Ok(_mapper.Map<EducationDto>(item));
    }

    public async Task<IResult> CreateAsync(EducationCreateDto dto)
    {
        var entity = _mapper.Map<Education>(dto);
        entity.UserId = _currentUser.RequireUserId();
        await _uow.GetRepository<Education>().AddAsync(entity);
        await _uow.SaveChangesAsync();
        return Result.Ok("Eğitim eklendi.");
    }

    public async Task<IResult> UpdateAsync(int id, EducationUpdateDto dto)
    {
        var item = await _uow.GetRepository<Education>().FirstOrDefaultAsync(e => e.Id == id && e.UserId == _currentUser.UserId);
        if (item is null) return Result.Fail("Eğitim bulunamadı.");
        _mapper.Map(dto, item);
        _uow.GetRepository<Education>().Update(item);
        await _uow.SaveChangesAsync();
        return Result.Ok("Eğitim güncellendi.");
    }

    public async Task<IResult> DeleteAsync(int id)
    {
        var item = await _uow.GetRepository<Education>().FirstOrDefaultAsync(e => e.Id == id && e.UserId == _currentUser.UserId);
        if (item is null) return Result.Fail("Eğitim bulunamadı.");
        await _uow.GetRepository<Education>().SoftDeleteAsync(id);
        await _uow.SaveChangesAsync();
        return Result.Ok("Eğitim silindi.");
    }
}

public class CertificateService : ICertificateService
{
    private readonly UnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public CertificateService(UnitOfWork uow, IMapper mapper, ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<IDataResult<IList<CertificateDto>>> GetAllActiveAsync(string ownerId)
    {
        var items = await _uow.GetRepository<Certificate>().FindAsync(c => c.UserId == ownerId && c.IsActive);
        return DataResult<IList<CertificateDto>>.Ok(_mapper.Map<IList<CertificateDto>>(items.OrderBy(c => c.DisplayOrder).ToList()));
    }

    public async Task<IDataResult<CertificateDto>> GetByIdAsync(int id)
    {
        var item = await _uow.GetRepository<Certificate>().FirstOrDefaultAsync(c => c.Id == id && c.UserId == _currentUser.UserId);
        return item is null ? DataResult<CertificateDto>.Fail("Sertifika bulunamadı.") : DataResult<CertificateDto>.Ok(_mapper.Map<CertificateDto>(item));
    }

    public async Task<IResult> CreateAsync(CertificateCreateDto dto)
    {
        var entity = _mapper.Map<Certificate>(dto);
        entity.UserId = _currentUser.RequireUserId();
        await _uow.GetRepository<Certificate>().AddAsync(entity);
        await _uow.SaveChangesAsync();
        return Result.Ok("Sertifika eklendi.");
    }

    public async Task<IResult> UpdateAsync(int id, CertificateUpdateDto dto)
    {
        var item = await _uow.GetRepository<Certificate>().FirstOrDefaultAsync(c => c.Id == id && c.UserId == _currentUser.UserId);
        if (item is null) return Result.Fail("Sertifika bulunamadı.");
        _mapper.Map(dto, item);
        _uow.GetRepository<Certificate>().Update(item);
        await _uow.SaveChangesAsync();
        return Result.Ok("Sertifika güncellendi.");
    }

    public async Task<IResult> DeleteAsync(int id)
    {
        var item = await _uow.GetRepository<Certificate>().FirstOrDefaultAsync(c => c.Id == id && c.UserId == _currentUser.UserId);
        if (item is null) return Result.Fail("Sertifika bulunamadı.");
        await _uow.GetRepository<Certificate>().SoftDeleteAsync(id);
        await _uow.SaveChangesAsync();
        return Result.Ok("Sertifika silindi.");
    }
}

public class SkillService : ISkillService
{
    private readonly UnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public SkillService(UnitOfWork uow, IMapper mapper, ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<IDataResult<IList<SkillCategoryDto>>> GetCategoriesWithSkillsAsync(string ownerId)
    {
        var categories = await _uow.GetRepository<SkillCategory>()
            .GetQueryable()
            .Where(c => c.UserId == ownerId && c.IsActive && !c.IsDeleted)
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
        var entity = _mapper.Map<Skill>(dto);
        entity.UserId = _currentUser.RequireUserId();
        await _uow.GetRepository<Skill>().AddAsync(entity);
        await _uow.SaveChangesAsync();
        return Result.Ok("Yetenek eklendi.");
    }

    public async Task<IResult> UpdateSkillAsync(int id, SkillUpdateDto dto)
    {
        var skill = await _uow.GetRepository<Skill>().FirstOrDefaultAsync(s => s.Id == id && s.UserId == _currentUser.UserId);
        if (skill is null) return Result.Fail("Yetenek bulunamadı.");
        _mapper.Map(dto, skill);
        _uow.GetRepository<Skill>().Update(skill);
        await _uow.SaveChangesAsync();
        return Result.Ok("Yetenek güncellendi.");
    }

    public async Task<IResult> DeleteSkillAsync(int id)
    {
        var skill = await _uow.GetRepository<Skill>().FirstOrDefaultAsync(s => s.Id == id && s.UserId == _currentUser.UserId);
        if (skill is null) return Result.Fail("Yetenek bulunamadı.");
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
        var entity = _mapper.Map<SkillCategory>(dto);
        entity.UserId = _currentUser.RequireUserId();
        await _uow.GetRepository<SkillCategory>().AddAsync(entity);
        await _uow.SaveChangesAsync();
        return Result.Ok("Kategori eklendi.");
    }

    public async Task<IResult> UpdateCategoryAsync(int id, SkillCategoryUpdateDto dto)
    {
        var cat = await _uow.GetRepository<SkillCategory>().FirstOrDefaultAsync(c => c.Id == id && c.UserId == _currentUser.UserId);
        if (cat is null) return Result.Fail("Kategori bulunamadı.");
        _mapper.Map(dto, cat);
        _uow.GetRepository<SkillCategory>().Update(cat);
        await _uow.SaveChangesAsync();
        return Result.Ok("Kategori güncellendi.");
    }

    public async Task<IResult> DeleteCategoryAsync(int id)
    {
        var cat = await _uow.GetRepository<SkillCategory>().FirstOrDefaultAsync(c => c.Id == id && c.UserId == _currentUser.UserId);
        if (cat is null) return Result.Fail("Kategori bulunamadı.");
        await _uow.GetRepository<SkillCategory>().SoftDeleteAsync(id);
        await _uow.SaveChangesAsync();
        return Result.Ok("Kategori silindi.");
    }
}

public class LanguageService : ILanguageService
{
    private readonly UnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public LanguageService(UnitOfWork uow, IMapper mapper, ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<IDataResult<IList<LanguageDto>>> GetAllActiveAsync(string ownerId)
    {
        var items = await _uow.GetRepository<Language>().FindAsync(l => l.UserId == ownerId && l.IsActive);
        return DataResult<IList<LanguageDto>>.Ok(_mapper.Map<IList<LanguageDto>>(items.OrderBy(l => l.DisplayOrder).ToList()));
    }

    public async Task<IDataResult<LanguageDto>> GetByIdAsync(int id)
    {
        var item = await _uow.GetRepository<Language>().FirstOrDefaultAsync(l => l.Id == id && l.UserId == _currentUser.UserId);
        return item is null ? DataResult<LanguageDto>.Fail("Dil bulunamadı.") : DataResult<LanguageDto>.Ok(_mapper.Map<LanguageDto>(item));
    }

    public async Task<IResult> CreateAsync(LanguageCreateDto dto)
    {
        var entity = _mapper.Map<Language>(dto);
        entity.UserId = _currentUser.RequireUserId();
        await _uow.GetRepository<Language>().AddAsync(entity);
        await _uow.SaveChangesAsync();
        return Result.Ok("Dil eklendi.");
    }

    public async Task<IResult> UpdateAsync(int id, LanguageUpdateDto dto)
    {
        var item = await _uow.GetRepository<Language>().FirstOrDefaultAsync(l => l.Id == id && l.UserId == _currentUser.UserId);
        if (item is null) return Result.Fail("Dil bulunamadı.");
        _mapper.Map(dto, item);
        _uow.GetRepository<Language>().Update(item);
        await _uow.SaveChangesAsync();
        return Result.Ok("Dil güncellendi.");
    }

    public async Task<IResult> DeleteAsync(int id)
    {
        var item = await _uow.GetRepository<Language>().FirstOrDefaultAsync(l => l.Id == id && l.UserId == _currentUser.UserId);
        if (item is null) return Result.Fail("Dil bulunamadı.");
        await _uow.GetRepository<Language>().SoftDeleteAsync(id);
        await _uow.SaveChangesAsync();
        return Result.Ok("Dil silindi.");
    }
}

public class AchievementService : IAchievementService
{
    private readonly UnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public AchievementService(UnitOfWork uow, IMapper mapper, ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<IDataResult<IList<AchievementDto>>> GetAllActiveAsync(string ownerId)
    {
        var items = await _uow.GetRepository<Achievement>().FindAsync(a => a.UserId == ownerId && a.IsActive);
        return DataResult<IList<AchievementDto>>.Ok(_mapper.Map<IList<AchievementDto>>(items.OrderBy(a => a.DisplayOrder).ToList()));
    }

    public async Task<IDataResult<AchievementDto>> GetByIdAsync(int id)
    {
        var item = await _uow.GetRepository<Achievement>().FirstOrDefaultAsync(a => a.Id == id && a.UserId == _currentUser.UserId);
        return item is null ? DataResult<AchievementDto>.Fail("Başarı bulunamadı.") : DataResult<AchievementDto>.Ok(_mapper.Map<AchievementDto>(item));
    }

    public async Task<IResult> CreateAsync(AchievementCreateDto dto)
    {
        var entity = _mapper.Map<Achievement>(dto);
        entity.UserId = _currentUser.RequireUserId();
        await _uow.GetRepository<Achievement>().AddAsync(entity);
        await _uow.SaveChangesAsync();
        return Result.Ok("Başarı eklendi.");
    }

    public async Task<IResult> UpdateAsync(int id, AchievementUpdateDto dto)
    {
        var item = await _uow.GetRepository<Achievement>().FirstOrDefaultAsync(a => a.Id == id && a.UserId == _currentUser.UserId);
        if (item is null) return Result.Fail("Başarı bulunamadı.");
        _mapper.Map(dto, item);
        _uow.GetRepository<Achievement>().Update(item);
        await _uow.SaveChangesAsync();
        return Result.Ok("Başarı güncellendi.");
    }

    public async Task<IResult> DeleteAsync(int id)
    {
        var item = await _uow.GetRepository<Achievement>().FirstOrDefaultAsync(a => a.Id == id && a.UserId == _currentUser.UserId);
        if (item is null) return Result.Fail("Başarı bulunamadı.");
        await _uow.GetRepository<Achievement>().SoftDeleteAsync(id);
        await _uow.SaveChangesAsync();
        return Result.Ok("Başarı silindi.");
    }
}

public class TestimonialService : ITestimonialService
{
    private readonly UnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public TestimonialService(UnitOfWork uow, IMapper mapper, ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    // Admin-only: moderation list, includes not-yet-approved testimonials for the current tenant
    public async Task<IDataResult<IList<TestimonialDto>>> GetAllActiveAsync()
    {
        var items = await _uow.GetRepository<Testimonial>().FindAsync(t => t.UserId == _currentUser.UserId && t.IsActive);
        return DataResult<IList<TestimonialDto>>.Ok(_mapper.Map<IList<TestimonialDto>>(items));
    }

    public async Task<IDataResult<IList<TestimonialDto>>> GetApprovedAsync(string ownerId)
    {
        var items = await _uow.GetRepository<Testimonial>().FindAsync(t => t.UserId == ownerId && t.IsActive && t.IsApproved);
        return DataResult<IList<TestimonialDto>>.Ok(_mapper.Map<IList<TestimonialDto>>(items.OrderBy(t => t.DisplayOrder).ToList()));
    }

    public async Task<IDataResult<TestimonialDto>> GetByIdAsync(int id)
    {
        var item = await _uow.GetRepository<Testimonial>().FirstOrDefaultAsync(t => t.Id == id && t.UserId == _currentUser.UserId);
        return item is null ? DataResult<TestimonialDto>.Fail("Referans bulunamadı.") : DataResult<TestimonialDto>.Ok(_mapper.Map<TestimonialDto>(item));
    }

    public async Task<IResult> CreateAsync(TestimonialCreateDto dto)
    {
        var entity = _mapper.Map<Testimonial>(dto);
        entity.UserId = _currentUser.RequireUserId();
        await _uow.GetRepository<Testimonial>().AddAsync(entity);
        await _uow.SaveChangesAsync();
        return Result.Ok("Referans eklendi.");
    }

    public async Task<IResult> UpdateAsync(int id, TestimonialUpdateDto dto)
    {
        var item = await _uow.GetRepository<Testimonial>().FirstOrDefaultAsync(t => t.Id == id && t.UserId == _currentUser.UserId);
        if (item is null) return Result.Fail("Referans bulunamadı.");
        _mapper.Map(dto, item);
        _uow.GetRepository<Testimonial>().Update(item);
        await _uow.SaveChangesAsync();
        return Result.Ok("Referans güncellendi.");
    }

    public async Task<IResult> DeleteAsync(int id)
    {
        var item = await _uow.GetRepository<Testimonial>().FirstOrDefaultAsync(t => t.Id == id && t.UserId == _currentUser.UserId);
        if (item is null) return Result.Fail("Referans bulunamadı.");
        await _uow.GetRepository<Testimonial>().SoftDeleteAsync(id);
        await _uow.SaveChangesAsync();
        return Result.Ok("Referans silindi.");
    }
}

public class SocialMediaService : ISocialMediaService
{
    private readonly UnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public SocialMediaService(UnitOfWork uow, IMapper mapper, ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<IDataResult<IList<SocialMediaDto>>> GetAllActiveAsync(string ownerId)
    {
        var items = await _uow.GetRepository<SocialMedia>().FindAsync(s => s.UserId == ownerId && s.IsActive);
        return DataResult<IList<SocialMediaDto>>.Ok(_mapper.Map<IList<SocialMediaDto>>(items.OrderBy(s => s.DisplayOrder).ToList()));
    }

    public async Task<IDataResult<SocialMediaDto>> GetByIdAsync(int id)
    {
        var item = await _uow.GetRepository<SocialMedia>().FirstOrDefaultAsync(s => s.Id == id && s.UserId == _currentUser.UserId);
        return item is null ? DataResult<SocialMediaDto>.Fail("Sosyal medya bulunamadı.") : DataResult<SocialMediaDto>.Ok(_mapper.Map<SocialMediaDto>(item));
    }

    public async Task<IResult> CreateAsync(SocialMediaCreateDto dto)
    {
        var entity = _mapper.Map<SocialMedia>(dto);
        entity.UserId = _currentUser.RequireUserId();
        await _uow.GetRepository<SocialMedia>().AddAsync(entity);
        await _uow.SaveChangesAsync();
        return Result.Ok("Sosyal medya eklendi.");
    }

    public async Task<IResult> UpdateAsync(int id, SocialMediaUpdateDto dto)
    {
        var item = await _uow.GetRepository<SocialMedia>().FirstOrDefaultAsync(s => s.Id == id && s.UserId == _currentUser.UserId);
        if (item is null) return Result.Fail("Sosyal medya bulunamadı.");
        _mapper.Map(dto, item);
        _uow.GetRepository<SocialMedia>().Update(item);
        await _uow.SaveChangesAsync();
        return Result.Ok("Sosyal medya güncellendi.");
    }

    public async Task<IResult> DeleteAsync(int id)
    {
        var item = await _uow.GetRepository<SocialMedia>().FirstOrDefaultAsync(s => s.Id == id && s.UserId == _currentUser.UserId);
        if (item is null) return Result.Fail("Sosyal medya bulunamadı.");
        await _uow.GetRepository<SocialMedia>().SoftDeleteAsync(id);
        await _uow.SaveChangesAsync();
        return Result.Ok("Sosyal medya silindi.");
    }
}

public class MenuService : IMenuService
{
    private readonly UnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public MenuService(UnitOfWork uow, IMapper mapper, ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    // Not wired into any public view yet — scoped to the current admin's own menu for now.
    public async Task<IDataResult<IList<MenuItemDto>>> GetHeaderMenuAsync()
    {
        var items = await _uow.GetRepository<MenuItem>()
            .GetQueryable()
            .Where(m => m.UserId == _currentUser.UserId && m.IsActive && !m.IsDeleted && m.Location == Core.Enums.MenuLocation.Header && m.ParentMenuItemId == null)
            .Include(m => m.Children.Where(c => c.IsActive && !c.IsDeleted))
            .OrderBy(m => m.DisplayOrder)
            .ToListAsync();
        return DataResult<IList<MenuItemDto>>.Ok(_mapper.Map<IList<MenuItemDto>>(items));
    }

    public async Task<IDataResult<IList<MenuItemDto>>> GetAllAsync()
    {
        var items = await _uow.GetRepository<MenuItem>().FindAsync(m => m.UserId == _currentUser.UserId);
        return DataResult<IList<MenuItemDto>>.Ok(_mapper.Map<IList<MenuItemDto>>(items));
    }

    public async Task<IDataResult<MenuItemDto>> GetByIdAsync(int id)
    {
        var item = await _uow.GetRepository<MenuItem>().FirstOrDefaultAsync(m => m.Id == id && m.UserId == _currentUser.UserId);
        return item is null ? DataResult<MenuItemDto>.Fail("Menü öğesi bulunamadı.") : DataResult<MenuItemDto>.Ok(_mapper.Map<MenuItemDto>(item));
    }

    public async Task<IResult> CreateAsync(MenuItemCreateDto dto)
    {
        var entity = _mapper.Map<MenuItem>(dto);
        entity.UserId = _currentUser.RequireUserId();
        await _uow.GetRepository<MenuItem>().AddAsync(entity);
        await _uow.SaveChangesAsync();
        return Result.Ok("Menü öğesi eklendi.");
    }

    public async Task<IResult> UpdateAsync(int id, MenuItemUpdateDto dto)
    {
        var item = await _uow.GetRepository<MenuItem>().FirstOrDefaultAsync(m => m.Id == id && m.UserId == _currentUser.UserId);
        if (item is null) return Result.Fail("Menü öğesi bulunamadı.");
        _mapper.Map(dto, item);
        _uow.GetRepository<MenuItem>().Update(item);
        await _uow.SaveChangesAsync();
        return Result.Ok("Menü öğesi güncellendi.");
    }

    public async Task<IResult> DeleteAsync(int id)
    {
        var item = await _uow.GetRepository<MenuItem>().FirstOrDefaultAsync(m => m.Id == id && m.UserId == _currentUser.UserId);
        if (item is null) return Result.Fail("Menü öğesi bulunamadı.");
        await _uow.GetRepository<MenuItem>().SoftDeleteAsync(id);
        await _uow.SaveChangesAsync();
        return Result.Ok("Menü öğesi silindi.");
    }
}

public class FooterService : IFooterService
{
    private readonly UnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public FooterService(UnitOfWork uow, IMapper mapper, ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    // Not wired into any public view yet — scoped to the current admin's own footer content for now.
    public async Task<IDataResult<IList<FooterContentDto>>> GetAllActiveAsync()
    {
        var items = await _uow.GetRepository<FooterContent>().FindAsync(f => f.UserId == _currentUser.UserId && f.IsActive);
        return DataResult<IList<FooterContentDto>>.Ok(_mapper.Map<IList<FooterContentDto>>(items.OrderBy(f => f.DisplayOrder).ToList()));
    }

    public async Task<IDataResult<FooterContentDto>> GetByIdAsync(int id)
    {
        var item = await _uow.GetRepository<FooterContent>().FirstOrDefaultAsync(f => f.Id == id && f.UserId == _currentUser.UserId);
        return item is null ? DataResult<FooterContentDto>.Fail("Footer içeriği bulunamadı.") : DataResult<FooterContentDto>.Ok(_mapper.Map<FooterContentDto>(item));
    }

    public async Task<IResult> CreateAsync(FooterContentCreateDto dto)
    {
        var entity = _mapper.Map<FooterContent>(dto);
        entity.UserId = _currentUser.RequireUserId();
        await _uow.GetRepository<FooterContent>().AddAsync(entity);
        await _uow.SaveChangesAsync();
        return Result.Ok("Footer içeriği eklendi.");
    }

    public async Task<IResult> UpdateAsync(int id, FooterContentUpdateDto dto)
    {
        var item = await _uow.GetRepository<FooterContent>().FirstOrDefaultAsync(f => f.Id == id && f.UserId == _currentUser.UserId);
        if (item is null) return Result.Fail("Footer içeriği bulunamadı.");
        _mapper.Map(dto, item);
        _uow.GetRepository<FooterContent>().Update(item);
        await _uow.SaveChangesAsync();
        return Result.Ok("Footer içeriği güncellendi.");
    }

    public async Task<IResult> DeleteAsync(int id)
    {
        var item = await _uow.GetRepository<FooterContent>().FirstOrDefaultAsync(f => f.Id == id && f.UserId == _currentUser.UserId);
        if (item is null) return Result.Fail("Footer içeriği bulunamadı.");
        await _uow.GetRepository<FooterContent>().SoftDeleteAsync(id);
        await _uow.SaveChangesAsync();
        return Result.Ok("Footer içeriği silindi.");
    }
}

public class SeoService : ISeoService
{
    private readonly UnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public SeoService(UnitOfWork uow, IMapper mapper, ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<IDataResult<SeoSettingsDto?>> GetByPageSlugAsync(string ownerId, string slug)
    {
        var seo = await _uow.GetRepository<SeoSettings>().FirstOrDefaultAsync(s => s.UserId == ownerId && s.PageSlug == slug);
        return DataResult<SeoSettingsDto?>.Ok(_mapper.Map<SeoSettingsDto?>(seo));
    }

    public async Task<IDataResult<IList<SeoSettingsDto>>> GetAllAsync()
    {
        var items = await _uow.GetRepository<SeoSettings>().FindAsync(s => s.UserId == _currentUser.UserId);
        return DataResult<IList<SeoSettingsDto>>.Ok(_mapper.Map<IList<SeoSettingsDto>>(items));
    }

    public async Task<IResult> UpdateAsync(SeoSettingsUpdateDto dto)
    {
        var seo = await _uow.GetRepository<SeoSettings>().FirstOrDefaultAsync(s => s.Id == dto.Id && s.UserId == _currentUser.UserId);
        if (seo is null) return Result.Fail("SEO ayarı bulunamadı.");
        _mapper.Map(dto, seo);
        _uow.GetRepository<SeoSettings>().Update(seo);
        await _uow.SaveChangesAsync();
        return Result.Ok("SEO ayarları güncellendi.");
    }
}
