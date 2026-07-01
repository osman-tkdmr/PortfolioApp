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

        CreateMap<BlogPostCreateDto, BlogPost>()
            .ForMember(d => d.Slug, o => o.MapFrom(s => SlugHelper.Slugify(s.Title)))
            .ForMember(d => d.PublishedAt, o => o.MapFrom(s => s.IsPublished ? (DateTime?)DateTime.UtcNow : null))
            .ForMember(d => d.BlogPostTags, o => o.Ignore())
            .ForMember(d => d.BlogCategory, o => o.Ignore())
            .ForMember(d => d.Author, o => o.Ignore());

        CreateMap<BlogPostUpdateDto, BlogPost>()
            .ForMember(d => d.Slug, o => o.MapFrom(s => SlugHelper.Slugify(s.Title)))
            .ForMember(d => d.BlogPostTags, o => o.Ignore())
            .ForMember(d => d.BlogCategory, o => o.Ignore())
            .ForMember(d => d.Author, o => o.Ignore());

        CreateMap<BlogCategory, BlogCategoryDto>()
            .ForMember(d => d.PostCount, o => o.MapFrom(s => s.BlogPosts.Count(p => p.IsPublished && !p.IsDeleted)));

        CreateMap<BlogCategoryCreateDto, BlogCategory>();
        CreateMap<BlogCategoryUpdateDto, BlogCategory>();

        CreateMap<BlogTag, BlogTagDto>()
            .ForMember(d => d.PostCount, o => o.MapFrom(s => s.BlogPostTags.Count));

        CreateMap<BlogTagCreateDto, BlogTag>()
            .ForMember(d => d.Slug, o => o.MapFrom(s => SlugHelper.Slugify(s.Name)));
    }
}
