using FitnessAPI.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace FitnessAPI.Infrastructure.Services;

public class FileService : IFileService
{
    private readonly IConfiguration _configuration;
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
    private readonly long _maxFileSizeBytes;

    public FileService(IConfiguration configuration)
    {
        _configuration = configuration;
        var maxMb = int.Parse(configuration["FileSettings:MaxFileSizeMB"] ?? "10");
        _maxFileSizeBytes = maxMb * 1024 * 1024;
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string folder, CancellationToken cancellationToken = default)
    {
        var uploadPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            _configuration["FileSettings:UploadPath"] ?? "wwwroot/uploads",
            folder
        );

        Directory.CreateDirectory(uploadPath);

        var extension = Path.GetExtension(fileName).ToLower();
        var newFileName = $"{Guid.NewGuid()}{extension}";
        var fullPath = Path.Combine(uploadPath, newFileName);

        await using var fileStream2 = File.Create(fullPath);
        await fileStream.CopyToAsync(fileStream2, cancellationToken);

        return $"/uploads/{folder}/{newFileName}";
    }

    public async Task DeleteAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(fileUrl)) return;

        var relativePath = fileUrl.TrimStart('/');
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

        if (File.Exists(fullPath))
            await Task.Run(() => File.Delete(fullPath), cancellationToken);
    }

    public bool IsValidFileExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLower();
        return _allowedExtensions.Contains(extension);
    }

    public bool IsValidFileSize(long fileSize) => fileSize <= _maxFileSizeBytes;
}
