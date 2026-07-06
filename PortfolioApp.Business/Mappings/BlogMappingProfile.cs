using AutoMapper;
using PortfolioApp.Core.Utilities;
using PortfolioApp.DTO.DTOs.Blog;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.Business.Mappings;

public class BlogMappingProfile : Profile
{
    public BlogMappingProfile()
    {
        CreateMap<BlogPost, BlogPostDto>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.BlogCategory != null ? s.BlogCategory.Name : string.Empty))
            .ForMember(d => d.AuthorName, o => o.MapFrom(s => s.Author != null ? s.Author.FullName : string.Empty))
            .ForMember(d => d.AuthorImageUrl, o => o.MapFrom(s => s.Author != null ? s.Author.ProfileImageUrl : null))
            .ForMember(d => d.TagNames, o => o.MapFrom(s => s.BlogPostTags.Select(t => t.BlogTag.Name).ToList()))
            .ForMember(d => d.Tags, o => o.MapFrom(s => s.BlogPostTags.Select(t => t.BlogTag)));

        // MemberList.None: ViewCount/ReadTimeMinutes/Comments are computed or defaulted server-side
        // (BlogService.CreateAsync sets ReadTimeMinutes via PaginationHelper, ViewCount starts at 0).
        CreateMap<BlogPostCreateDto, BlogPost>(MemberList.None)
            .ForMember(d => d.Slug, o => o.MapFrom(s => SlugHelper.Slugify(s.Title)))
            .ForMember(d => d.PublishedAt, o => o.MapFrom(s => s.IsPublished ? (DateTime?)DateTime.UtcNow : null))
            .ForMember(d => d.BlogPostTags, o => o.Ignore())
            .ForMember(d => d.BlogCategory, o => o.Ignore())
            .ForMember(d => d.Author, o => o.Ignore());

        CreateMap<BlogPostUpdateDto, BlogPost>(MemberList.None)
            .ForMember(d => d.Slug, o => o.MapFrom(s => SlugHelper.Slugify(s.Title)))
            .ForMember(d => d.BlogPostTags, o => o.Ignore())
            .ForMember(d => d.BlogCategory, o => o.Ignore())
            .ForMember(d => d.Author, o => o.Ignore());

        CreateMap<BlogCategory, BlogCategoryDto>()
            .ForMember(d => d.PostCount, o => o.MapFrom(s => s.BlogPosts.Count(p => p.IsPublished && !p.IsDeleted)));

        // MemberList.None: Slug is computed via SlugHelper.Slugify in BlogService.CreateCategoryAsync, not through AutoMapper.
        CreateMap<BlogCategoryCreateDto, BlogCategory>(MemberList.None);
        CreateMap<BlogCategoryUpdateDto, BlogCategory>(MemberList.None);

        CreateMap<BlogTag, BlogTagDto>()
            .ForMember(d => d.PostCount, o => o.MapFrom(s => s.BlogPostTags.Count));

        // MemberList.None: DisplayOrder isn't exposed on BlogTagCreateDto (defaults to 0); BlogPostTags is a nav collection.
        CreateMap<BlogTagCreateDto, BlogTag>(MemberList.None)
            .ForMember(d => d.Slug, o => o.MapFrom(s => SlugHelper.Slugify(s.Name)));
    }
}
