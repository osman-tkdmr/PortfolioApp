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
        await ValidateDocumentSignatureAsync(file, ext);

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

        var uploadsRoot = Path.GetFullPath(Path.Combine(_env.WebRootPath, "uploads") + Path.DirectorySeparatorChar);
        var normalized = relativePath.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.GetFullPath(Path.Combine(_env.WebRootPath, normalized));

        // Reject anything that escapes the uploads directory (e.g. via "..") before touching the filesystem
        if (!fullPath.StartsWith(uploadsRoot, StringComparison.Ordinal))
            throw new InvalidOperationException("Geçersiz dosya yolu.");

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

    // Extension whitelist alone trusts the client-supplied filename; check the actual
    // file signature so a renamed arbitrary payload can't pass as a .pdf/.doc/.docx.
    private static async Task ValidateDocumentSignatureAsync(IFormFile file, string ext)
    {
        var header = new byte[8];
        await using (var stream = file.OpenReadStream())
        {
            var read = await stream.ReadAsync(header.AsMemory(0, header.Length));
            if (read < header.Length)
                Array.Clear(header, read, header.Length - read);
        }

        var isValid = ext switch
        {
            ".pdf" => header.AsSpan(0, 5).SequenceEqual("%PDF-"u8),
            ".doc" => header.AsSpan(0, 8).SequenceEqual(new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 }),
            ".docx" => header.AsSpan(0, 4).SequenceEqual(new byte[] { 0x50, 0x4B, 0x03, 0x04 }),
            _ => false
        };

        if (!isValid)
            throw new InvalidOperationException("Dosya içeriği beklenen türle eşleşmiyor.");
    }
}
