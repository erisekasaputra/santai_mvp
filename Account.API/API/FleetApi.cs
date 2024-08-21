
using Account.API.Applications.Commands.FleetCommand.CreateFleetByUserId;
using Account.API.Applications.Commands.FleetCommand.DeleteFleetByIdByUserId;
using Account.API.Applications.Commands.FleetCommand.UpdateFleetByUserId;
using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Applications.Queries.GetFleetByIdByUserId;
using Account.API.Applications.Queries.GetPaginatedFleetByUserId;
using Account.API.CustomAttributes;
using Account.API.Extensions;
using Account.API.SeedWork;
using Account.API.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Account.API.API;

public static class FleetApi
{
    public static IEndpointRouteBuilder MapFleetApi(this IEndpointRouteBuilder builder)
    {
        var app = builder.MapGroup("api/v1/users/{userId}/fleet"); 

        app.MapPost("/", CreateFleetByUserId)
            .WithMetadata(new IdempotencyAttribute(nameof(CreateFleetByUserId))); 

        app.MapGet("/", GetPaginatedFleetByUserId); 
        app.MapGet("/{fleetId}", GetFleetByIdByUserId).CacheOutput(); 

        app.MapDelete("/{fleetId}", DeleteFleetByIdByUserId); 
        
        app.MapPut("/{fleetId}", UpdateFleetByUserId); 

        return builder; 
    }

    private static async Task<IResult> UpdateFleetByUserId(
        Guid userId, 
        Guid fleetId,
        [FromBody] UpdateFleetRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<UpdateFleetRequestDto> validator)
    {
        // later on , user id is from user claims at authentication level  
        try
        {
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
                    request.Make,
                    request.Model,
                    request.YearOfManufacture,
                    request.ChassisNumber,
                    request.EngineNumber,
                    request.InsuranceNumber,
                    request.IsInsuranceValid,
                    request.LastInspectionDateLocal,
                    request.OdometerReading,
                    request.FuelType,
                    request.Color,
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
        [FromServices] ApplicationService service)
    {
        // later on , user id is from user claims at authentication level  
        try
        { 
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
        [FromServices] IValidator<CreateFleetRequestDto> validator)
    {
        // later on , user id is from user claims at authentication level  
        try
        {
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
                    request.Make,
                    request.Model,
                    request.YearOfManufacture,
                    request.ChassisNumber,
                    request.EngineNumber,
                    request.InsuranceNumber,
                    request.IsInsuranceValid,
                    request.LastInspectionDateLocal,
                    request.OdometerReading,
                    request.FuelType,
                    request.Color,
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
        [FromServices] ApplicationService service)
    { 
        try
        { 
            var result = await service.Mediator.Send(
                new GetFleetByIdByUserIdQuery(userId, fleetId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> GetPaginatedFleetByUserId(
        Guid userId,
        [AsParameters] PaginatedItemRequestDto request,
        [FromServices] ApplicationService service)
    {
        // later on , user id is from user claims at authentication level  
        try
        {  
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
