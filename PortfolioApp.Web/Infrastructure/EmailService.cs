using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace PortfolioApp.Web.Infrastructure;

public interface IEmailService
{
    Task SendAsync(string toEmail, string toName, string subject, string htmlBody);
    Task SendContactNotificationAsync(string senderName, string senderEmail, string subject, string message);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendAsync(string toEmail, string toName, string subject, string htmlBody)
    {
        try
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(
                _config["EmailSettings:SenderName"] ?? "Portfolio",
                _config["EmailSettings:SenderEmail"] ?? "noreply@portfolio.com"));
            email.To.Add(new MailboxAddress(toName, toEmail));
            email.Subject = subject;
            email.Body = new TextPart("html") { Text = htmlBody };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                _config["EmailSettings:SmtpHost"] ?? "localhost",
                _config.GetValue<int>("EmailSettings:SmtpPort", 587),
                _config.GetValue<bool>("EmailSettings:UseSsl") ? SecureSocketOptions.StartTls : SecureSocketOptions.None);

            var username = _config["EmailSettings:Username"];
            var password = _config["EmailSettings:Password"] ?? "";
            if (!string.IsNullOrEmpty(username))
                await smtp.AuthenticateAsync(username, password);

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "E-posta gönderilemedi. To: {Email}, Subject: {Subject}", toEmail, subject);
        }
    }

    public async Task SendContactNotificationAsync(string senderName, string senderEmail, string subject, string message)
    {
        var adminEmail = _config["AdminSettings:Email"] ?? "admin@portfolio.com";
        var htmlBody = $"""
            <h2>Yeni İletişim Mesajı</h2>
            <p><strong>Gönderen:</strong> {senderName} &lt;{senderEmail}&gt;</p>
            <p><strong>Konu:</strong> {subject}</p>
            <hr/>
            <p>{message.Replace("\n", "<br/>")}</p>
            """;

        await SendAsync(adminEmail, "Admin", $"[Portfolio] {subject}", htmlBody);
    }
}
