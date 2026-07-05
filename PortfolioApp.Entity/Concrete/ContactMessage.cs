using PortfolioApp.Core.Entities;

namespace PortfolioApp.Entity.Concrete;

public class ContactMessage : BaseEntity, IUserOwnedEntity
{
    public string UserId { get; set; } = string.Empty;

    public string SenderName { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string? SenderPhone { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public bool IsReplied { get; set; }
    public string? ReplyText { get; set; }
    public DateTime? RepliedAt { get; set; }
    public string? IpAddress { get; set; }
    public bool IsSpam { get; set; }
}
