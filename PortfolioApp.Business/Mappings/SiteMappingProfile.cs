using AutoMapper;
using PortfolioApp.DTO.DTOs.Site;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.Business.Mappings;

public class SiteMappingProfile : Profile
{
    public SiteMappingProfile()
    {
        CreateMap<SiteSettings, SiteSettingsDto>();
        // MemberList.None: ActiveTheme is a navigation property only ever changed via ThemeService.ActivateThemeAsync
        // (through the ActiveThemeId scalar), never directly through this update DTO.
        CreateMap<SiteSettingsUpdateDto, SiteSettings>(MemberList.None).ForMember(d => d.Id, o => o.Ignore());

        CreateMap<SeoSettings, SeoSettingsDto>();
        CreateMap<SeoSettingsUpdateDto, SeoSettings>().ForMember(d => d.Id, o => o.Ignore());

        CreateMap<Theme, ThemeDto>();

        CreateMap<HeroSection, HeroSectionDto>();
        CreateMap<HeroSectionUpdateDto, HeroSection>().ForMember(d => d.Id, o => o.Ignore());

        CreateMap<ContactInfo, ContactInfoDto>();
        // MemberList.None: IsActive isn't exposed on ContactInfoDto/ContactInfoUpdateDto — toggled elsewhere, not through this update path.
        CreateMap<ContactInfoUpdateDto, ContactInfo>(MemberList.None).ForMember(d => d.Id, o => o.Ignore());

        CreateMap<ContactMessage, ContactMessageDto>();
        // MemberList.None: IsRead/IsReplied/ReplyText/RepliedAt/IpAddress/IsSpam are set by dedicated
        // ContactService methods (MarkAsReadAsync/ReplyAsync/MarkAsSpamAsync), never by the public create DTO.
        CreateMap<ContactMessageCreateDto, ContactMessage>(MemberList.None);

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
