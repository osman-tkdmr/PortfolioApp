using PortfolioApp.Core.Results;
using PortfolioApp.DTO.DTOs.Project;

namespace PortfolioApp.Business.Services.Interfaces;

public interface IProjectService
{
    Task<IDataResult<ProjectDto>> GetByIdAsync(int id);
    Task<IDataResult<ProjectDto>> GetBySlugAsync(string slug);
    Task<IDataResult<IList<ProjectDto>>> GetAllAsync();
    Task<IDataResult<IList<ProjectDto>>> GetAllActiveAsync();
    Task<IDataResult<IList<ProjectDto>>> GetFeaturedAsync(int count = 6);
    Task<IDataResult<PaginatedResult<ProjectDto>>> GetPagedAsync(int page, int pageSize, string? categorySlug = null);
    Task<IResult> CreateAsync(ProjectCreateDto dto);
    Task<IResult> UpdateAsync(int id, ProjectUpdateDto dto);
    Task<IResult> DeleteAsync(int id);
    Task<IResult> ToggleFeaturedAsync(int id);

    Task<IDataResult<IList<ProjectCategoryDto>>> GetCategoriesAsync();
    Task<IResult> CreateCategoryAsync(ProjectCategoryCreateDto dto);
    Task<IResult> UpdateCategoryAsync(int id, ProjectCategoryUpdateDto dto);
    Task<IResult> DeleteCategoryAsync(int id);

    Task<IDataResult<IList<TechnologyDto>>> GetTechnologiesAsync();
    Task<IResult> CreateTechnologyAsync(TechnologyCreateDto dto);
    Task<IResult> UpdateTechnologyAsync(int id, TechnologyUpdateDto dto);
    Task<IResult> DeleteTechnologyAsync(int id);
}
