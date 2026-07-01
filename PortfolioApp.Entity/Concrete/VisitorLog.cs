namespace PortfolioApp.Entity.Concrete;

public class VisitorLog
{
    public int Id { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? Browser { get; set; }
    public string? Os { get; set; }
    public string? DeviceType { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? PageUrl { get; set; }
    public string? Referrer { get; set; }
    public DateTime VisitedAt { get; set; } = DateTime.UtcNow;
    public bool IsBot { get; set; }
    public string? SessionId { get; set; }
}
