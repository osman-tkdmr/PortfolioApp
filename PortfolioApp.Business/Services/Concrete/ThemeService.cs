using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.Core.Constants;
using PortfolioApp.Core.Results;
using PortfolioApp.DataAccess.UnitOfWork;
using PortfolioApp.DTO.DTOs.Site;
using PortfolioApp.Entity.Concrete;
using Microsoft.EntityFrameworkCore;

namespace PortfolioApp.Business.Services.Concrete;

public class ThemeService : IThemeService
{
    private readonly UnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;

    public ThemeService(UnitOfWork uow, IMapper mapper, IMemoryCache cache)
    {
        _uow = uow;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<IDataResult<ThemeDto?>> GetActiveThemeAsync()
    {
        var theme = await _cache.GetOrCreateAsync("active_theme_entity", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
            return await _uow.GetRepository<Theme>()
                .FirstOrDefaultAsync(t => t.IsActive);
        });

        return DataResult<ThemeDto?>.Ok(_mapper.Map<ThemeDto?>(theme));
    }

    public async Task<IDataResult<IList<ThemeDto>>> GetAllAsync()
    {
        var themes = await _uow.GetRepository<Theme>().GetAllAsync();
        return DataResult<IList<ThemeDto>>.Ok(_mapper.Map<IList<ThemeDto>>(themes));
    }

    public async Task<IResult> ActivateThemeAsync(int themeId)
    {
        var target = await _uow.GetRepository<Theme>().GetByIdAsync(themeId);
        if (target is null)
            return Result.Fail("Tema bulunamadı.");

        await _uow.BeginTransactionAsync();
        try
        {
            // Deactivate all themes
            var allThemes = await _uow.GetRepository<Theme>().GetAllAsync();
            foreach (var theme in allThemes.Where(t => t.IsActive))
            {
                theme.IsActive = false;
                _uow.GetRepository<Theme>().Update(theme);
            }

            // Activate selected
            target.IsActive = true;
            _uow.GetRepository<Theme>().Update(target);

            // Update SiteSettings
            var settings = await _uow.GetRepository<SiteSettings>().FirstOrDefaultAsync(_ => true);
            if (settings is not null)
            {
                settings.ActiveThemeId = themeId;
                _uow.GetRepository<SiteSettings>().Update(settings);
            }

            await _uow.SaveChangesAsync();
            await _uow.CommitTransactionAsync();

            // Invalidate cache
            _cache.Remove(AppConstants.CacheKeys.ActiveTheme);
            _cache.Remove("active_theme_entity");
            _cache.Remove(AppConstants.CacheKeys.SiteSettings);

            return Result.Ok($"'{target.Name}' teması aktif edildi.");
        }
        catch
        {
            await _uow.RollbackTransactionAsync();
            return Result.Fail("Tema değiştirme işlemi başarısız.");
        }
    }
}
