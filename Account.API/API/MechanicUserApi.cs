using Account.API.Applications.Commands.MechanicUserCommand.ActivateMechanicStatusByUserId;
using Account.API.Applications.Commands.MechanicUserCommand.ConfirmDrivingLicenseByUserId;
using Account.API.Applications.Commands.MechanicUserCommand.ConfirmNationalIdentityByUserId;
using Account.API.Applications.Commands.MechanicUserCommand.CreateMechanicUser;
using Account.API.Applications.Commands.MechanicUserCommand.DeactivateMechanicStatusByUserId;
using Account.API.Applications.Commands.MechanicUserCommand.DeleteMechanicUserByUserId;
using Account.API.Applications.Commands.MechanicUserCommand.RejectDrivingLicenseByUserId;
using Account.API.Applications.Commands.MechanicUserCommand.RejectNationalIdentityByUserId;
using Account.API.Applications.Commands.MechanicUserCommand.SetDrivingLicenseByUserId;
using Account.API.Applications.Commands.MechanicUserCommand.SetNationalIdentityByUserId;
using Account.API.Applications.Commands.MechanicUserCommand.SetRatingByUserId;
using Account.API.Applications.Commands.MechanicUserCommand.UpdateLocationByUserId;
using Account.API.Applications.Commands.MechanicUserCommand.UpdateMechanicUserByUserId;
using Account.API.Applications.Commands.MechanicUserCommand.UpdateProfilePictureMechanicUser;
using Account.API.Applications.Commands.MechanicUserCommand.VerifyMechanicUserByUserId;
using Account.API.Applications.Commands.OrderTaskCommand.AcceptOrderByMechanicUserId;
using Account.API.Applications.Commands.OrderTaskCommand.RejectOrderMechanicByUserId; 
using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Applications.Queries.GetDrivingLicenseByMechanicUserId;
using Account.API.Applications.Queries.GetMechanicStatusExistence;
using Account.API.Applications.Queries.GetMechanicUserById;
using Account.API.Applications.Queries.GetNationalIdentityByMechanicUserId;
using Account.API.Applications.Queries.GetPaginatedMechanicCertificationByUserId;
using Account.API.Applications.Queries.GetPaginatedMechanicUser;
using Account.API.Applications.Services; 
using Account.API.Extensions; 
using Core.Configurations;
using Core.CustomAttributes;
using Core.CustomMessages;
using Core.Dtos;
using Core.Enumerations;
using Core.Results;
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

        app.MapPatch("/image/profile", UpdateMechanicUserImageProfile)
            .RequireAuthorization(PolicyName.RegularUserOnlyPolicy.ToString()); 

        app.MapPost("/", CreateMechanicUser) 
            .WithMetadata(new IdempotencyAttribute(nameof(CreateMechanicUser)))
            .RequireAuthorization(PolicyName.MechanicUserOnlyPolicy.ToString()); 

        app.MapGet("/", GetPaginatedMechanicUser)
             .RequireAuthorization(PolicyName.AdministratorUserOnlyPolicy.ToString());

        app.MapGet("/{mechanicUserId}", GetMechanicUserById) 
             .RequireAuthorization(PolicyName.MechanicUserAndAdministratorUserPolicy.ToString());

        app.MapGet("/{mechanicUserId}/certifications", GetPaginatedCertificationsByMechanicUserId)
             .RequireAuthorization(PolicyName.MechanicUserAndAdministratorUserPolicy.ToString());

        app.MapGet("/{mechanicUserId}/driving-license", GetDrivingLicenseByMechanicUserId) 
            .RequireAuthorization(PolicyName.MechanicUserAndAdministratorUserPolicy.ToString());

        app.MapGet("/{mechanicUserId}/national-identity", GetNationalIdentityByMechanicUserId) 
            .RequireAuthorization(PolicyName.MechanicUserAndAdministratorUserPolicy.ToString());

        app.MapGet("/status", GetMechanicStatusExistence)
            .RequireAuthorization(PolicyName.MechanicUserOnlyPolicy.ToString());   
        
        app.MapPatch("/location", UpdateMechanicLocation)
            .RequireAuthorization(PolicyName.MechanicUserOnlyPolicy.ToString());

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

        app.MapPatch("{mechanicId}/order/{orderId}/accept", TestAccept)
          .AllowAnonymous();

        return app;
    }

    private static async Task<IResult> UpdateMechanicUserImageProfile(
      [FromBody] UpdateUserProfilePictureRequestDto request,
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

            if (string.IsNullOrEmpty(request.ImageUrl))
            {
                return TypedResults.BadRequest(Result.Failure("Image url can not be empty", ResponseStatus.BadRequest));
            }

            var result = await service.Mediator.Send(new UpdateProfilePictureMechanicUserCommand(
                userClaim.Sub,
                request.ImageUrl
                ));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
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
    
    private static async Task<IResult> UpdateMechanicLocation(
        [AsParameters] LocationRequestDto location, 
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

            var result = await service.Mediator.Send(new UpdateLocationByUserIdCommand(
                userClaim.Sub,
                location.Latitude,
                location.Longitude));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    } 


    private static async Task<IResult> GetMechanicStatusExistence( 
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

            var result = await service.Mediator.Send(new GetMechanicStatusExistenceQuery(userClaim.Sub));

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
                request.NationalIdentity));
             
            return result.ToIResult();
        }
        catch (Exception ex) 
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }
}
