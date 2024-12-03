using Core.CustomMessages;
using Core.Results;
using Core.Utilities;
using Master.Data.API.Domain.Entity;
using Master.Data.API.Dtos;
using Master.Data.API.Infrastructure.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching; 

namespace Master.Data.API.Controllers; 

[ApiController]
[Route("api/v1/banners")]
public class BannerController : ControllerBase
{
    private readonly ILogger<BannerController> _logger;
    private readonly BannerRepository _repository;
    public BannerController(ILogger<BannerController> logger, BannerRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    [HttpGet]
    [OutputCache(Duration = 60)]
    [AllowAnonymous]
    public async Task<IResult> GetActiveBanners()
    {
        try
        {
            var banners = await _repository.GetActiveBannersAsync();
            if (banners == null || banners.Count == 0)
            {
                return TypedResults.NotFound(Result.Failure("Banner not found", ResponseStatus.NotFound));
            }

            return TypedResults.Ok(Result.Success(new 
            {
                Banners = banners.Select(x => x.ImagePath).ToList()
            }, ResponseStatus.Ok));
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }


    [HttpGet("{id}")]
    [OutputCache(Duration = 60)]
    [AllowAnonymous]
    public async Task<IResult> GetBannerById(Guid id)
    {
        try
        {
            var banner = await _repository.GetBannerById(id);
            if (banner == null)
            {
                return TypedResults.NotFound(Result.Failure("Banner not found", ResponseStatus.NotFound));
            }

            return TypedResults.Ok(Result.Success(new
            {
                Banner = banner
            }, ResponseStatus.Ok));
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IResult> CreateBanner([FromBody] CreateBannerRequestDto request)
    {
        if (request == null)
        { 
            return TypedResults.BadRequest(Result.Failure("Invalid banner data", ResponseStatus.BadRequest));
        }

        try
        {
            var banner = new Banner(request.Name, request.Description, request.ImagePath, request.IsActive);
            var result = await _repository.SaveAsync(banner);

            return TypedResults.Created($"api/v1/banners/{result.Id}", Result.Success(new
            {
                Banner = banner
            }, ResponseStatus.Created));
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }

    // Update an existing Banner
    [HttpPut("{id}")]
    [AllowAnonymous]
    public async Task<IResult> UpdateBanner(Guid id, [FromBody] UpdateBannerRequestDto request)
    {
        if (request == null || request.Id != id)
        {
            return TypedResults.BadRequest(Result.Failure("Banner id missmatch", ResponseStatus.BadRequest));
        }

        try
        {
            var existingBanner = await _repository.GetBannerById(id);
            if (existingBanner == null)
            {
                return TypedResults.NotFound(Result.Failure("Banner not found", ResponseStatus.NotFound));
            }

            existingBanner.Update(request.Name, request.Description, request.ImagePath, request.IsActive);

            await _repository.UpdateAsync(existingBanner);
            return TypedResults.NoContent();
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }

    // Delete a Banner
    [HttpDelete("{id}")]
    [AllowAnonymous]
    public async Task<IResult> DeleteBanner(Guid id)
    {
        try
        {
            var banner = await _repository.GetBannerById(id);
            if (banner == null)
            {
                return TypedResults.NotFound(Result.Failure("Banner not found", ResponseStatus.NotFound));
            }

            await _repository.DeleteAsync(banner);
            return TypedResults.NoContent();
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }
}

