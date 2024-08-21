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
using Account.API.Applications.Queries.GetDrivingLicenseByMechanicUserId;
using Account.API.Applications.Queries.GetMechanicUserById;
using Account.API.Applications.Queries.GetNationalIdentityByMechanicUserId;
using Account.API.Applications.Queries.GetPaginatedMechanicCertificationByUserId;
using Account.API.Applications.Queries.GetPaginatedMechanicUser;
using Account.API.CustomAttributes;
using Account.API.Extensions;
using Account.API.Infrastructures;
using Account.API.Options;
using Account.API.SeedWork;
using Account.API.Services; 
using FluentValidation;
using Identity.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
namespace Account.API.API;

public static class MechanicUserApi
{
    public static IEndpointRouteBuilder MapMechanicUserApi(this IEndpointRouteBuilder builder)
    {
        var app = builder.MapGroup("api/v1/users/mechanic"); 

        app.MapPost("/", CreateMechanicUser)
            .WithMetadata(new IdempotencyAttribute(nameof(CreateMechanicUser)))
            .RequireAuthorization(PolicyName.MechanicUserPolicy); 


        app.MapGet("/", GetPaginatedMechanicUser)
             .RequireAuthorization(PolicyName.AdministratorPolicy);
        
        app.MapGet("/{mechanicUserId}", GetMechanicUserById)
            .CacheOutput()
             .RequireAuthorization(PolicyName.MechanicUserPolicy, PolicyName.AdministratorPolicy); 

        app.MapGet("/{mechanicUserId}/certifications", GetPaginatedCertificationsByMechanicUserId)
             .RequireAuthorization(PolicyName.MechanicUserPolicy, PolicyName.AdministratorPolicy);

        app.MapGet("/{mechanicUserId}/driving-license", GetDrivingLicenseByMechanicUserId)
            .CacheOutput()
            .RequireAuthorization(PolicyName.MechanicUserPolicy, PolicyName.AdministratorPolicy);

        app.MapGet("/{mechanicUserId}/national-identity", GetNationalIdentityByMechanicUserId)
            .CacheOutput()
            .RequireAuthorization(PolicyName.MechanicUserPolicy, PolicyName.AdministratorPolicy);



        app.MapPatch("/{mechanicUserId}/rating", SetRating)
             .RequireAuthorization(PolicyName.AdministratorPolicy);

        app.MapPatch("/{mechanicUserId}/verify", VerifyMechanicUserByUserId)
             .RequireAuthorization(PolicyName.AdministratorPolicy);

        app.MapPatch("/{mechanicUserId}/driving-license/{drivingLicenseId}/confirm", ConfirmDrivingLicenseByUserId)
             .RequireAuthorization(PolicyName.AdministratorPolicy);

        app.MapPatch("/{mechanicUserId}/driving-license/{drivingLicenseId}/reject", RejectDrivingLicenseByUserId)
             .RequireAuthorization(PolicyName.AdministratorPolicy);

        app.MapPatch("/{mechanicUserId}/national-identity/{nationalIdentityId}/confirm", ConfirmNationalIdentityByUserId)
             .RequireAuthorization(PolicyName.AdministratorPolicy);

        app.MapPatch("/{mechanicUserId}/national-identity/{nationalIdentityId}/reject", RejectNationalIdentityByUserId)
             .RequireAuthorization(PolicyName.AdministratorPolicy);

        app.MapPatch("/device-id", SetDeviceIdByUserId)
             .RequireAuthorization(PolicyName.MechanicUserPolicy);

        app.MapPatch("/{mechanicUserId}/device-id/reset", ResetDeviceIdByUserId)
             .RequireAuthorization(PolicyName.MechanicUserPolicy, PolicyName.AdministratorPolicy);

        app.MapPatch("/{mechanicUserId}/device-id/force-set", ForceSetDeviceIdByUserId)
             .RequireAuthorization(PolicyName.MechanicUserPolicy, PolicyName.AdministratorPolicy);



        app.MapPost("/driving-license", SetDrivingLicenseByUserId)
            .RequireAuthorization(PolicyName.MechanicUserPolicy);

        app.MapPost("/national-identity", SetNationalIdentityByUserId)
            .RequireAuthorization(PolicyName.MechanicUserPolicy);

        app.MapPut("/", UpdateMechanicUserByUserId) 
            .RequireAuthorization(PolicyName.MechanicUserPolicy);

        app.MapDelete("/{mechanicUserId}", DeleteMechanicUserByUserId)  
            .RequireAuthorization(PolicyName.AdministratorPolicy);

        return app;
    }

