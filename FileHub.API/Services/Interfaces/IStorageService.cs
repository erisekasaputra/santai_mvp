namespace FileHub.API.Services.Interfaces;

public interface IStorageService
{
    Task DeleteFilePrivateAsync(string objectName);
    Task DeleteFilePublicAsync(string objectName);
    Task<string> UploadFilePublicAsync(string objectName, IFormFile file);
    Task<string> UploadFilePrivateAsync(string objectName, IFormFile file);
    Task<bool> IsBucketPrivateExistsAsync();
    Task<bool> IsBucketPublicExistsAsync();
    Task<byte[]> GetFilePrivateAsync(string objectName);
    Task<byte[]> GetFilePublicAsync(string objectName);
    Task<string> GeneratePublicObjectUrl(string resourceName, bool isUsingCdn = false);
}
