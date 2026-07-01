using AutoMapper;
using PortfolioApp.DTO.DTOs.Portfolio;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.Business.Mappings;

public class PortfolioMappingProfile : Profile
{
    public PortfolioMappingProfile()
    {
        CreateMap<About, AboutDto>();
        CreateMap<AboutUpdateDto, About>()
            .ForMember(d => d.Id, o => o.Ignore());

        CreateMap<Experience, ExperienceDto>();
        CreateMap<ExperienceCreateDto, Experience>();
        CreateMap<ExperienceUpdateDto, Experience>().ForMember(d => d.Id, o => o.Ignore());

        CreateMap<Education, EducationDto>();
        CreateMap<EducationCreateDto, Education>();
        CreateMap<EducationUpdateDto, Education>().ForMember(d => d.Id, o => o.Ignore());

        CreateMap<Certificate, CertificateDto>();
        CreateMap<CertificateCreateDto, Certificate>();
        CreateMap<CertificateUpdateDto, Certificate>().ForMember(d => d.Id, o => o.Ignore());

        CreateMap<SkillCategory, SkillCategoryDto>()
            .ForMember(d => d.Skills, o => o.MapFrom(s => s.Skills.Where(sk => sk.IsActive && !sk.IsDeleted)));

        CreateMap<SkillCategoryCreateDto, SkillCategory>();
        CreateMap<SkillCategoryUpdateDto, SkillCategory>().ForMember(d => d.Id, o => o.Ignore());

        CreateMap<Skill, SkillDto>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.SkillCategory != null ? s.SkillCategory.Name : string.Empty));

        CreateMap<SkillCreateDto, Skill>();
        CreateMap<SkillUpdateDto, Skill>().ForMember(d => d.Id, o => o.Ignore());

        CreateMap<Language, LanguageDto>();
        CreateMap<LanguageCreateDto, Language>();
        CreateMap<LanguageUpdateDto, Language>().ForMember(d => d.Id, o => o.Ignore());

        CreateMap<Achievement, AchievementDto>();
        CreateMap<AchievementCreateDto, Achievement>();
        CreateMap<AchievementUpdateDto, Achievement>().ForMember(d => d.Id, o => o.Ignore());
    }
}
