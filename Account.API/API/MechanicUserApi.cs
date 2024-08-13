using Account.API.Applications.Commands.MechanicUserCommand.ConfirmDrivingLicenseByUserId;
using Account.API.Applications.Commands.MechanicUserCommand.ConfirmNationalIdentityByUserId;
using Account.API.Applications.Commands.MechanicUserCommand.CreateMechanicUser;
using Account.API.Applications.Commands.MechanicUserCommand.DeleteMechanicUserByUserId;
using Account.API.Applications.Commands.MechanicUserCommand.ForceSetDeviceIdByMechanicUserId;
using Account.API.Applications.Commands.MechanicUserCommand.RejectDrivingLicenseByUserId;
using Account.API.Applications.Commands.MechanicUserCommand.RejectNationalIdentityByUserId;
using Account.API.Applications.Commands.MechanicUserCommand.ResetDeviceIdByMechanicUserId;
using Account.API.Applications.Commands.MechanicUserCommand.SetDeviceIdByMechanicUserId;
using Account.API.Applications.Commands.MechanicUserCommand.SetDrivingLicenseByUserId;
using Account.API.Applications.Commands.MechanicUserCommand.SetNationalIdentityByUserId;
using Account.API.Applications.Commands.MechanicUserCommand.SetRatingByUserId;
using Account.API.Applications.Commands.MechanicUserCommand.UpdateMechanicUserByUserId;
using Account.API.Applications.Commands.MechanicUserCommand.VerifyMechanicUserByUserId; 
using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Applications.Queries.GetMechanicUserById;
using Account.API.Extensions;
using Account.API.Options;
using Account.API.Services; 
using FluentValidation; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
namespace Account.API.API;

public static class MechanicUserApi
{
    public static IEndpointRouteBuilder MapMechanicUserApi(this IEndpointRouteBuilder builder)
    {
        var app = builder.MapGroup("api/v1/users/mechanic"); 

        app.MapPost("/", CreateMechanicUser);

        app.MapGet("/{mechanicUserId}", GetMechanicUserById); 
        app.MapGet("/", GetPaginatedMechanicUser);
        app.MapGet("/{mechanicUserId}/certifications", GetPaginatedCertificationsByMechanicUserId); 
        app.MapGet("/{mechanicUserId}/driving-license", GetDrivingLicenseByMechanicUserId); 
        app.MapGet("/{mechanicUserId}/national-identity", GetNationalIdentityByMechanicUserId); 

        app.MapPatch("/{mechanicUserId}/rating", SetRating);
        app.MapPatch("/{mechanicUserId}/verify", VerifyMechanicUserByUserId);
        app.MapPatch("/{mechanicUserId}/driving-license/{drivingLicenseId}/confirm", ConfirmDrivingLicenseByUserId);
        app.MapPatch("/{mechanicUserId}/driving-license/{drivingLicenseId}/reject", RejectDrivingLicenseByUserId);

        app.MapPatch("/{mechanicUserId}/national-identity/{nationalIdentityId}/confirm", ConfirmNationalIdentityByUserId);
        app.MapPatch("/{mechanicUserId}/national-identity/{nationalIdentityId}/reject", RejectNationalIdentityByUserId);

        app.MapPatch("/{mechanicUserId}/device-id", SetDeviceIdByUserId);
        app.MapPatch("/{mechanicUserId}/device-id/reset", ResetDeviceIdByUserId);
        app.MapPatch("/{mechanicUserId}/device-id/force-set", ForceSetDeviceIdByUserId);

        app.MapPost("/{mechanicUserId}/driving-license", SetDrivingLicenseByUserId);
        app.MapPost("/{mechanicUserId}/national-identity", SetNationalIdentityByUserId);

        app.MapPut("/{mechanicUserId}", UpdateMechanicUserByUserId);

        app.MapDelete("/{mechanicUserId}", DeleteMechanicUserByUserId);

        return app;
    }

    private static async Task GetNationalIdentityByMechanicUserId(Guid mechanicUserId)
    {
        throw new NotImplementedException();
    }

    private static async Task GetDrivingLicenseByMechanicUserId(Guid mechanicUserId)
    {
        throw new NotImplementedException();
    }

    private static async Task GetPaginatedCertificationsByMechanicUserId(Guid mechanicUserId, [AsParameters] PaginatedItemRequestDto request)
    {
        throw new NotImplementedException();
    }

    private static async Task GetPaginatedMechanicUser([AsParameters] PaginatedItemRequestDto request)
    {
        throw new NotImplementedException();
    }

