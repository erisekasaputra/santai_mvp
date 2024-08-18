using FileHub.API.Abstraction;
using FileHub.API.Configurations;
using FileHub.API.Utilities;
using Microsoft.Extensions.Options;
using Minio; 
using Minio.DataModel.Args;  
namespace FileHub.API.Infrastructure;

public class MinioStorageService : IStorageService
{
    private IOptionsMonitor<StorageConfigs> _config;

    private IOptionsMonitor<KmsConfigs> _kmsConfig;

    private readonly IMinioClient _client;

    private readonly string _bucketPrivate; 
    private readonly string _bucketPublic; 

    public MinioStorageService(IOptionsMonitor<StorageConfigs> config, IOptionsMonitor<KmsConfigs> kmsConfig)
    {
        _config = config; 
        _kmsConfig = kmsConfig;
        _client = new MinioClient()
            .WithEndpoint(config.CurrentValue.ServiceUrl)
            .WithCredentials(config.CurrentValue.AccessKey, config.CurrentValue.SecretKey)
            .Build(); 
        _bucketPrivate = config.CurrentValue.BucketPrivate ?? string.Empty;
        _bucketPublic = config.CurrentValue.BucketPublic ?? string.Empty;
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
        //var hash = Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(_kmsConfig.CurrentValue.SecretKey)));

        var putObjectArgs = new PutObjectArgs()
            .WithBucket(_bucketPublic)
            .WithObject(objectName)
            .WithStreamData(file.OpenReadStream())
            .WithContentType(file.ContentType)
            .WithObjectSize(file.Length);
            //.WithHeaders(new Dictionary<string, string>
            //{
            //    { "X-Amz-Server-Side-Encryption-Customer-Algorithm", "AES256" },
            //    { "X-Amz-Server-Side-Encryption-Customer-Key", _kmsConfig.CurrentValue.SecretKey },
            //    { "X-Amz-Server-Side-Encryption-Customer-Key-MD5", hash }
            //});

        await _client.PutObjectAsync(putObjectArgs).ConfigureAwait(false); 

        return objectName;
    }

    public async Task<string> UploadFilePrivateAsync(string objectName, IFormFile file)
    {
        //var hash = Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(_kmsConfig.CurrentValue.SecretKey)));

        var putObjectArgs = new PutObjectArgs()
            .WithBucket(_bucketPrivate)
            .WithObject(objectName)
            .WithStreamData(file.OpenReadStream())
            .WithContentType(file.ContentType)
            .WithObjectSize(file.Length);
        //.WithHeaders(new Dictionary<string, string>
        //{
        //    { "X-Amz-Server-Side-Encryption-Customer-Algorithm", "AES256" },
        //    { "X-Amz-Server-Side-Encryption-Customer-Key", _kmsConfig.CurrentValue.SecretKey },
        //    { "X-Amz-Server-Side-Encryption-Customer-Key-MD5", hash }
        //});

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
        string url = $"http://{_config.CurrentValue.ServiceUrl.RemovePrefixProcotol()}/{_bucketPublic}/{resourceName}"; 

        if (isUsingCdn)
        {
            url = $"http://{_config.CurrentValue.CdnServiceUrl.RemovePrefixProcotol()}/{resourceName}";
        }   
        
        return await Task.FromResult(url);
    } 
}
