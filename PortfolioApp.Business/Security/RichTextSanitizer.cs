using Ganss.Xss;

namespace PortfolioApp.Business.Security;

public static class RichTextSanitizer
{
    // TinyMCE admin editors persist this HTML and it's later rendered with @Html.Raw
    // to public visitors, so it must be stripped of scripts/handlers before saving.
    private static readonly HtmlSanitizer Sanitizer = CreateSanitizer();

    public static string Sanitize(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return string.Empty;

        return Sanitizer.Sanitize(html);
    }

    private static HtmlSanitizer CreateSanitizer()
    {
        var sanitizer = new HtmlSanitizer();
        sanitizer.AllowedSchemes.Clear();
        sanitizer.AllowedSchemes.Add("http");
        sanitizer.AllowedSchemes.Add("https");
        sanitizer.AllowedSchemes.Add("mailto");
        return sanitizer;
    }
}
