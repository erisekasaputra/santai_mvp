using FileHub.API.Abstraction;
using FileHub.API.Configurations;  
using Microsoft.Extensions.Options;
using Minio; 
using Minio.DataModel.Args;
using System.Net;
using System.Security.Cryptography;
using System.Text;
namespace FileHub.API.Infrastructure;

public class MinioStorageService : IStorageService
{
    private IOptionsMonitor<StorageConfigs> _config;

    private IOptionsMonitor<KmsConfigs> _kmsConfig;

    private readonly IMinioClient _client;

    private readonly string _bucketName;
    public MinioStorageService(IOptionsMonitor<StorageConfigs> config, IOptionsMonitor<KmsConfigs> kmsConfig)
    {
        _config = config; 
        _kmsConfig = kmsConfig;
        _client = new MinioClient()
            .WithEndpoint(config.CurrentValue.ServiceUrl)
            .WithCredentials(config.CurrentValue.AccessKey, config.CurrentValue.SecretKey)
            .Build(); 
        _bucketName = config.CurrentValue.BucketName ?? string.Empty;
    }
    public async Task DeleteFileAsync(string objectName)
    {
        string serviceUrl = _config.CurrentValue.ServiceUrl ?? string.Empty;
        string bucketName = _config.CurrentValue.BucketName ?? string.Empty;

        ArgumentNullException.ThrowIfNull(serviceUrl, nameof(serviceUrl));  
        ArgumentNullException.ThrowIfNull(bucketName, nameof(bucketName));  
        ArgumentNullException.ThrowIfNull(objectName, nameof(objectName));  

        var objectArgument = new RemoveObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName);

        await _client.RemoveObjectAsync(objectArgument).ConfigureAwait(false); 
    } 

    public async Task<string> UploadFileAsync(string objectName, IFormFile file)
    {
        var hash = Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(_kmsConfig.CurrentValue.SecretKey)));

        var putObjectArgs = new PutObjectArgs()
            .WithBucket(_bucketName)
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
    
    public async Task<bool> IsBucketExistsAsync()
    {
        var bucketExistsArgs = new BucketExistsArgs()
            .WithBucket(_bucketName);

        var result = await _client.BucketExistsAsync(bucketExistsArgs).ConfigureAwait(false);

        return result;
    }

    public async Task CreateBucketAsync()
    {
        var bucketCreate = new MakeBucketArgs()
            .WithBucket(_bucketName);

        await _client.MakeBucketAsync(bucketCreate).ConfigureAwait(false); 
    }

    public async Task<byte[]> GetFileAsync(string objectName)
    { 
        using var stream = new MemoryStream();
        var getArgs = new GetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName)
            .WithCallbackStream(outStream => outStream.CopyTo(stream));
        
        await _client.GetObjectAsync(getArgs).ConfigureAwait(false);

        return stream.ToArray();
    }
}
