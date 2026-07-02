namespace PortfolioApp.Core.Utilities;

public static class FileHelper
{
    // .svg intentionally excluded — SVG can carry <script>/event-handler payloads (stored XSS via upload).
    private static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp"];
    private static readonly string[] AllowedDocumentExtensions = [".pdf", ".doc", ".docx"];

    public static string GetUniqueFileName(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        return $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
    }

    public static bool IsImageFile(string fileName)
    {
        var extension = Path.GetExtension(fileName)?.ToLowerInvariant();
        return extension != null && AllowedImageExtensions.Contains(extension);
    }

    public static bool IsDocumentFile(string fileName)
    {
        var extension = Path.GetExtension(fileName)?.ToLowerInvariant();
        return extension != null && AllowedDocumentExtensions.Contains(extension);
    }

    public static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        return string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
    }
}
