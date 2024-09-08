using Amazon.S3.Model;
using Amazon.S3;
using Microsoft.Extensions.Options; 
using FileHub.API.Services.Interfaces;
using Core.Configurations;
using Core.Utilities;

namespace FileHub.API.Services;

public class AwsStorageService : IStorageService
{
    private readonly IOptionsMonitor<StorageConfiguration> _config;
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketPrivate;
    private readonly string _bucketPublic;

    public AwsStorageService(IOptionsMonitor<StorageConfiguration> config)
    {
        _config = config;

        // Initialize the S3 client with region and credentials
        _s3Client = new AmazonS3Client(
            _config.CurrentValue.AccessKey,
            _config.CurrentValue.SecretKey,
            Amazon.RegionEndpoint.GetBySystemName(_config.CurrentValue.Region)
        );

        _bucketPrivate = _config.CurrentValue.BucketPrivate ?? string.Empty;
        _bucketPublic = _config.CurrentValue.BucketPublic ?? string.Empty;
    }

    public async Task DeleteFilePrivateAsync(string objectName)
    {
        var deleteObjectRequest = new DeleteObjectRequest
        {
            BucketName = _bucketPrivate,
            Key = objectName
        };

        await _s3Client.DeleteObjectAsync(deleteObjectRequest);
    }

    public async Task DeleteFilePublicAsync(string objectName)
    {
        var deleteObjectRequest = new DeleteObjectRequest
        {
            BucketName = _bucketPublic,
            Key = objectName
        };

        await _s3Client.DeleteObjectAsync(deleteObjectRequest);
    }

    public async Task<string> UploadFilePublicAsync(string objectName, IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = _bucketPublic,
            Key = objectName,
            InputStream = stream,
            ContentType = file.ContentType
        };

        await _s3Client.PutObjectAsync(putObjectRequest);
        return objectName;
    }

    public async Task<string> UploadFilePrivateAsync(string objectName, IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = _bucketPrivate,
            Key = objectName,
            InputStream = stream,
            ContentType = file.ContentType
        };

        await _s3Client.PutObjectAsync(putObjectRequest);
        return objectName;
    }

    public async Task<bool> IsBucketPrivateExistsAsync()
    {
        return await DoesS3BucketExistAsync(_bucketPrivate);
    }

    public async Task<bool> IsBucketPublicExistsAsync()
    {
        return await DoesS3BucketExistAsync(_bucketPublic);
    }

    private async Task<bool> DoesS3BucketExistAsync(string bucketName)
    {
        try
        {
            var response = await _s3Client.ListBucketsAsync();
            return response.Buckets.Any(b => b.BucketName == bucketName);
        }
        catch (Exception ex)
        {
            // Handle the exception or log it
            throw new Exception($"Error checking bucket existence: {ex.Message}", ex);
        }
    }

    public async Task<byte[]> GetFilePrivateAsync(string objectName)
    {
        return await GetFileAsync(_bucketPrivate, objectName);
    }

    public async Task<byte[]> GetFilePublicAsync(string objectName)
    {
        return await GetFileAsync(_bucketPublic, objectName);
    }

    private async Task<byte[]> GetFileAsync(string bucketName, string objectName)
    {
        var getObjectRequest = new GetObjectRequest
        {
            BucketName = bucketName,
            Key = objectName
        };

        using var response = await _s3Client.GetObjectAsync(getObjectRequest);
        using var stream = new MemoryStream();
        await response.ResponseStream.CopyToAsync(stream);
        return stream.ToArray();
    }

    public async Task<string> GeneratePublicObjectUrl(string resourceName, bool isUsingCdn = false)
    {
        string url = $"https://{_bucketPublic}.s3.{_config.CurrentValue.Region}.amazonaws.com/{resourceName}";
        if (isUsingCdn)
        {
            url = $"https://{_config.CurrentValue.CdnServiceUrl.RemovePrefixProcotol()}/{resourceName}";
        }

        return await Task.FromResult(url);
    }
}
