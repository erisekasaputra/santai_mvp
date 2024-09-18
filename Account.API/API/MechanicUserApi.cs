using Account.API.Applications.Commands.MechanicUserCommand.ActivateMechanicStatusByUserId;
using Account.API.Applications.Commands.MechanicUserCommand.ConfirmDrivingLicenseByUserId;
using Account.API.Applications.Commands.MechanicUserCommand.ConfirmNationalIdentityByUserId;
using Account.API.Applications.Commands.MechanicUserCommand.CreateMechanicUser;
using Account.API.Applications.Commands.MechanicUserCommand.DeactivateMechanicStatusByUserId;
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
using Account.API.Applications.Commands.OrderTaskCommand.AcceptOrderByMechanicUserId;
using Account.API.Applications.Commands.OrderTaskCommand.RejectOrderMechanicByUserId;
using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Applications.Queries.GetDrivingLicenseByMechanicUserId;
using Account.API.Applications.Queries.GetMechanicUserById;
using Account.API.Applications.Queries.GetNationalIdentityByMechanicUserId;
using Account.API.Applications.Queries.GetPaginatedMechanicCertificationByUserId;
using Account.API.Applications.Queries.GetPaginatedMechanicUser;
using Account.API.Applications.Services; 
using Account.API.CustomAttributes;
using Account.API.Extensions;
using Account.Domain.Enumerations;
using Core.Configurations;
using Core.Dtos;
using Core.Enumerations;
using Core.Messages; 
using Core.SeedWorks; 
using Core.Services.Interfaces;
using FluentValidation; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
namespace Account.API.API;

