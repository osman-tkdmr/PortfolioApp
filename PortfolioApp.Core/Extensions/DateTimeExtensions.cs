namespace PortfolioApp.Core.Extensions;

public static class DateTimeExtensions
{
    public static string ToRelativeTime(this DateTime dateTime)
    {
        var diff = DateTime.UtcNow - dateTime.ToUniversalTime();

        return diff.TotalSeconds switch
        {
            < 60 => "Az önce",
            < 3600 => $"{(int)diff.TotalMinutes} dakika önce",
            < 86400 => $"{(int)diff.TotalHours} saat önce",
            < 2592000 => $"{(int)diff.TotalDays} gün önce",
            < 31536000 => $"{(int)(diff.TotalDays / 30)} ay önce",
            _ => $"{(int)(diff.TotalDays / 365)} yıl önce"
        };
    }

    public static string ToTurkishDate(this DateTime dateTime) =>
        dateTime.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("tr-TR"));

    public static string ToTurkishDate(this DateOnly date) =>
        date.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("tr-TR"));
}
