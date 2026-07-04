using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.Core.Constants;
using PortfolioApp.Core.Interfaces;
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
    private readonly ICurrentUserService _currentUser;

    public ThemeService(UnitOfWork uow, IMapper mapper, IMemoryCache cache, ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _cache = cache;
        _currentUser = currentUser;
    }

    // Admin-only (Dashboard) — resolves the current tenant's own selected theme via their SiteSettings.
    public async Task<IDataResult<ThemeDto?>> GetActiveThemeAsync()
    {
        var cacheKey = $"active_theme_entity:{_currentUser.UserId}";
        var theme = await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
            var settings = await _uow.GetRepository<SiteSettings>().FirstOrDefaultAsync(s => s.UserId == _currentUser.UserId);
            if (settings?.ActiveThemeId is null) return null;
            return await _uow.GetRepository<Theme>().FirstOrDefaultAsync(t => t.Id == settings.ActiveThemeId && t.IsActive);
        });

        return DataResult<ThemeDto?>.Ok(_mapper.Map<ThemeDto?>(theme));
    }

    public async Task<IDataResult<IList<ThemeDto>>> GetAllAsync()
    {
        var themes = await _uow.GetRepository<Theme>().GetAllAsync();
        return DataResult<IList<ThemeDto>>.Ok(_mapper.Map<IList<ThemeDto>>(themes));
    }

    // Themes are shared/global reference data (not user-owned) — "activating" one for a tenant
    // only ever changes THAT tenant's own SiteSettings.ActiveThemeId, never the shared Theme rows
    // themselves (doing so would silently switch every other tenant's selected theme too).
    public async Task<IResult> ActivateThemeAsync(int themeId)
    {
        var target = await _uow.GetRepository<Theme>().FirstOrDefaultAsync(t => t.Id == themeId && t.IsActive);
        if (target is null)
            return Result.Fail("Tema bulunamadı.");

        var settings = await _uow.GetRepository<SiteSettings>().FirstOrDefaultAsync(s => s.UserId == _currentUser.UserId);
        if (settings is null)
            return Result.Fail("Site ayarları bulunamadı.");

        settings.ActiveThemeId = themeId;
        _uow.GetRepository<SiteSettings>().Update(settings);
        await _uow.SaveChangesAsync();

        _cache.Remove($"{AppConstants.CacheKeys.ActiveTheme}:{_currentUser.UserId}");
        _cache.Remove($"active_theme_entity:{_currentUser.UserId}");
        _cache.Remove($"{AppConstants.CacheKeys.SiteSettings}:{_currentUser.UserId}");

        return Result.Ok($"'{target.Name}' teması aktif edildi.");
    }
}