public static class MechanicUserApi
{
    const int _cacheExpiry = 10;
    public static IEndpointRouteBuilder MapMechanicUserApi(this IEndpointRouteBuilder builder)
    {
        var app = builder.MapGroup("api/v1/users/mechanic");

        app.MapPost("/", CreateMechanicUser) 
            .WithMetadata(new IdempotencyAttribute(nameof(CreateMechanicUser)))
            .RequireAuthorization(PolicyName.MechanicUserOnlyPolicy.ToString());

        app.MapGet("test", TestCreateMechanic);

        app.MapGet("/", GetPaginatedMechanicUser)
             .RequireAuthorization(PolicyName.AdministratorUserOnlyPolicy.ToString())
             .CacheOutput(config =>
             {
                 config.Expire(TimeSpan.FromSeconds(_cacheExpiry));
                 config.SetVaryByQuery(PaginatedRequestDto.PageNumberName, PaginatedRequestDto.PageSizeName);
             });

        app.MapGet("/{mechanicUserId}", GetMechanicUserById)
            .CacheOutput()
             .RequireAuthorization(PolicyName.MechanicUserAndAdministratorUserPolicy.ToString()); 

        app.MapGet("/{mechanicUserId}/certifications", GetPaginatedCertificationsByMechanicUserId)
             .RequireAuthorization(PolicyName.MechanicUserAndAdministratorUserPolicy.ToString())
             .CacheOutput(config =>
             {
                 config.Expire(TimeSpan.FromSeconds(_cacheExpiry));
                 config.SetVaryByQuery(PaginatedRequestDto.PageNumberName, PaginatedRequestDto.PageSizeName);
             }); 

        app.MapGet("/{mechanicUserId}/driving-license", GetDrivingLicenseByMechanicUserId)
            .CacheOutput()
            .RequireAuthorization(PolicyName.MechanicUserAndAdministratorUserPolicy.ToString());

        app.MapGet("/{mechanicUserId}/national-identity", GetNationalIdentityByMechanicUserId)
            .CacheOutput()
            .RequireAuthorization(PolicyName.MechanicUserAndAdministratorUserPolicy.ToString());


        app.MapPatch("/status/activate", ActivateMechanicStatus)
            .RequireAuthorization(PolicyName.MechanicUserOnlyPolicy.ToString());
        
        app.MapPatch("/status/deactivate", DeactivateMechanicStatus)
            .RequireAuthorization(PolicyName.MechanicUserOnlyPolicy.ToString());

        app.MapPatch("/{mechanicUserId}/rating", SetRating)
             .RequireAuthorization(PolicyName.AdministratorUserOnlyPolicy.ToString());

        app.MapPatch("/{mechanicUserId}/verify", VerifyMechanicUserByUserId)
             .RequireAuthorization(PolicyName.AdministratorUserOnlyPolicy.ToString());

        app.MapPatch("/{mechanicUserId}/driving-license/{drivingLicenseId}/confirm", ConfirmDrivingLicenseByUserId)
             .RequireAuthorization(PolicyName.AdministratorUserOnlyPolicy.ToString());

        app.MapPatch("/{mechanicUserId}/driving-license/{drivingLicenseId}/reject", RejectDrivingLicenseByUserId)
             .RequireAuthorization(PolicyName.AdministratorUserOnlyPolicy.ToString());

        app.MapPatch("/{mechanicUserId}/national-identity/{nationalIdentityId}/confirm", ConfirmNationalIdentityByUserId)
             .RequireAuthorization(PolicyName.AdministratorUserOnlyPolicy.ToString());

        app.MapPatch("/{mechanicUserId}/national-identity/{nationalIdentityId}/reject", RejectNationalIdentityByUserId)
             .RequireAuthorization(PolicyName.AdministratorUserOnlyPolicy.ToString());

        app.MapPatch("/device-id", SetDeviceIdByUserId)
             .RequireAuthorization(PolicyName.MechanicUserOnlyPolicy.ToString());

        app.MapPatch("/{mechanicUserId}/device-id/reset", ResetDeviceIdByUserId)
             .RequireAuthorization(PolicyName.MechanicUserAndAdministratorUserPolicy.ToString());

        app.MapPatch("/{mechanicUserId}/device-id/force-set", ForceSetDeviceIdByUserId)
             .RequireAuthorization(PolicyName.MechanicUserAndAdministratorUserPolicy.ToString());

        app.MapPatch("/order/{orderId}/accept", ConfirmOrderByMechanicUserId)
             .RequireAuthorization(PolicyName.MechanicUserOnlyPolicy.ToString());

        app.MapPatch("/order/{orderId}/reject", RejectOrderByMechanicUserId)
             .RequireAuthorization(PolicyName.MechanicUserOnlyPolicy.ToString());  

        app.MapPost("/driving-license", SetDrivingLicenseByUserId)
            .RequireAuthorization(PolicyName.MechanicUserOnlyPolicy.ToString());

        app.MapPost("/national-identity", SetNationalIdentityByUserId)
            .RequireAuthorization(PolicyName.MechanicUserOnlyPolicy.ToString());

        app.MapPut("/", UpdateMechanicUserByUserId) 
            .RequireAuthorization(PolicyName.MechanicUserOnlyPolicy.ToString());

        app.MapDelete("/{mechanicUserId}", DeleteMechanicUserByUserId)  
            .RequireAuthorization(PolicyName.AdministratorUserOnlyPolicy.ToString());





        app.MapPatch("/{mechanicId}/order/{orderId}/accept", TestAccept)
             .AllowAnonymous();

        app.MapPatch("/{mechanicId}/order/{orderId}/reject", TestReject)
             .AllowAnonymous();

        return app;
    }