    private static async Task<IResult> GetNationalIdentityByMechanicUserId(
        Guid mechanicUserId, 
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

            if (userClaim.CurrentUserType != UserType.Administrator && mechanicUserId != userClaim.Sub)
            {
                return TypedResults.BadRequest();
            }

            var result = await service.Mediator.Send(new GetNationalIdentityByMechanicUserIdQuery(mechanicUserId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> GetDrivingLicenseByMechanicUserId(
        Guid mechanicUserId,
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

            if (userClaim.CurrentUserType != UserType.Administrator && mechanicUserId != userClaim.Sub)
            {
                return TypedResults.BadRequest();
            }

            var result = await service.Mediator.Send(new GetDrivingLicenseByMechanicUserIdQuery(mechanicUserId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> GetPaginatedCertificationsByMechanicUserId(
        Guid mechanicUserId, 
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

            if (userClaim.CurrentUserType != UserType.Administrator && mechanicUserId != userClaim.Sub)
            {
                return TypedResults.BadRequest();
            } 


            var result = await service.Mediator.Send(new GetPaginatedMechanicCertificationByUserIdQuery(
                mechanicUserId,
                request.PageNumber,
                request.PageSize));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> GetPaginatedMechanicUser(
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


            var result = await service.Mediator.Send(new GetPaginatedMechanicUserQuery(
                request.PageNumber,
                request.PageSize));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> DeleteMechanicUserByUserId(
        Guid mechanicUserId,
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
        [FromBody] UpdateMechanicRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<UpdateMechanicRequestDto> validator,
        [FromServices] IUserInfoService userInfoService)
    {
        try
        {
            var userClaim = userInfoService.GetUserInfoAsync();
            if (userClaim is null)
            {
                return TypedResults.Unauthorized();
            }

            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
            {
                return TypedResults.BadRequest(validation.Errors);
            }

            var result = await service.Mediator.Send(new UpdateMechanicUserByUserIdCommand(
                userClaim.Sub,
                request.PersonalInfo,
                request.Address,
                request.TimeZoneId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> SetNationalIdentityByUserId( 
        [FromBody] NationalIdentityRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<NationalIdentityRequestDto> validator,
        [FromServices] IUserInfoService userInfoService)
    {
        try
        {
            var userClaim = userInfoService.GetUserInfoAsync();
            if (userClaim is null)
            {
                return TypedResults.Unauthorized();
            }

            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
            {
                return TypedResults.BadRequest(validation.Errors);
            }

            var result = await service.Mediator.Send(new SetNationalIdentityByUserIdCommand(
                userClaim.Sub,
                request.IdentityNumber,
                request.FrontSideImageUrl,
                request.BackSideImageUrl));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> SetDrivingLicenseByUserId( 
        [FromBody] DrivingLicenseRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<DrivingLicenseRequestDto> validator,
        [FromServices] IUserInfoService userInfoService)
    {
        try
        {
            var userClaim = userInfoService.GetUserInfoAsync();
            if (userClaim is null)
            {
                return TypedResults.Unauthorized();
            }

            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
            {
                return TypedResults.BadRequest(validation.Errors);
            }

            var result = await service.Mediator.Send(new SetDrivingLicenseByUserIdCommand(
                userClaim.Sub,
                request.LicenseNumber,
                request.FrontSideImageUrl,
                request.BackSideImageUrl));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> ForceSetDeviceIdByUserId(
        Guid mechanicUserId,
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

            if (userClaim.CurrentUserType != UserType.Administrator && mechanicUserId != userClaim.Sub)
            {
                return TypedResults.BadRequest();
            }

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
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> ResetDeviceIdByUserId(
        Guid mechanicUserId,
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

            if (userClaim.CurrentUserType != UserType.Administrator && mechanicUserId != userClaim.Sub)
            {
                return TypedResults.BadRequest();
            }
             

            var result = await service.Mediator.Send(new ResetDeviceIdByMechanicUserIdCommand(mechanicUserId));

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

            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
            {
                return TypedResults.BadRequest(validation.Errors);
            }

            var result = await service.Mediator.Send(new SetDeviceIdByMechanicUserIdCommand(
                userClaim.Sub,
                request.DeviceId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            var userClaim = userInfoService.GetUserInfoAsync();
            if (userClaim is null)
            {
                return TypedResults.Unauthorized();
            }

            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> RejectNationalIdentityByUserId(
        Guid mechanicUserId, 
        Guid nationalIdentityId,
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

            var result = await service.Mediator.Send(new RejectNationalIdentityByUserIdCommand(mechanicUserId, nationalIdentityId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> ConfirmNationalIdentityByUserId(
        Guid mechanicUserId, 
        Guid nationalIdentityId,
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

            var result = await service.Mediator.Send(new ConfirmNationalIdentityByUserIdCommand(mechanicUserId, nationalIdentityId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> RejectDrivingLicenseByUserId(
        Guid mechanicUserId,
        Guid drivingLicenseId,
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

            var result = await service.Mediator.Send(new RejectDrivingLicenseByUserIdCommand(mechanicUserId, drivingLicenseId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> ConfirmDrivingLicenseByUserId(
        Guid mechanicUserId,
        Guid drivingLicenseId,
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

            var result = await service.Mediator.Send(new ConfirmDrivingLicenseByUserIdCommand(mechanicUserId, drivingLicenseId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> VerifyMechanicUserByUserId(
        Guid mechanicUserId,
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

            var result = await service.Mediator.Send(new VerifyMechanicUserByUserIdCommand(mechanicUserId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    } 
     
    private static async Task<IResult> SetRating(
        Guid mechanicUserId, 
        [FromBody] SetRatingRequestDto request,
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

            var result = await service.Mediator.Send(new SetRatingByUserIdCommand(mechanicUserId, request.Rating));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> GetMechanicUserById(
        Guid mechanicUserId,
        [FromServices] IKeyManagementService _kms,
        [FromServices] IOptionsMonitor<KeyManagementServiceOption> options,
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

            if (userClaim.CurrentUserType != UserType.Administrator && mechanicUserId != userClaim.Sub)
            {
                return TypedResults.BadRequest();
            }

            var result = await service.Mediator.Send(new GetMechanicUserByIdQuery(mechanicUserId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> CreateMechanicUser(
        [FromBody] MechanicUserRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<MechanicUserRequestDto> validator,
        [FromServices] IUserInfoService userInfoService)
    {
        try
        {
            var userClaim = userInfoService.GetUserInfoAsync();
            if (userClaim is null)
            {
                return TypedResults.Unauthorized();
            }

            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                return TypedResults.BadRequest(validation.Errors);
            }

            var result = await service.Mediator.Send(new CreateMechanicUserCommand(
                userClaim.Sub, 
                userClaim.Email,
                userClaim.PhoneNumber,
                request.TimeZoneId,
                request.ReferralCode,
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
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }
}
