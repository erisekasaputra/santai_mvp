
using Account.API.Applications.Commands.FleetCommand.CreateFleetByUserId;
using Account.API.Applications.Commands.FleetCommand.DeleteFleetByIdByUserId;
using Account.API.Applications.Commands.FleetCommand.UpdateFleetByUserId;
using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Applications.Queries.GetFleetByIdByUserId;
using Account.API.Applications.Queries.GetPaginatedFleetByUserId;
using Account.API.Applications.Services; 
using Account.API.Extensions;
using Core.CustomAttributes;
using Core.CustomMessages;
using Core.Dtos;
using Core.Enumerations;
using Core.Models;
using Core.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Account.API.API;

public static class FleetApi
{ 
    public static IEndpointRouteBuilder MapFleetApi(this IEndpointRouteBuilder builder)
    {
        var app = builder.MapGroup("api/v1/users/{userId}/fleet"); 

        app.MapPost("/", CreateFleetByUserId)
            .WithMetadata(new IdempotencyAttribute(nameof(CreateFleetByUserId)))
            .RequireAuthorization();

        app.MapGet("/", GetPaginatedFleetByUserId)
            .RequireAuthorization();

        app.MapGet("/{fleetId}", GetFleetByIdByUserId)
            .RequireAuthorization();

        app.MapDelete("/{fleetId}", DeleteFleetByIdByUserId)
            .RequireAuthorization(); 

        app.MapPut("/{fleetId}", UpdateFleetByUserId)
            .RequireAuthorization();

        return builder; 
    }

    private static bool QueryUserIdEqualWithClaimUserId(UserClaim claim, Guid userId)
    {
        if (claim.CurrentUserType is not UserType.Administrator && userId != claim.Sub)
        {
            return false;
        }

        return true;
    }

    private static async Task<IResult> UpdateFleetByUserId(
        Guid userId, 
        Guid fleetId,
        [FromBody] UpdateFleetRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<UpdateFleetRequestDto> validator,
        [FromServices] IUserInfoService userInfoService)
    { 
        try
        {
            var userClaim = userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Unauthorized();
            }  

            if (!QueryUserIdEqualWithClaimUserId(userClaim, userId))
            {
                return TypedResults.Forbid();
            }


            var validate = await validator.ValidateAsync(request);
            if (!validate.IsValid)
            {
                var error = validate.Errors;
                return TypedResults.BadRequest(error);
            }

            var result = await service.Mediator.Send(
                new UpdateFleetByUserIdCommand(
                    userId, 
                    fleetId, 
                    request.RegistrationNumber,
                    request.VehicleType,
                    request.Brand,
                    request.Model,
                    request.YearOfManufacture,
                    request.ChassisNumber,
                    request.EngineNumber,
                    request.InsuranceNumber,
                    request.IsInsuranceValid,
                    request.LastInspectionDateLocal,
                    request.OdometerReading,
                    request.FuelType, 
                    request.OwnerName,
                    request.OwnerAddress,
                    request.UsageStatus,
                    request.OwnershipStatus,
                    request.TransmissionType,
                    request.ImageUrl));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> DeleteFleetByIdByUserId(
        Guid userId,
        Guid fleetId,
        [FromServices] ApplicationService service,
        [FromServices] IUserInfoService userInfoService)
    {
        // later on , user id is from user claims at authentication level  
        try
        {

            var userClaim = userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Unauthorized();
            }

            if (!QueryUserIdEqualWithClaimUserId(userClaim, userId))
            {
                return TypedResults.Forbid();
            }


            var result = await service.Mediator.Send(
                new DeleteFleetByIdByUserIdCommand(userId, fleetId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> CreateFleetByUserId(
        Guid userId,
        [FromBody] CreateFleetRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<CreateFleetRequestDto> validator,
        [FromServices] IUserInfoService userInfoService)
    {
        // later on , user id is from user claims at authentication level  
        try
        {
            var userClaim = userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Unauthorized();
            }

            if (!QueryUserIdEqualWithClaimUserId(userClaim, userId))
            {
                return TypedResults.Forbid();
            }


            var validate = await validator.ValidateAsync(request);
            if (!validate.IsValid)
            {
                var error = validate.Errors;
                return TypedResults.BadRequest(error);
            }

            var result = await service.Mediator.Send(
                new CreateFleetByUserIdCommand(
                    userId,
                    request.RegistrationNumber,
                    request.VehicleType,
                    request.Brand,
                    request.Model,
                    request.YearOfManufacture,
                    request.ChassisNumber,
                    request.EngineNumber,
                    request.InsuranceNumber,
                    request.IsInsuranceValid,
                    request.LastInspectionDateLocal,
                    request.OdometerReading,
                    request.FuelType, 
                    request.OwnerName,
                    request.OwnerAddress,
                    request.UsageStatus,
                    request.OwnershipStatus,
                    request.TransmissionType,
                    request.ImageUrl));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> GetFleetByIdByUserId(
        Guid userId,
        Guid fleetId,
        [FromServices] ApplicationService service,
        [FromServices] IUserInfoService userInfoService) 
    { 
        try
        {
            var userClaim = userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Unauthorized();
            }  

            var result = await service.Mediator.Send(
                new GetFleetByIdByUserIdQuery(userId, fleetId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> GetPaginatedFleetByUserId(
        Guid userId,
        [AsParameters] PaginatedRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IUserInfoService userInfoService)
    {
        // later on , user id is from user claims at authentication level  
        try
        {
            var userClaim = userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Unauthorized();
            }

            if (!QueryUserIdEqualWithClaimUserId(userClaim, userId))
            {
                return TypedResults.Forbid();
            }

            var result = await service.Mediator.Send(
                new GetPaginatedFleetByUserIdQuery(userId, request.PageNumber, request.PageSize));
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }
}
