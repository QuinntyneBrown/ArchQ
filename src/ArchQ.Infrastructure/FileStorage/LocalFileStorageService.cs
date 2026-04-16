using ArchQ.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ArchQ.Infrastructure.FileStorage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;

    public LocalFileStorageService(IConfiguration configuration)
    {
        _basePath = configuration.GetValue<string>("FileStorage:BasePath") ?? Path.Combine(Directory.GetCurrentDirectory(), "storage");
    }

    public async Task<string> UploadAsync(string storageKey, Stream content, string contentType)
    {
        var filePath = GetFullPath(storageKey);
        var directory = Path.GetDirectoryName(filePath)!;

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await content.CopyToAsync(fileStream);

        return storageKey;
    }

    public Task<Stream> DownloadAsync(string storageKey)
    {
        var filePath = GetFullPath(storageKey);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {storageKey}");
        }

        Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return Task.FromResult(stream);
    }

    public Task DeleteAsync(string storageKey)
    {
        var filePath = GetFullPath(storageKey);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }

    private string GetFullPath(string storageKey)
    {
        // Prevent directory traversal
        var normalized = storageKey.Replace('\\', '/');
        if (normalized.Contains(".."))
        {
            throw new InvalidOperationException("Invalid storage key.");
        }

        return Path.Combine(_basePath, normalized);
    }
}
