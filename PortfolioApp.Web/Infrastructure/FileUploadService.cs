using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace PortfolioApp.Web.Infrastructure;

public interface IFileUploadService
{
    Task<string> UploadImageAsync(IFormFile file, string folder, int maxWidth = 1200);
    Task<string> UploadDocumentAsync(IFormFile file, string folder);
    void DeleteFile(string? relativePath);
}

public class FileUploadService : IFileUploadService
{
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _config;
    private readonly long _maxFileSize;
    private readonly string[] _allowedImageExts;
    private readonly string[] _allowedDocExts;

    public FileUploadService(IWebHostEnvironment env, IConfiguration config)
    {
        _env = env;
        _config = config;
        _maxFileSize = config.GetValue<long>("FileUpload:MaxFileSizeBytes", 5_242_880);
        _allowedImageExts = config.GetSection("FileUpload:AllowedImageExtensions")
            .Get<string[]>() ?? [".jpg", ".jpeg", ".png", ".gif", ".webp"];
        _allowedDocExts = config.GetSection("FileUpload:AllowedDocExtensions")
            .Get<string[]>() ?? [".pdf", ".doc", ".docx"];
    }

    public async Task<string> UploadImageAsync(IFormFile file, string folder, int maxWidth = 1200)
    {
        ValidateFile(file, _allowedImageExts);

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid():N}{ext}";
        var uploadDir = Path.Combine(_env.WebRootPath, "uploads", folder);
        Directory.CreateDirectory(uploadDir);

        var fullPath = Path.Combine(uploadDir, fileName);

        using var image = await Image.LoadAsync(file.OpenReadStream());
        if (image.Width > maxWidth)
            image.Mutate(x => x.Resize(maxWidth, 0));

        await image.SaveAsync(fullPath);

        return $"/uploads/{folder}/{fileName}";
    }

    public async Task<string> UploadDocumentAsync(IFormFile file, string folder)
    {
        ValidateFile(file, _allowedDocExts);

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid():N}{ext}";
        var uploadDir = Path.Combine(_env.WebRootPath, "uploads", folder);
        Directory.CreateDirectory(uploadDir);

        var fullPath = Path.Combine(uploadDir, fileName);
        await using var stream = File.Create(fullPath);
        await file.CopyToAsync(stream);

        return $"/uploads/{folder}/{fileName}";
    }

    public void DeleteFile(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return;

        // Prevent path traversal
        var normalized = relativePath.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(_env.WebRootPath, normalized);

        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }

    private void ValidateFile(IFormFile file, string[] allowedExts)
    {
        if (file.Length > _maxFileSize)
            throw new InvalidOperationException($"Dosya boyutu {_maxFileSize / 1024 / 1024} MB'ı geçemez.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExts.Contains(ext))
            throw new InvalidOperationException($"İzin verilmeyen dosya türü: {ext}");
    }
}
