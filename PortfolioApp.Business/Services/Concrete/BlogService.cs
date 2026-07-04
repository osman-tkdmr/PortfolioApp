using AutoMapper;
using FluentValidation;
using PortfolioApp.Business.Security;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.Core.Interfaces;
using PortfolioApp.Core.Results;
using PortfolioApp.Core.Utilities;
using PortfolioApp.DataAccess.Repositories.Concrete;
using PortfolioApp.DataAccess.UnitOfWork;
using PortfolioApp.DTO.DTOs.Blog;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.Business.Services.Concrete;

public class BlogService : IBlogService
{
    private readonly UnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IValidator<BlogPostCreateDto> _createValidator;
    private readonly IValidator<BlogPostUpdateDto> _updateValidator;
    private readonly ICurrentUserService _currentUser;

    public BlogService(UnitOfWork uow, IMapper mapper,
        IValidator<BlogPostCreateDto> createValidator,
        IValidator<BlogPostUpdateDto> updateValidator,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _currentUser = currentUser;
    }

    public async Task<IDataResult<BlogPostDto>> GetByIdAsync(int id)
    {
        var post = await _uow.GetRepository<BlogPost>().FirstOrDefaultAsync(p => p.Id == id && p.AuthorId == _currentUser.UserId);
        return post is null
            ? DataResult<BlogPostDto>.Fail("Blog yazısı bulunamadı.")
            : DataResult<BlogPostDto>.Ok(_mapper.Map<BlogPostDto>(post));
    }

    public async Task<IDataResult<BlogPostDto>> GetBySlugAsync(string ownerId, string slug)
    {
        var post = await _uow.BlogPosts.GetBySlugAsync(ownerId, slug);
        return post is null
            ? DataResult<BlogPostDto>.Fail("Blog yazısı bulunamadı.")
            : DataResult<BlogPostDto>.Ok(_mapper.Map<BlogPostDto>(post));
    }

    public async Task<IDataResult<IList<BlogPostDto>>> GetAllAsync()
    {
        var posts = await _uow.GetRepository<BlogPost>().FindAsync(p => p.AuthorId == _currentUser.UserId);
        return DataResult<IList<BlogPostDto>>.Ok(_mapper.Map<IList<BlogPostDto>>(posts));
    }

    public async Task<IDataResult<IList<BlogPostDto>>> GetPublishedAsync()
    {
        var posts = await _uow.GetRepository<BlogPost>().FindAsync(p => p.IsPublished);
        return DataResult<IList<BlogPostDto>>.Ok(_mapper.Map<IList<BlogPostDto>>(posts));
    }

    public async Task<IDataResult<IList<BlogPostDto>>> GetFeaturedAsync(int count = 3)
    {
        var posts = await _uow.BlogPosts.GetFeaturedAsync(count);
        return DataResult<IList<BlogPostDto>>.Ok(_mapper.Map<IList<BlogPostDto>>(posts));
    }

    public async Task<IDataResult<IList<BlogPostDto>>> GetRecentAsync(string ownerId, int count = 5)
    {
        var posts = await _uow.BlogPosts.GetRecentAsync(ownerId, count);
        return DataResult<IList<BlogPostDto>>.Ok(_mapper.Map<IList<BlogPostDto>>(posts));
    }

    public async Task<IDataResult<PaginatedResult<BlogPostDto>>> GetPagedAsync(string ownerId, int page, int pageSize, string? categorySlug = null, string? tagSlug = null, string? search = null)
    {
        var (posts, totalCount) = await _uow.BlogPosts.GetPagedAsync(ownerId, page, pageSize, categorySlug, tagSlug, search);
        var result = PaginatedResult<BlogPostDto>.Create(
            _mapper.Map<IList<BlogPostDto>>(posts),
            totalCount, page, pageSize);
        return DataResult<PaginatedResult<BlogPostDto>>.Ok(result);
    }

    public async Task<IResult> CreateAsync(BlogPostCreateDto dto, string authorId)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return Result.Fail(string.Join(", ", validation.Errors.Select(e => e.ErrorMessage)));

        var post = _mapper.Map<BlogPost>(dto);
        post.Content = RichTextSanitizer.Sanitize(dto.Content);
        post.AuthorId = authorId;
        post.ReadTimeMinutes = PaginationHelper.CalculateReadingTime(post.Content);

        if (dto.IsPublished)
            post.PublishedAt = DateTime.UtcNow;

        await _uow.GetRepository<BlogPost>().AddAsync(post);
        await _uow.SaveChangesAsync();

        if (dto.TagIds.Any())
        {
            foreach (var tagId in dto.TagIds)
                await _uow.Context.BlogPostTags.AddAsync(new BlogPostTag { BlogPostId = post.Id, BlogTagId = tagId });
            await _uow.SaveChangesAsync();
        }

