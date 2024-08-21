using Account.API.Applications.Commands.RegularUserCommand.CreateRegularUser;
using Account.API.Applications.Commands.RegularUserCommand.DeleteRegularUserByUserId;
using Account.API.Applications.Commands.RegularUserCommand.UpdateRegularUserByUserId;
using Account.API.Applications.Commands.RegularUserCommand.ForceSetDeviceIdByUserId;
using Account.API.Applications.Commands.RegularUserCommand.ResetDeviceIdByUserId;
using Account.API.Applications.Commands.RegularUserCommand.SetDeviceIdByUserId;
using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Applications.Queries.GetRegularUserByUserId;
using Account.API.Extensions;
using Account.API.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Account.API.Applications.Queries.GetPaginatedRegularUser;
using Account.API.CustomAttributes;
using Account.API.SeedWork;
using Account.API.Infrastructures;
using Identity.Contracts;

namespace Account.API.API;

public static class RegularUserApi
{
    public static IEndpointRouteBuilder MapRegularUserApi(this IEndpointRouteBuilder builder)
    {
        var app = builder.MapGroup("api/v1/users/regular");

        app.MapGet("/{regularUserId}", GetRegularUserByUserId).CacheOutput()
            .RequireAuthorization("RegularUserPolicy", "AdministratorPolicy");

        app.MapGet("/", GetPaginatedRegularUser)
            .RequireAuthorization("AdministratorPolicy");

        app.MapPost("/", CreateRegularUser)
            .WithMetadata(new IdempotencyAttribute(nameof(CreateRegularUser)))
            .RequireAuthorization("RegularUserPolicy");
        
        app.MapPut("/{regularUserId}", UpdateRegularUserByUserId)
            .RequireAuthorization("RegularUserPolicy");
        
        app.MapPatch("/{regularUserId}/device-id", SetDeviceIdByUserId)
            .RequireAuthorization("RegularUserPolicy");

        app.MapPatch("/device-id/force-set", ForceSetDeviceIdByUserId)
            .RequireAuthorization("RegularUserPolicy");

        app.MapPatch("/{regularUserId}/device-id/reset", ResetDeviceIdByUserId) 
            .RequireAuthorization("RegularUserPolicy");

        app.MapDelete("/{regularUserId}", DeleteRegularUserByUserId)
            .RequireAuthorization("AdministratorPolicy");

        return app;
    }

    private static async Task<IResult> GetPaginatedRegularUser(
        [AsParameters] PaginatedItemRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IUserInfoService userInfoService)
    {
        try
        {
            var userClaim = userInfoService.GetUserInfoAsync();
            if (userClaim is null)
            {
                return TypedResults.Unauthorized();
            }

            var result = await service.Mediator.Send(new GetPaginatedRegularUserQuery(request.PageNumber, request.PageSize));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> ForceSetDeviceIdByUserId( 
        [FromBody] DeviceIdRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<DeviceIdRequestDto> validator,
        [FromServices] IUserInfoService userInfoService)
    {
        try
        {
            var userClaim = userInfoService.GetUserInfoAsync(); 
            if (userClaim is null)
            {
                return TypedResults.Unauthorized();
            }

            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return TypedResults.BadRequest(validationResult.Errors);
            }

            var result = await service.Mediator.Send(new ForceSetDeviceIdByUserIdCommand(
                userClaim.Sub,
                request.DeviceId)); 

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> ResetDeviceIdByUserId( 
        [FromServices] ApplicationService service,
        [FromServices] IUserInfoService userInfoService)
    {
        try
        {
            var userClaim = userInfoService.GetUserInfoAsync();
            if (userClaim is null)
            {
                return TypedResults.Unauthorized();
            }

            var result = await service.Mediator.Send(new ResetDeviceIdByUserIdCommand(userClaim.Sub)); 
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> SetDeviceIdByUserId( 
        [FromBody] DeviceIdRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<DeviceIdRequestDto> validator,
        [FromServices] IUserInfoService userInfoService)
    {
        try
        {
            var userClaim = userInfoService.GetUserInfoAsync();
            if (userClaim is null)
            {
                return TypedResults.Unauthorized();
            }

            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return TypedResults.BadRequest(validationResult.Errors);
            }

            var result = await service.Mediator.Send(new SetDeviceIdByUserIdCommand(
                userClaim.Sub,
                request.DeviceId)); 

            return result.ToIResult();
        }
        catch (Exception ex)
        { 
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> DeleteRegularUserByUserId(
        Guid regularUserId,
        [FromServices] ApplicationService service,
        [FromServices] IUserInfoService userInfoService)
    {
        try
        {
            var userClaim = userInfoService.GetUserInfoAsync();
            if (userClaim is null)
            {
                return TypedResults.Unauthorized();
            }

            var result = await service.Mediator.Send(new DeleteRegularUserByUserIdCommand(regularUserId)); 
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        } 
    }

    private static async Task<IResult> UpdateRegularUserByUserId( 
        [FromBody] UpdateRegularUserRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<UpdateRegularUserRequestDto> validator,
        [FromServices] IUserInfoService userInfoService)
    {
        try
        {
            var userClaim = userInfoService.GetUserInfoAsync();
            if (userClaim is null)
            {
                return TypedResults.Unauthorized(); 
            }

            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return TypedResults.BadRequest(validationResult.Errors);
            }

            var result = await service.Mediator.Send(new UpdateRegularUserByUserIdCommand(
                userClaim.Sub,
                request.TimeZoneId,
                request.Address,
                request.PersonalInfo));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> GetRegularUserByUserId(
        Guid regularUserId,
        [FromServices] ApplicationService service,
        [FromServices] IUserInfoService userInfoService)
    {
        try
        {
            var userClaim = userInfoService.GetUserInfoAsync();
            if (userClaim is null)
            {
                return TypedResults.Unauthorized();
            }

            if (userClaim.CurrentUserType != UserType.Administrator && regularUserId != userClaim.Sub)
            { 
                return TypedResults.BadRequest();
            }

            var result = await service.Mediator.Send(new GetRegularUserByUserIdQuery(regularUserId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    } 

    private static async Task<IResult> CreateRegularUser(
        [FromBody] RegularUserRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<RegularUserRequestDto> validator,
        [FromServices] IUserInfoService userInfoService)
    {
        try
        {
            var userClaim = userInfoService.GetUserInfoAsync();
            if (userClaim is null)
            {
                return TypedResults.Unauthorized();
            }

            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return TypedResults.BadRequest(validationResult.Errors);
            }

            var result = await service.Mediator.Send(new CreateRegularUserCommand(
                userClaim.Sub,
                userClaim.Email,
                userClaim.PhoneNumber,
                request.TimeZoneId,
                request.ReferralCode,
                request.Address,
                request.PersonalInfo,
                request.DeviceId)); 
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }
}
