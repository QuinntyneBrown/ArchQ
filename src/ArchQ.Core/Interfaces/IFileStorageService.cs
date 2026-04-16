namespace ArchQ.Core.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadAsync(string storageKey, Stream content, string contentType);
    Task<Stream> DownloadAsync(string storageKey);
    Task DeleteAsync(string storageKey);
}
