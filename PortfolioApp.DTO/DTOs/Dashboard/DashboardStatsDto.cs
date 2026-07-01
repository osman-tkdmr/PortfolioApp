namespace PortfolioApp.DTO.DTOs.Dashboard;

public class DashboardStatsDto
{
    public int TotalProjects { get; set; }
    public int TotalBlogPosts { get; set; }
    public int TotalMessages { get; set; }
    public int UnreadMessages { get; set; }
    public int TotalVisitors { get; set; }
    public int TodayVisitors { get; set; }
    public int PendingComments { get; set; }
    public int TotalSkills { get; set; }
    public string? ActiveThemeName { get; set; }
    public IList<VisitorChartDataDto> Last30DaysVisitors { get; set; } = [];
    public IList<RecentMessageDto> RecentMessages { get; set; } = [];
    public IList<RecentProjectDto> RecentProjects { get; set; } = [];
}

public class VisitorChartDataDto
{
    public DateOnly Date { get; set; }
    public int Count { get; set; }
}

public class RecentMessageDto
{
    public int Id { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}

public class RecentProjectDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