    private static async Task<IResult> DeleteMechanicUserByUserId(
        Guid mechanicUserId,
        [FromServices] ApplicationService service)
    {
        try
        {
            var result = await service.Mediator.Send(new DeleteMechanicUserByUserIdCommand(mechanicUserId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> UpdateMechanicUserByUserId(
        Guid mechanicUserId,
        [FromBody] UpdateMechanicRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<UpdateMechanicRequestDto> validator)
    {
        try
        {
            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
            {
                return TypedResults.BadRequest(validation.Errors);
            }

            var result = await service.Mediator.Send(new UpdateMechanicUserByUserIdCommand(
                mechanicUserId,
                request.PersonalInfo,
                request.Address,
                request.TimeZoneId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> SetNationalIdentityByUserId(
        Guid mechanicUserId,
        [FromBody] NationalIdentityRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<NationalIdentityRequestDto> validator)
    {
        try
        {
            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
            {
                return TypedResults.BadRequest(validation.Errors);
            }

            var result = await service.Mediator.Send(new SetNationalIdentityByUserIdCommand(
                mechanicUserId,
                request.IdentityNumber,
                request.FrontSideImageUrl,
                request.BackSideImageUrl));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> SetDrivingLicenseByUserId(
        Guid mechanicUserId,
        [FromBody] DrivingLicenseRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<DrivingLicenseRequestDto> validator)
    {
        try
        {
            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
            {
                return TypedResults.BadRequest(validation.Errors);
            }

            var result = await service.Mediator.Send(new SetDrivingLicenseByUserIdCommand(
                mechanicUserId,
                request.LicenseNumber,
                request.FrontSideImageUrl,
                request.BackSideImageUrl));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> ForceSetDeviceIdByUserId(
        Guid mechanicUserId,
        [FromBody] DeviceIdRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<DeviceIdRequestDto> validator)
    {
        try
        {
            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
            {
                return TypedResults.BadRequest(validation.Errors);
            }

            var result = await service.Mediator.Send(new ForceSetDeviceIdByMechanicUserIdCommand(
                mechanicUserId,
                request.DeviceId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> ResetDeviceIdByUserId(
        Guid mechanicUserId,
        [FromServices] ApplicationService service)
    {
        try
        {
            var result = await service.Mediator.Send(new ResetDeviceIdByMechanicUserIdCommand(mechanicUserId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> SetDeviceIdByUserId(
        Guid mechanicUserId,
        [FromBody] DeviceIdRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<DeviceIdRequestDto> validator)
    {
        try
        {
            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
            {
                return TypedResults.BadRequest(validation.Errors);
            }

            var result = await service.Mediator.Send(new SetDeviceIdByMechanicUserIdCommand(
                mechanicUserId,
                request.DeviceId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> RejectNationalIdentityByUserId(
        Guid mechanicUserId, 
        Guid nationalIdentityId,
        [FromServices] ApplicationService service)
    {
        try
        {
            var result = await service.Mediator.Send(new RejectNationalIdentityByUserIdCommand(mechanicUserId, nationalIdentityId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> ConfirmNationalIdentityByUserId(
        Guid mechanicUserId, 
        Guid nationalIdentityId,
        [FromServices] ApplicationService service)
    {
        try
        {
            var result = await service.Mediator.Send(new ConfirmNationalIdentityByUserIdCommand(mechanicUserId, nationalIdentityId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> RejectDrivingLicenseByUserId(
        Guid mechanicUserId,
        Guid drivingLicenseId,
        [FromServices] ApplicationService service)
    {
        try
        {
            var result = await service.Mediator.Send(new RejectDrivingLicenseByUserIdCommand(mechanicUserId, drivingLicenseId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> ConfirmDrivingLicenseByUserId(
        Guid mechanicUserId,
        Guid drivingLicenseId,
        [FromServices] ApplicationService service)
    {
        try
        {
            var result = await service.Mediator.Send(new ConfirmDrivingLicenseByUserIdCommand(mechanicUserId, drivingLicenseId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> VerifyMechanicUserByUserId(
        Guid mechanicUserId,
        [FromServices] ApplicationService service)
    {
        try
        {
            var result = await service.Mediator.Send(new VerifyMechanicUserByUserIdCommand(mechanicUserId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

















    private static async Task<IResult> SetRating(
        Guid mechanicUserId, SetRatingRequestDto request,
        [FromServices] ApplicationService service)
    {
        try
        {
            var result = await service.Mediator.Send(new SetRatingByUserIdCommand(mechanicUserId, request.Rating));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> GetMechanicUserById(
        Guid mechanicUserId,
        [FromServices] IKeyManagementService _kms,
        [FromServices] IOptionsMonitor<KeyManagementServiceOption> options,
        [FromServices] ApplicationService service)
    {
        try
        {
            var result = await service.Mediator.Send(new GetMechanicUserByIdQuery(mechanicUserId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> CreateMechanicUser(
        [FromBody] MechanicUserRequestDto request,
        [FromServices] ApplicationService service,
        IValidator<MechanicUserRequestDto> validator)
    {
        try
        { 
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                return TypedResults.BadRequest(validation.Errors);
            }

            var result = await service.Mediator.Send(new CreateMechanicUserCommand(
                request.IdentityId,
                request.Username,
                request.Email,
                request.PhoneNumber,
                request.TimeZoneId,
                request.PersonalInfo,
                request.Address,
                request.Certifications,
                request.DrivingLicense,
                request.NationalIdentity,
                request.DeviceId));   

            return result.ToIResult();
        }
        catch (Exception ex) 
        {
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }
}
