using FileHub.API.Abstraction;

namespace FileHub.API.Infrastructure;

public class AwsStorageService : IStorageService
{
    public Task CreateBucketAsync()
    {
        throw new NotImplementedException();
    }

    public Task DeleteFileAsync(string objectName)
    {
        throw new NotImplementedException();
    }

    public Task<byte[]> GetFileAsync(string objectName)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsBucketExistsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<string> UploadFileAsync(string objectName, IFormFile file)
    {
        throw new NotImplementedException();
    }
}
