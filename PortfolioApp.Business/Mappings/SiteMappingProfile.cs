using AutoMapper;
using PortfolioApp.DTO.DTOs.Site;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.Business.Mappings;

public class SiteMappingProfile : Profile
{
    public SiteMappingProfile()
    {
        CreateMap<SiteSettings, SiteSettingsDto>();
        CreateMap<SiteSettingsUpdateDto, SiteSettings>().ForMember(d => d.Id, o => o.Ignore());

        CreateMap<SeoSettings, SeoSettingsDto>();
        CreateMap<SeoSettingsUpdateDto, SeoSettings>().ForMember(d => d.Id, o => o.Ignore());

        CreateMap<Theme, ThemeDto>();

        CreateMap<HeroSection, HeroSectionDto>();
        CreateMap<HeroSectionUpdateDto, HeroSection>().ForMember(d => d.Id, o => o.Ignore());

        CreateMap<ContactInfo, ContactInfoDto>();
        CreateMap<ContactInfoUpdateDto, ContactInfo>().ForMember(d => d.Id, o => o.Ignore());

        CreateMap<ContactMessage, ContactMessageDto>();
        CreateMap<ContactMessageCreateDto, ContactMessage>();

        CreateMap<SocialMedia, SocialMediaDto>();
        CreateMap<SocialMediaCreateDto, SocialMedia>();
        CreateMap<SocialMediaUpdateDto, SocialMedia>().ForMember(d => d.Id, o => o.Ignore());

        CreateMap<MenuItem, MenuItemDto>()
            .ForMember(d => d.Children, o => o.MapFrom(s => s.Children.Where(c => c.IsActive && !c.IsDeleted)));

        CreateMap<MenuItemCreateDto, MenuItem>()
            .ForMember(d => d.Children, o => o.Ignore())
            .ForMember(d => d.ParentMenuItem, o => o.Ignore());

        CreateMap<MenuItemUpdateDto, MenuItem>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Children, o => o.Ignore())
            .ForMember(d => d.ParentMenuItem, o => o.Ignore());

        CreateMap<FooterContent, FooterContentDto>();
        CreateMap<FooterContentCreateDto, FooterContent>();
        CreateMap<FooterContentUpdateDto, FooterContent>().ForMember(d => d.Id, o => o.Ignore());

        CreateMap<Testimonial, TestimonialDto>();
        CreateMap<TestimonialCreateDto, Testimonial>();
        CreateMap<TestimonialUpdateDto, Testimonial>().ForMember(d => d.Id, o => o.Ignore());
    }
}