    private static async Task<IResult> TestAccept(
        Guid mechanicId,
        Guid orderId,
        [FromServices] ApplicationService service)
    {
        try
        {  
            var result = await service.Mediator.Send(
                new AcceptOrderByMechanicUserIdCommand(orderId, mechanicId)); 
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }



    private static async Task<IResult> TestReject(
        Guid mechanicId,
        Guid orderId,
        [FromServices] ApplicationService service)
    {
        try
        { 
            var result = await service.Mediator.Send(
                new RejectOrderByMechanicUserIdCommand(orderId, mechanicId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }


    private static async Task<IResult> ConfirmOrderByMechanicUserId(
        Guid orderId,
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

            var result = await service.Mediator.Send(new AcceptOrderByMechanicUserIdCommand(orderId, userClaim.Sub));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }


    private static async Task<IResult> RejectOrderByMechanicUserId(
        Guid orderId,
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

            var result = await service.Mediator.Send(new RejectOrderByMechanicUserIdCommand(orderId, userClaim.Sub));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }


    private static async Task<IResult> ActivateMechanicStatus( 
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

            var result = await service.Mediator.Send(new ActivateMechanicStatusByUserIdCommand(userClaim.Sub));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> DeactivateMechanicStatus( 
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
             
            var result = await service.Mediator.Send(new DeactivateMechanicStatusByUserIdCommand(userClaim.Sub));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> GetNationalIdentityByMechanicUserId(
        Guid mechanicUserId, 
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

            if (userClaim.CurrentUserType != UserType.Administrator && mechanicUserId != userClaim.Sub)
            {
                return TypedResults.Forbid();
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
            var userClaim = userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Unauthorized();
            }

            if (userClaim.CurrentUserType != UserType.Administrator && mechanicUserId != userClaim.Sub)
            {
                return TypedResults.Forbid();
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
        [AsParameters] PaginatedRequestDto request,
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

            if (userClaim.CurrentUserType != UserType.Administrator && mechanicUserId != userClaim.Sub)
            {
                return TypedResults.Forbid();
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
        [AsParameters] PaginatedRequestDto request,
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
            var userClaim = userInfoService.GetUserInfo();
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
            var userClaim = userInfoService.GetUserInfo();
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
            var userClaim = userInfoService.GetUserInfo();
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
            var userClaim = userInfoService.GetUserInfo();
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
            var userClaim = userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Unauthorized();
            }

            if (userClaim.CurrentUserType != UserType.Administrator && mechanicUserId != userClaim.Sub)
            {
                return TypedResults.Forbid();
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
            var userClaim = userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Unauthorized();
            }

            if (userClaim.CurrentUserType != UserType.Administrator && mechanicUserId != userClaim.Sub)
            {
                return TypedResults.Forbid();
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
            var userClaim = userInfoService.GetUserInfo();
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
            var userClaim = userInfoService.GetUserInfo();
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
            var userClaim = userInfoService.GetUserInfo();
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
            var userClaim = userInfoService.GetUserInfo();
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
            var userClaim = userInfoService.GetUserInfo();
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
            var userClaim = userInfoService.GetUserInfo();
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
            var userClaim = userInfoService.GetUserInfo();
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
            var userClaim = userInfoService.GetUserInfo();
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
        [FromServices] IEncryptionService _kms,
        [FromServices] IOptionsMonitor<EncryptionConfiguration> options,
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

    private static async Task TestCreateMechanic(
        [FromServices] ApplicationService service)
    {

        string[] ids = {
                "2cfb79f6-8992-4fc8-bd7d-3e7145adf322",
                "367d4274-f3b9-456f-9019-ed09610d68eb",
                "6a60aca8-eef0-4b4a-8a73-a98b1272d671",
                "7c051a23-d2ad-44a5-8091-44382d8f5ab1",
                "7035b81a-ee66-4209-b505-ef6efbdd2881",
                "e85170c1-883c-42e7-8ba0-93b0e412271b",
                "a654d2af-4002-42ae-b8ef-b020254e636e",
                "f65eb0dc-5d4b-48d3-946c-22056989dcc5",
                "43f8590f-9a96-44fa-b151-65069ba6ff6d",
                "1718a5e1-0b90-43ff-b4a2-13f6eaaa5096" 
        };

        int index = 1;
        foreach (string id in ids)
        {
            var result = await service.Mediator.Send(new CreateMechanicUserCommand(
                Guid.Parse(id),
                $"erisekasaputra28{index}@gmail.com",
                $"0857913832{index}",
                "asia/jakarta",
                "",
                new PersonalInfoRequestDto("eris", "eka", "saputra", DateTime.UtcNow, Gender.Male, ""),
                new AddressRequestDto("Karangsono", null, null, "Blitar", "Jawa Timur", "66171", "IDN"),
                new List<CertificationRequestDto>()
                {
                    new ($"CERTSSSS{index}", "CERTSSSS", DateTime.UtcNow.AddMonths(1), []),
                },
                new DrivingLicenseRequestDto($"LICE1111NSE1{index}", "https://image.png", "https://image.png"),
                new NationalIdentityRequestDto($"IDENT1111ITY{index}", "https://image.png", "https://image.png"),
                $"DEVICEID123{index}"));

            await service.Mediator.Send(new ActivateMechanicStatusByUserIdCommand(Guid.Parse(id)));

            index++;
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
            var userClaim = userInfoService.GetUserInfo();
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
