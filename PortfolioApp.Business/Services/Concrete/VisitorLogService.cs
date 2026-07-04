using Microsoft.EntityFrameworkCore;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.Core.Results;
using PortfolioApp.DataAccess.Context;
using PortfolioApp.DTO.DTOs.Dashboard;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.Business.Services.Concrete;

public class VisitorLogService : IVisitorLogService
{
    private readonly PortfolioDbContext _context;

    public VisitorLogService(PortfolioDbContext context)
    {
        _context = context;
    }

    public async Task LogVisitAsync(string ownerId, string? ipAddress, string? userAgent, string? pageUrl, string? referrer, string? sessionId, bool isBot)
    {
        var log = new VisitorLog
        {
            UserId = ownerId,
            IpAddress = ipAddress,
            UserAgent = userAgent?.Length > 500 ? userAgent[..500] : userAgent,
            PageUrl = pageUrl?.Length > 500 ? pageUrl[..500] : pageUrl,
            Referrer = referrer?.Length > 500 ? referrer[..500] : referrer,
            SessionId = sessionId,
            IsBot = isBot,
            VisitedAt = DateTime.UtcNow,
            DeviceType = DetectDeviceType(userAgent),
            Browser = DetectBrowser(userAgent)
        };

        _context.VisitorLogs.Add(log);
        await _context.SaveChangesAsync();
    }

    public async Task<IDataResult<IList<VisitorChartDataDto>>> GetLast30DaysAsync(string ownerId)
    {
        var since = DateTime.UtcNow.AddDays(-29).Date;
        var data = await _context.VisitorLogs
            .Where(v => v.UserId == ownerId && !v.IsBot && v.VisitedAt >= since)
            .GroupBy(v => DateOnly.FromDateTime(v.VisitedAt))
            .Select(g => new VisitorChartDataDto { Date = g.Key, Count = g.Count() })
            .OrderBy(d => d.Date)
            .ToListAsync();

        return DataResult<IList<VisitorChartDataDto>>.Ok(data);
    }

    public async Task<int> GetTodayCountAsync(string ownerId)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return await _context.VisitorLogs
            .CountAsync(v => v.UserId == ownerId && !v.IsBot && DateOnly.FromDateTime(v.VisitedAt) == today);
    }

    public async Task<int> GetTotalCountAsync(string ownerId) =>
        await _context.VisitorLogs.CountAsync(v => v.UserId == ownerId && !v.IsBot);

    private static string? DetectDeviceType(string? userAgent)
    {
        if (string.IsNullOrEmpty(userAgent)) return null;
        if (userAgent.Contains("Mobile", StringComparison.OrdinalIgnoreCase)) return "Mobile";
        if (userAgent.Contains("Tablet", StringComparison.OrdinalIgnoreCase)) return "Tablet";
        return "Desktop";
    }

    private static string? DetectBrowser(string? userAgent)
    {
        if (string.IsNullOrEmpty(userAgent)) return null;
        if (userAgent.Contains("Chrome", StringComparison.OrdinalIgnoreCase)) return "Chrome";
        if (userAgent.Contains("Firefox", StringComparison.OrdinalIgnoreCase)) return "Firefox";
        if (userAgent.Contains("Safari", StringComparison.OrdinalIgnoreCase)) return "Safari";
        if (userAgent.Contains("Edge", StringComparison.OrdinalIgnoreCase)) return "Edge";
        return "Other";
    }
}
