using PortfolioApp.Core.Results;

namespace PortfolioApp.DTO.ViewModels.Admin.Shared;

public class PaginatedListViewModel<T>
{
    public PaginatedResult<T> Data { get; set; } = new();
    public string? SearchQuery { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
}
