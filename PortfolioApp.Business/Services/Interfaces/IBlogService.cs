using PortfolioApp.Core.Results;
using PortfolioApp.DTO.DTOs.Blog;

namespace PortfolioApp.Business.Services.Interfaces;

public interface IBlogService
{
    Task<IDataResult<BlogPostDto>> GetByIdAsync(int id);
    Task<IDataResult<BlogPostDto>> GetBySlugAsync(string ownerId, string slug);
    Task<IDataResult<IList<BlogPostDto>>> GetAllAsync();
    Task<IDataResult<IList<BlogPostDto>>> GetPublishedAsync();
    Task<IDataResult<IList<BlogPostDto>>> GetFeaturedAsync(int count = 3);
    Task<IDataResult<IList<BlogPostDto>>> GetRecentAsync(string ownerId, int count = 5);
    Task<IDataResult<PaginatedResult<BlogPostDto>>> GetPagedAsync(string ownerId, int page, int pageSize, string? categorySlug = null, string? tagSlug = null, string? search = null);
    Task<IResult> CreateAsync(BlogPostCreateDto dto, string authorId);
    Task<IResult> UpdateAsync(int id, BlogPostUpdateDto dto);
    Task<IResult> DeleteAsync(int id);
    Task<IResult> PublishAsync(int id);
    Task<IResult> UnpublishAsync(int id);
    Task IncrementViewCountAsync(int id);

    Task<IDataResult<IList<BlogCategoryDto>>> GetCategoriesAsync(string ownerId);
    Task<IResult> CreateCategoryAsync(BlogCategoryCreateDto dto);
    Task<IResult> UpdateCategoryAsync(int id, BlogCategoryUpdateDto dto);
    Task<IResult> DeleteCategoryAsync(int id);

    Task<IDataResult<IList<BlogTagDto>>> GetTagsAsync();
    Task<IResult> CreateTagAsync(BlogTagCreateDto dto);
    Task<IResult> DeleteTagAsync(int id);
}
