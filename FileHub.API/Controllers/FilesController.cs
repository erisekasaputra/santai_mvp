using FileHub.API.Abstraction;
using FileHub.API.SeedWork;
using FileHub.API.Utilities;
using FileHub.API.Validations.Abstraction;
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
    private readonly IFileValidation _fileValidation;
    private readonly ICacheService _cacheService;
    public FilesController(
        IStorageService storageService,
        ILogger<FilesController> logger,
        IFileValidation fileValidation,
        ICacheService cacheService)
    {
        _storageService = storageService;
        _logger = logger;
        _fileValidation = fileValidation;
        _cacheService = cacheService;
    }
     

    [HttpPost("user/{userId}/profile-picture")]
    [EnableRateLimiting("FileUploadRateLimiterPolicy")]
    public async Task<IResult> SaveUserProfilePicture(Guid userId, IFormFile file)
    {
        try
        {
            if (file is null || userId == Guid.Empty)
            {
                return await Task.FromResult(TypedResults.BadRequest("Invalid request parameter"));
            }

            if (!_fileValidation.IsValidImage(file))
            { 
                return await Task.FromResult(TypedResults.BadRequest("Invalid file type. Only image files are allowed."));
            }

            if (!await _storageService.IsBucketExistsAsync())
            {
                await _storageService.CreateBucketAsync();
            }

            var newFileName = UrlBuilder.Build(userId.ToString(), "_", Guid.NewGuid().ToString(), Path.GetExtension(file.FileName));
            var objectName = UrlBuilder.Build(ObjectPrefix.UserImageProfilePrefix, newFileName);

            await _storageService.UploadFileAsync(objectName, file);

            return TypedResults.Created(string.Empty, new {
                ResourceName = newFileName
            });
        }
        catch (MinioException ex)
        {
            _logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }


    [HttpGet("user/profile-picture/{resourceName}")]
    [EnableRateLimiting("FileUploadRateLimiterPolicy")]
    public async Task<IResult> Get(string resourceName)
    {
        try
        {
            if (string.IsNullOrEmpty(resourceName))
            {
                return TypedResults.BadRequest(
                    Result.Failure("", ResponseStatus.BadRequest)
                    .WithError(new("ResourceName", "Object url must not empty")));
            }

            var type = Path.GetExtension(resourceName);

            var objectName = UrlBuilder.Build(ObjectPrefix.UserImageProfilePrefix, resourceName);

            var bytes = await _storageService.GetFileAsync(objectName);

            if (bytes is null)
            {
                return TypedResults.NotFound(
                    Result.Failure("", ResponseStatus.NotFound)
                    .WithError(new("ResourceName", "Object resource not found")));
            }

            return TypedResults.File(bytes, ContentType.GetContentType(type), resourceName);
        }
        catch (Exception ex) 
        {
            _logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }
}
