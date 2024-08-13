using Account.API.Applications.Commands.RegularUserCommand.CreateRegularUser;
using Account.API.Applications.Commands.RegularUserCommand.DeleteRegularUserByUserId;
using Account.API.Applications.Commands.RegularUserCommand.UpdateRegularUserByUserId;
using Account.API.Applications.Commands.RegularUserCommand.ForceSetDeviceIdByUserId;
using Account.API.Applications.Commands.RegularUserCommand.ResetDeviceIdByUserId;
using Account.API.Applications.Commands.RegularUserCommand.SetDeviceIdByUserId;
using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Applications.Queries.GetRegularUserByUserId;
using Account.API.Extensions;
using Account.API.SeedWork;
using Account.API.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc; 
using Account.API.Applications.Queries.GetPaginatedRegularUser;

namespace Account.API.API;

public static class RegularUserApi
{
    public static IEndpointRouteBuilder MapRegularUserApi(this IEndpointRouteBuilder builder)
    {
        var app = builder.MapGroup("api/v1/users/regular");

        app.MapGet("/{regularUserId}", GetRegularUserByUserId);
        app.MapGet("/", GetPaginatedRegularUser);

        app.MapPost("/", CreateRegularUser).WithMetadata(new IdempotencyAttribute());
        
        app.MapPut("/{regularUserId}", UpdateRegularUserByUserId);
        
        app.MapPatch("/{regularUserId}/device-id", SetDeviceIdByUserId);
        app.MapPatch("/{regularUserId}/device-id/force-set", ForceSetDeviceIdByUserId);
        app.MapPatch("/{regularUserId}/device-id/reset", ResetDeviceIdByUserId);
        app.MapDelete("/{regularUserId}", DeleteRegularUserByUserId);

        return app;
    }

    private static async Task<IResult> GetPaginatedRegularUser(
        [AsParameters] PaginatedItemRequestDto request,
        [FromServices] ApplicationService service)
    {
        try
        { 
            var result = await service.Mediator.Send(new GetPaginatedRegularUserQuery(request.PageNumber, request.PageSize));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> ForceSetDeviceIdByUserId(
        Guid regularUserId,
        [FromBody] DeviceIdRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<DeviceIdRequestDto> validator)
    {
        try
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return TypedResults.BadRequest(validationResult.Errors);
            }

            var result = await service.Mediator.Send(new ForceSetDeviceIdByUserIdCommand(
                regularUserId,
                request.DeviceId)); 

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> ResetDeviceIdByUserId(
        Guid regularUserId,
        [FromServices] ApplicationService service)
    {
        try
        {
            var result = await service.Mediator.Send(new ResetDeviceIdByUserIdCommand(regularUserId)); 
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> SetDeviceIdByUserId(
        Guid regularUserId,
        [FromBody] DeviceIdRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<DeviceIdRequestDto> validator)
    {
        try
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return TypedResults.BadRequest(validationResult.Errors);
            }

            var result = await service.Mediator.Send(new SetDeviceIdByUserIdCommand(
                regularUserId,
                request.DeviceId)); 

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> DeleteRegularUserByUserId(
        Guid regularUserId,
        [FromServices] ApplicationService service)
    {
        try
        {
            var result = await service.Mediator.Send(new DeleteRegularUserByUserIdCommand(regularUserId)); 
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        } 
    }

    private static async Task<IResult> UpdateRegularUserByUserId(
        Guid regularUserId,
        [FromBody] UpdateRegularUserRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<UpdateRegularUserRequestDto> validator)
    {
        try
        { 
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return TypedResults.BadRequest(validationResult.Errors);
            }

            var result = await service.Mediator.Send(new UpdateRegularUserByUserIdCommand(
                regularUserId,
                request.TimeZoneId,
                request.Address,
                request.PersonalInfo));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> GetRegularUserByUserId(
        Guid regularUserId,
        [FromServices] ApplicationService service)
    {
        try
        {
            var result = await service.Mediator.Send(new GetRegularUserByUserIdQuery(regularUserId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    } 

    private static async Task<IResult> CreateRegularUser(
        [FromBody] RegularUserRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<RegularUserRequestDto> validator)
    {
        try
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return TypedResults.BadRequest(validationResult.Errors);
            }

            var result = await service.Mediator.Send(new CreateRegularUserCommand(
                request.IdentityId,
                request.Username,
                request.Email,
                request.PhoneNumber,
                request.TimeZoneId,
                request.ReferralCode,
                request.Address,
                request.PersonalInfo,
                request.DeviceId)); 
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }
}
