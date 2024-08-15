namespace FileHub.API.Abstraction;

public interface IStorageService
{
    Task<string> UploadFileAsync(string objectName, IFormFile file); 
    Task DeleteFileAsync(string objectName); 
    Task CreateBucketAsync();
    Task<bool> IsBucketExistsAsync(); 
    Task<byte[]> GetFileAsync(string objectName);  
}
