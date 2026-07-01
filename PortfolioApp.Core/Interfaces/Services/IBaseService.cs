using PortfolioApp.Core.Results;

namespace PortfolioApp.Core.Interfaces.Services;

public interface IBaseService<TDto, TCreateDto, TUpdateDto>
{
    Task<IDataResult<TDto>> GetByIdAsync(int id);
    Task<IDataResult<IList<TDto>>> GetAllAsync();
    Task<IDataResult<IList<TDto>>> GetAllActiveAsync();
    Task<IResult> CreateAsync(TCreateDto dto);
    Task<IResult> UpdateAsync(int id, TUpdateDto dto);
    Task<IResult> DeleteAsync(int id);
    Task<IResult> ToggleActiveAsync(int id);
}
