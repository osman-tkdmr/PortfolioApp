using AutoMapper;
using PortfolioApp.Core.Utilities;
using PortfolioApp.DTO.DTOs.Project;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.Business.Mappings;

public class ProjectMappingProfile : Profile
{
    public ProjectMappingProfile()
    {
        CreateMap<Project, ProjectDto>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.ProjectCategory != null ? s.ProjectCategory.Name : string.Empty))
            .ForMember(d => d.Technologies, o => o.MapFrom(s => s.ProjectTechnologies.Select(pt => pt.Technology)));

        CreateMap<ProjectCreateDto, Project>()
            .ForMember(d => d.Slug, o => o.MapFrom(s => SlugHelper.Slugify(s.Title)))
            .ForMember(d => d.ProjectTechnologies, o => o.Ignore())
            .ForMember(d => d.Images, o => o.Ignore())
            .ForMember(d => d.ProjectCategory, o => o.Ignore());

        CreateMap<ProjectUpdateDto, Project>()
            .ForMember(d => d.Slug, o => o.MapFrom(s => SlugHelper.Slugify(s.Title)))
            .ForMember(d => d.ProjectTechnologies, o => o.Ignore())
            .ForMember(d => d.Images, o => o.Ignore())
            .ForMember(d => d.ProjectCategory, o => o.Ignore());

        CreateMap<ProjectImage, ProjectImageDto>();

        // MemberList.None: ProjectTechnologies is a nav collection, never settable through the technology create/update DTO.
        CreateMap<Technology, TechnologyDto>();
        CreateMap<TechnologyCreateDto, Technology>(MemberList.None);
        CreateMap<TechnologyUpdateDto, Technology>(MemberList.None);

        CreateMap<ProjectCategory, ProjectCategoryDto>()
            .ForMember(d => d.ProjectCount, o => o.MapFrom(s => s.Projects.Count(p => p.IsActive && !p.IsDeleted)));

        // MemberList.None: Projects is a nav collection, never settable through the category create/update DTO.
        CreateMap<ProjectCategoryCreateDto, ProjectCategory>(MemberList.None)
            .ForMember(d => d.Slug, o => o.MapFrom(s => SlugHelper.Slugify(s.Name)));

        CreateMap<ProjectCategoryUpdateDto, ProjectCategory>(MemberList.None)
            .ForMember(d => d.Slug, o => o.MapFrom(s => SlugHelper.Slugify(s.Name)));
    }
}
