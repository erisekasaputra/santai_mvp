
using Core.Configurations;
using Core.Utilities;
using FileHub.API.Services.Interfaces; 
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace FileHub.API.Services;
public class MinioStorageService : IStorageService
{
    private IOptionsMonitor<StorageConfiguration> _storageConfig;  
    private IOptionsMonitor<MinioConfiguration> _minioConfig;  
    private readonly IMinioClient _client;

    private readonly string _bucketPrivate;
    private readonly string _bucketPublic;

    public MinioStorageService(
        IOptionsMonitor<StorageConfiguration> storageConfig,
        IOptionsMonitor<MinioConfiguration> minioConfig)
    {
        _storageConfig = storageConfig;
        _minioConfig = minioConfig;

        _client = new MinioClient()
            .WithEndpoint(_minioConfig.CurrentValue.ServiceUrl)
            .WithCredentials(_minioConfig.CurrentValue.AccessKey, _minioConfig.CurrentValue.SecretKey)
            .Build();
        _bucketPrivate = _storageConfig.CurrentValue.BucketPrivate ?? string.Empty;
        _bucketPublic = _storageConfig.CurrentValue.BucketPublic ?? string.Empty;
    }
    public async Task DeleteFilePrivateAsync(string objectName)
    {
        var objectArgument = new RemoveObjectArgs()
            .WithBucket(_bucketPrivate)
            .WithObject(objectName);

        await _client.RemoveObjectAsync(objectArgument).ConfigureAwait(false);
    }

    public async Task DeleteFilePublicAsync(string objectName)
    {
        var objectArgument = new RemoveObjectArgs()
            .WithBucket(_bucketPublic)
            .WithObject(objectName);

        await _client.RemoveObjectAsync(objectArgument).ConfigureAwait(false);
    }

    public async Task<string> UploadFilePublicAsync(string objectName, IFormFile file)
    { 
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(_bucketPublic)
            .WithObject(objectName)
            .WithStreamData(file.OpenReadStream())
            .WithContentType(file.ContentType)
            .WithObjectSize(file.Length); 

        await _client.PutObjectAsync(putObjectArgs).ConfigureAwait(false);

        return objectName;
    }

    public async Task<string> UploadFilePrivateAsync(string objectName, IFormFile file)
    { 
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(_bucketPrivate)
            .WithObject(objectName)
            .WithStreamData(file.OpenReadStream())
            .WithContentType(file.ContentType)
            .WithObjectSize(file.Length); 

        await _client.PutObjectAsync(putObjectArgs).ConfigureAwait(false);

        return objectName;
    }

    public async Task<bool> IsBucketPrivateExistsAsync()
    {
        var bucketExistsArgs = new BucketExistsArgs()
            .WithBucket(_bucketPrivate);

        var result = await _client.BucketExistsAsync(bucketExistsArgs).ConfigureAwait(false);

        return result;
    }

    public async Task<bool> IsBucketPublicExistsAsync()
    {
        var bucketExistsArgs = new BucketExistsArgs()
            .WithBucket(_bucketPublic);

        var result = await _client.BucketExistsAsync(bucketExistsArgs).ConfigureAwait(false);

        return result;
    }


    public async Task<byte[]> GetFilePrivateAsync(string objectName)
    {
        using var stream = new MemoryStream();
        var getArgs = new GetObjectArgs()
            .WithBucket(_bucketPrivate)
            .WithObject(objectName)
            .WithCallbackStream(outStream => outStream.CopyTo(stream));

        await _client.GetObjectAsync(getArgs).ConfigureAwait(false);

        return stream.ToArray();
    }

    public async Task<byte[]> GetFilePublicAsync(string objectName)
    {
        using var stream = new MemoryStream();
        var getArgs = new GetObjectArgs()
            .WithBucket(_bucketPublic)
            .WithObject(objectName)
            .WithCallbackStream(outStream => outStream.CopyTo(stream));

        await _client.GetObjectAsync(getArgs).ConfigureAwait(false);

        return stream.ToArray();
    }

    public async Task<string> GeneratePublicObjectUrl(string resourceName, bool isUsingCdn = false)
    {
        string url = $"http://{_minioConfig.CurrentValue.ServiceUrl.RemovePrefixProcotol()}/{_bucketPublic}/{resourceName}";

        if (isUsingCdn)
        {
            url = $"http://{_storageConfig.CurrentValue.CdnServiceUrl.RemovePrefixProcotol()}/{resourceName}";
        }

        return await Task.FromResult(url);
    }
}
