using Core.Messages;
using Core.Results;
using Core.Utilities;
using Core.Validations;
using FileHub.API.SeedWork;
using FileHub.API.Services.Interfaces; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Minio.Exceptions;

namespace FileHub.API.Controllers;


[Route("/api/v1/[controller]")]
[ApiController]
public class FilesController
{
    private readonly IStorageService _storageService;
    private readonly ILogger<FilesController> _logger; 
    private readonly ICacheService _cacheService;
    public FilesController(
        IStorageService storageService,
        ILogger<FilesController> logger, 
        ICacheService cacheService)
    {
        _storageService = storageService;
        _logger = logger; 
        _cacheService = cacheService;
    }


    [HttpPost("images/private")]
    [EnableRateLimiting("FileUploadRateLimiterPolicy")]
    public async Task<IResult> SavePrivateImages(IFormFile file)
    {
        try
        {
            if (file is null)
            {
                return await Task.FromResult(TypedResults.BadRequest("Invalid request parameter"));
            }

            if (!FileValidation.IsValidImage(file))
            {
                return await Task.FromResult(
                    TypedResults.BadRequest("Invalid file type. Only image files are allowed."));
            }

            if (!await _storageService.IsBucketPrivateExistsAsync())
            {
                _logger.LogError("Private bucket does not configured yet");
                return await Task.FromResult(
                    TypedResults.InternalServerError(Messages.InternalServerError));
            }

            var newFileName = UrlBuilder.Build(Guid.NewGuid().ToString(), Path.GetExtension(file.FileName));
            var objectName = UrlBuilder.Build(ObjectPrefix.ImageResource, newFileName);

            await _storageService.UploadFilePrivateAsync(objectName, file);

            return TypedResults.Created(string.Empty, new
            {
                ResourceName = newFileName
            });
        }
        catch (MinioException ex)
        {
            _logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }


    [HttpPost("images/public")]
    [EnableRateLimiting("FileUploadRateLimiterPolicy")]
    public async Task<IResult> SavePublicImages(IFormFile file)
    {
        try
        {
            if (file is null)
            {
                return await Task.FromResult(
                    TypedResults.BadRequest("Invalid request parameter"));
            }

            if (!FileValidation.IsValidImage(file))
            {
                return await Task.FromResult(
                    TypedResults.BadRequest("Invalid file type. Only image files are allowed."));
            }

            if (!await _storageService.IsBucketPublicExistsAsync())
            {
                _logger.LogError("Public bucket does not configured yet");
                return await Task.FromResult(TypedResults.InternalServerError(Messages.InternalServerError));
            }

            var newFileName = UrlBuilder.Build(Guid.NewGuid().ToString(), Path.GetExtension(file.FileName));
            var objectName = UrlBuilder.Build(ObjectPrefix.ImageResource, newFileName);

            await _storageService.UploadFilePublicAsync(objectName, file);

            return TypedResults.Created(string.Empty, new
            {
                ResourceName = newFileName
            });
        }
        catch (MinioException ex)
        {
            _logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }


    [HttpGet("images/private/{resourceName}")]
    [EnableRateLimiting("FileReadRateLimiting")]
    public async Task<IResult> Get(string resourceName)
    {
        try
        {
            var type = Path.GetExtension(resourceName);
            var objectName = UrlBuilder.Build(ObjectPrefix.ImageResource, resourceName);

            var cache = await _cacheService.GetAsync<byte[]>(objectName);
            if (cache is not null)
            {
                return TypedResults.File(cache, ContentType.GetContentType(type), resourceName);
            }

            if (string.IsNullOrEmpty(resourceName))
            {
                return TypedResults.BadRequest(
                    Result.Failure("Data not found", ResponseStatus.BadRequest)
                    .WithError(new("ResourceName", "Object url must not empty")));
            }

            var bytes = await _storageService.GetFilePrivateAsync(objectName);

            if (bytes is null)
            {
                return TypedResults.NotFound(
                    Result.Failure("Data not found", ResponseStatus.NotFound)
                    .WithError(new("ResourceName", "Object resource not found")));
            }

            await _cacheService.SetAsync(objectName, bytes, TimeSpan.FromMinutes(5));

            return TypedResults.File(bytes, ContentType.GetContentType(type), resourceName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    [HttpGet("images/public/{resourceName}/url")]
    public async Task<IResult> GetPublicResourceUrl(string resourceName)
    {
        try
        {
            return TypedResults.Ok(new 
            { 
                Url = await _storageService.GeneratePublicObjectUrl(UrlBuilder.Build(ObjectPrefix.ImageResource, resourceName))
            });  
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }
}
