namespace PortfolioApp.Core.Utilities;

public static class PaginationHelper
{
    public static int CalculateTotalPages(int totalCount, int pageSize)
    {
        if (pageSize <= 0) return 0;
        return (int)Math.Ceiling((double)totalCount / pageSize);
    }

    public static int NormalizePage(int page, int totalPages)
    {
        if (page < 1) return 1;
        if (totalPages > 0 && page > totalPages) return totalPages;
        return page;
    }

    public static int CalculateReadingTime(string htmlContent, int wordsPerMinute = 200)
    {
        if (string.IsNullOrWhiteSpace(htmlContent)) return 1;
        var text = System.Text.RegularExpressions.Regex.Replace(htmlContent, "<[^>]+>", " ");
        var wordCount = text.Split([' ', '\n', '\r', '\t'], StringSplitOptions.RemoveEmptyEntries).Length;
        return Math.Max(1, (int)Math.Ceiling((double)wordCount / wordsPerMinute));
    }
}
