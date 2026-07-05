using Microsoft.EntityFrameworkCore;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.Core.Interfaces;
using PortfolioApp.Core.Results;
using PortfolioApp.DataAccess.UnitOfWork;
using PortfolioApp.DTO.DTOs.Dashboard;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.Business.Services.Concrete;

public class DashboardService : IDashboardService
{
    private readonly UnitOfWork _uow;
    private readonly IThemeService _themeService;
    private readonly ICurrentUserService _currentUser;

    public DashboardService(UnitOfWork uow, IThemeService themeService, ICurrentUserService currentUser)
    {
        _uow = uow;
        _themeService = themeService;
        _currentUser = currentUser;
    }

    public async Task<IDataResult<DashboardStatsDto>> GetStatsAsync()
    {
        var ownerId = _currentUser.RequireUserId();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var last30Days = DateTime.UtcNow.AddDays(-29).Date;

        var totalProjects = await _uow.GetRepository<Project>().CountAsync(p => p.UserId == ownerId);
        var totalBlogPosts = await _uow.GetRepository<BlogPost>().CountAsync(p => p.AuthorId == ownerId && p.IsPublished);
        var totalMessages = await _uow.GetRepository<ContactMessage>().CountAsync(m => m.UserId == ownerId);
        var unreadMessages = await _uow.GetRepository<ContactMessage>().CountAsync(m => m.UserId == ownerId && !m.IsRead && !m.IsSpam);
        var pendingComments = await _uow.GetRepository<Comment>().CountAsync(c => c.UserId == ownerId && !c.IsApproved);
        var totalSkills = await _uow.GetRepository<Skill>().CountAsync(s => s.UserId == ownerId && s.IsActive);
        var totalVisitors = await _uow.Context.VisitorLogs.Where(v => v.UserId == ownerId && !v.IsBot).CountAsync();
        var todayVisitors = await _uow.Context.VisitorLogs.Where(v => v.UserId == ownerId && !v.IsBot && DateOnly.FromDateTime(v.VisitedAt) == today).CountAsync();

        var chartData = await _uow.Context.VisitorLogs
            .Where(v => v.UserId == ownerId && !v.IsBot && v.VisitedAt >= last30Days)
            .GroupBy(v => DateOnly.FromDateTime(v.VisitedAt))
            .Select(g => new VisitorChartDataDto { Date = g.Key, Count = g.Count() })
            .OrderBy(d => d.Date)
            .ToListAsync();

        var recentMessages = await _uow.GetRepository<ContactMessage>().GetQueryable()
            .Where(m => m.UserId == ownerId && !m.IsSpam && !m.IsDeleted)
            .OrderByDescending(m => m.CreatedAt)
            .Take(5)
            .Select(m => new RecentMessageDto { Id = m.Id, SenderName = m.SenderName, Subject = m.Subject, CreatedAt = m.CreatedAt, IsRead = m.IsRead })
            .ToListAsync();

        var recentProjects = await _uow.GetRepository<Project>().GetQueryable()
            .Where(p => p.UserId == ownerId && !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .Take(5)
            .Select(p => new RecentProjectDto { Id = p.Id, Title = p.Title, CoverImageUrl = p.CoverImageUrl, CreatedAt = p.CreatedAt })
            .ToListAsync();

        var activeThemeResult = await _themeService.GetActiveThemeAsync();

        var stats = new DashboardStatsDto
        {
            TotalProjects = totalProjects,
            TotalBlogPosts = totalBlogPosts,
            TotalMessages = totalMessages,
            UnreadMessages = unreadMessages,
            PendingComments = pendingComments,
            TotalSkills = totalSkills,
            TotalVisitors = totalVisitors,
            TodayVisitors = todayVisitors,
            ActiveThemeName = activeThemeResult.Data?.Name,
            Last30DaysVisitors = chartData,
            RecentMessages = recentMessages,
            RecentProjects = recentProjects
        };

        return DataResult<DashboardStatsDto>.Ok(stats);
    }
}
