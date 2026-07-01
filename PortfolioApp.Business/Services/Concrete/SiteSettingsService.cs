using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.Core.Constants;
using PortfolioApp.Core.Results;
using PortfolioApp.DataAccess.UnitOfWork;
using PortfolioApp.DTO.DTOs.Site;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.Business.Services.Concrete;

public class SiteSettingsService : ISiteSettingsService
{
    private readonly UnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;

    public SiteSettingsService(UnitOfWork uow, IMapper mapper, IMemoryCache cache)
    {
        _uow = uow;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<IDataResult<SiteSettingsDto>> GetAsync()
    {
        var settings = await _cache.GetOrCreateAsync(AppConstants.CacheKeys.SiteSettings, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
            return await _uow.GetRepository<SiteSettings>().FirstOrDefaultAsync(_ => true);
        });

        return settings is null
            ? DataResult<SiteSettingsDto>.Fail("Site ayarları bulunamadı.")
            : DataResult<SiteSettingsDto>.Ok(_mapper.Map<SiteSettingsDto>(settings));
    }

    public async Task<IResult> UpdateAsync(SiteSettingsUpdateDto dto)
    {
        var settings = await _uow.GetRepository<SiteSettings>().GetByIdAsync(dto.Id);
        if (settings is null)
            return Result.Fail("Site ayarları bulunamadı.");

        _mapper.Map(dto, settings);
        _uow.GetRepository<SiteSettings>().Update(settings);
        await _uow.SaveChangesAsync();

        _cache.Remove(AppConstants.CacheKeys.SiteSettings);

        return Result.Ok("Site ayarları güncellendi.");
    }
}
