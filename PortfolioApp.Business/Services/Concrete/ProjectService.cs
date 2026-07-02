using AutoMapper;
using FluentValidation;
using PortfolioApp.Business.Security;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.Core.Results;
using PortfolioApp.Core.Utilities;
using PortfolioApp.DataAccess.UnitOfWork;
using PortfolioApp.DTO.DTOs.Project;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.Business.Services.Concrete;

public class ProjectService : IProjectService
{
    private readonly UnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IValidator<ProjectCreateDto> _createValidator;

    public ProjectService(UnitOfWork uow, IMapper mapper, IValidator<ProjectCreateDto> createValidator)
    {
        _uow = uow;
        _mapper = mapper;
        _createValidator = createValidator;
    }

    public async Task<IDataResult<ProjectDto>> GetByIdAsync(int id)
    {
        var project = await _uow.Projects.GetWithDetailsAsync(id);
        return project is null
            ? DataResult<ProjectDto>.Fail("Proje bulunamadı.")
            : DataResult<ProjectDto>.Ok(_mapper.Map<ProjectDto>(project));
    }

    public async Task<IDataResult<ProjectDto>> GetBySlugAsync(string slug)
    {
        var project = await _uow.Projects.GetBySlugAsync(slug);
        return project is null
            ? DataResult<ProjectDto>.Fail("Proje bulunamadı.")
            : DataResult<ProjectDto>.Ok(_mapper.Map<ProjectDto>(project));
    }

    public async Task<IDataResult<IList<ProjectDto>>> GetAllAsync()
    {
        var projects = await _uow.GetRepository<Project>().GetAllAsync();
        return DataResult<IList<ProjectDto>>.Ok(_mapper.Map<IList<ProjectDto>>(projects));
    }

    public async Task<IDataResult<IList<ProjectDto>>> GetAllActiveAsync()
    {
        var projects = await _uow.GetRepository<Project>().FindAsync(p => p.IsActive);
        return DataResult<IList<ProjectDto>>.Ok(_mapper.Map<IList<ProjectDto>>(projects));
    }

    public async Task<IDataResult<IList<ProjectDto>>> GetFeaturedAsync(int count = 6)
    {
        var projects = await _uow.Projects.GetFeaturedAsync(count);
        return DataResult<IList<ProjectDto>>.Ok(_mapper.Map<IList<ProjectDto>>(projects));
    }

    public async Task<IDataResult<PaginatedResult<ProjectDto>>> GetPagedAsync(int page, int pageSize, string? categorySlug = null)
    {
        var (projects, totalCount) = await _uow.Projects.GetPagedAsync(page, pageSize, categorySlug);
        var result = PaginatedResult<ProjectDto>.Create(_mapper.Map<IList<ProjectDto>>(projects), totalCount, page, pageSize);
        return DataResult<PaginatedResult<ProjectDto>>.Ok(result);
    }

    public async Task<IResult> CreateAsync(ProjectCreateDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return Result.Fail(string.Join(", ", validation.Errors.Select(e => e.ErrorMessage)));

        var project = _mapper.Map<Project>(dto);
        project.Description = RichTextSanitizer.Sanitize(dto.Description);
        await _uow.GetRepository<Project>().AddAsync(project);
        await _uow.SaveChangesAsync();

        if (dto.TechnologyIds.Any())
        {
            foreach (var techId in dto.TechnologyIds)
                await _uow.Context.ProjectTechnologies.AddAsync(new ProjectTechnology { ProjectId = project.Id, TechnologyId = techId });
            await _uow.SaveChangesAsync();
        }

        return Result.Ok("Proje başarıyla eklendi.");
    }