        return Result.Ok("Blog yazısı başarıyla eklendi.");
    }

    public async Task<IResult> UpdateAsync(int id, BlogPostUpdateDto dto)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return Result.Fail(string.Join(", ", validation.Errors.Select(e => e.ErrorMessage)));

        var post = await _uow.GetRepository<BlogPost>().FirstOrDefaultAsync(p => p.Id == id && p.AuthorId == _currentUser.UserId);
        if (post is null)
            return Result.Fail("Blog yazısı bulunamadı.");

        _mapper.Map(dto, post);
        post.Content = RichTextSanitizer.Sanitize(dto.Content);
        post.ReadTimeMinutes = PaginationHelper.CalculateReadingTime(post.Content);

        _uow.GetRepository<BlogPost>().Update(post);

        // Update tags — delete existing, re-add
        var existingTags = _uow.Context.BlogPostTags.Where(t => t.BlogPostId == id).ToList();
        _uow.Context.BlogPostTags.RemoveRange(existingTags);

        foreach (var tagId in dto.TagIds)
            await _uow.Context.BlogPostTags.AddAsync(new BlogPostTag { BlogPostId = id, BlogTagId = tagId });

        await _uow.SaveChangesAsync();
        return Result.Ok("Blog yazısı güncellendi.");
    }

    public async Task<IResult> DeleteAsync(int id)
    {
        var post = await _uow.GetRepository<BlogPost>().FirstOrDefaultAsync(p => p.Id == id && p.AuthorId == _currentUser.UserId);
        if (post is null) return Result.Fail("Blog yazısı bulunamadı.");
        await _uow.GetRepository<BlogPost>().SoftDeleteAsync(id);
        await _uow.SaveChangesAsync();
        return Result.Ok("Blog yazısı silindi.");
    }

    public async Task<IResult> PublishAsync(int id)
    {
        var post = await _uow.GetRepository<BlogPost>().FirstOrDefaultAsync(p => p.Id == id && p.AuthorId == _currentUser.UserId);
        if (post is null) return Result.Fail("Blog yazısı bulunamadı.");
        post.IsPublished = true;
        post.PublishedAt = DateTime.UtcNow;
        _uow.GetRepository<BlogPost>().Update(post);
        await _uow.SaveChangesAsync();
        return Result.Ok("Blog yazısı yayınlandı.");
    }

    public async Task<IResult> UnpublishAsync(int id)
    {
        var post = await _uow.GetRepository<BlogPost>().FirstOrDefaultAsync(p => p.Id == id && p.AuthorId == _currentUser.UserId);
        if (post is null) return Result.Fail("Blog yazısı bulunamadı.");
        post.IsPublished = false;
        _uow.GetRepository<BlogPost>().Update(post);
        await _uow.SaveChangesAsync();
        return Result.Ok("Blog yazısı yayından kaldırıldı.");
    }

    public async Task IncrementViewCountAsync(int id) =>
        await _uow.BlogPosts.IncrementViewCountAsync(id);

    // Categories — shared with public (category filter dropdown)
    public async Task<IDataResult<IList<BlogCategoryDto>>> GetCategoriesAsync(string ownerId)
    {
        var cats = await _uow.GetRepository<BlogCategory>().FindAsync(c => c.UserId == ownerId);
        return DataResult<IList<BlogCategoryDto>>.Ok(_mapper.Map<IList<BlogCategoryDto>>(cats));
    }

    public async Task<IResult> CreateCategoryAsync(BlogCategoryCreateDto dto)
    {
        var category = _mapper.Map<BlogCategory>(dto);
        category.Slug = SlugHelper.Slugify(dto.Name);
        category.UserId = _currentUser.RequireUserId();
        await _uow.GetRepository<BlogCategory>().AddAsync(category);
        await _uow.SaveChangesAsync();
        return Result.Ok("Kategori eklendi.");
    }

    public async Task<IResult> UpdateCategoryAsync(int id, BlogCategoryUpdateDto dto)
    {
        var cat = await _uow.GetRepository<BlogCategory>().FirstOrDefaultAsync(c => c.Id == id && c.UserId == _currentUser.UserId);
        if (cat is null) return Result.Fail("Kategori bulunamadı.");
        _mapper.Map(dto, cat);
        _uow.GetRepository<BlogCategory>().Update(cat);
        await _uow.SaveChangesAsync();
        return Result.Ok("Kategori güncellendi.");
    }

    public async Task<IResult> DeleteCategoryAsync(int id)
    {
        var cat = await _uow.GetRepository<BlogCategory>().FirstOrDefaultAsync(c => c.Id == id && c.UserId == _currentUser.UserId);
        if (cat is null) return Result.Fail("Kategori bulunamadı.");
        await _uow.GetRepository<BlogCategory>().SoftDeleteAsync(id);
        await _uow.SaveChangesAsync();
        return Result.Ok("Kategori silindi.");
    }

    // Tags — admin-only management
    public async Task<IDataResult<IList<BlogTagDto>>> GetTagsAsync()
    {
        var tags = await _uow.GetRepository<BlogTag>().FindAsync(t => t.UserId == _currentUser.UserId);
        return DataResult<IList<BlogTagDto>>.Ok(_mapper.Map<IList<BlogTagDto>>(tags));
    }

    public async Task<IResult> CreateTagAsync(BlogTagCreateDto dto)
    {
        var tag = _mapper.Map<BlogTag>(dto);
        tag.UserId = _currentUser.RequireUserId();
        await _uow.GetRepository<BlogTag>().AddAsync(tag);
        await _uow.SaveChangesAsync();
        return Result.Ok("Etiket eklendi.");
    }

    public async Task<IResult> DeleteTagAsync(int id)
    {
        var tag = await _uow.GetRepository<BlogTag>().FirstOrDefaultAsync(t => t.Id == id && t.UserId == _currentUser.UserId);
        if (tag is null) return Result.Fail("Etiket bulunamadı.");
        await _uow.GetRepository<BlogTag>().SoftDeleteAsync(id);
        await _uow.SaveChangesAsync();
        return Result.Ok("Etiket silindi.");
    }
}