    public async Task<IResult> UpdateAsync(int id, ProjectUpdateDto dto)
    {
        var project = await _uow.GetRepository<Project>().GetByIdAsync(id);
        if (project is null) return Result.Fail("Proje bulunamadı.");

        _mapper.Map(dto, project);
        project.Description = RichTextSanitizer.Sanitize(dto.Description);
        _uow.GetRepository<Project>().Update(project);

        // Update technologies
        var existing = _uow.Context.ProjectTechnologies.Where(pt => pt.ProjectId == id).ToList();
        _uow.Context.ProjectTechnologies.RemoveRange(existing);

        foreach (var techId in dto.TechnologyIds)
            await _uow.Context.ProjectTechnologies.AddAsync(new ProjectTechnology { ProjectId = id, TechnologyId = techId });

        await _uow.SaveChangesAsync();
        return Result.Ok("Proje güncellendi.");
    }

    public async Task<IResult> DeleteAsync(int id)
    {
        await _uow.GetRepository<Project>().SoftDeleteAsync(id);
        await _uow.SaveChangesAsync();
        return Result.Ok("Proje silindi.");
    }

    public async Task<IResult> ToggleFeaturedAsync(int id)
    {
        var project = await _uow.GetRepository<Project>().GetByIdAsync(id);
        if (project is null) return Result.Fail("Proje bulunamadı.");
        project.IsFeatured = !project.IsFeatured;
        _uow.GetRepository<Project>().Update(project);
        await _uow.SaveChangesAsync();
        return Result.Ok(project.IsFeatured ? "Proje öne çıkarıldı." : "Proje öne çıkarmadan kaldırıldı.");
    }

    // Categories
    public async Task<IDataResult<IList<ProjectCategoryDto>>> GetCategoriesAsync()
    {
        var cats = await _uow.GetRepository<ProjectCategory>().GetAllAsync();
        return DataResult<IList<ProjectCategoryDto>>.Ok(_mapper.Map<IList<ProjectCategoryDto>>(cats));
    }

    public async Task<IResult> CreateCategoryAsync(ProjectCategoryCreateDto dto)
    {
        var cat = _mapper.Map<ProjectCategory>(dto);
        await _uow.GetRepository<ProjectCategory>().AddAsync(cat);
        await _uow.SaveChangesAsync();
        return Result.Ok("Kategori eklendi.");
    }

    public async Task<IResult> UpdateCategoryAsync(int id, ProjectCategoryUpdateDto dto)
    {
        var cat = await _uow.GetRepository<ProjectCategory>().GetByIdAsync(id);
        if (cat is null) return Result.Fail("Kategori bulunamadı.");
        _mapper.Map(dto, cat);
        _uow.GetRepository<ProjectCategory>().Update(cat);
        await _uow.SaveChangesAsync();
        return Result.Ok("Kategori güncellendi.");
    }

    public async Task<IResult> DeleteCategoryAsync(int id)
    {
        await _uow.GetRepository<ProjectCategory>().SoftDeleteAsync(id);
        await _uow.SaveChangesAsync();
        return Result.Ok("Kategori silindi.");
    }

    // Technologies
    public async Task<IDataResult<IList<TechnologyDto>>> GetTechnologiesAsync()
    {
        var techs = await _uow.GetRepository<Technology>().GetAllAsync();
        return DataResult<IList<TechnologyDto>>.Ok(_mapper.Map<IList<TechnologyDto>>(techs));
    }

    public async Task<IResult> CreateTechnologyAsync(TechnologyCreateDto dto)
    {
        var tech = _mapper.Map<Technology>(dto);
        await _uow.GetRepository<Technology>().AddAsync(tech);
        await _uow.SaveChangesAsync();
        return Result.Ok("Teknoloji eklendi.");
    }

    public async Task<IResult> UpdateTechnologyAsync(int id, TechnologyUpdateDto dto)
    {
        var tech = await _uow.GetRepository<Technology>().GetByIdAsync(id);
        if (tech is null) return Result.Fail("Teknoloji bulunamadı.");
        _mapper.Map(dto, tech);
        _uow.GetRepository<Technology>().Update(tech);
        await _uow.SaveChangesAsync();
        return Result.Ok("Teknoloji güncellendi.");
    }

    public async Task<IResult> DeleteTechnologyAsync(int id)
    {
        await _uow.GetRepository<Technology>().SoftDeleteAsync(id);
        await _uow.SaveChangesAsync();
        return Result.Ok("Teknoloji silindi.");
    }
}
