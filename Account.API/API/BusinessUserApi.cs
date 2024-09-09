using Account.API.Applications.Commands.BusinessUserCommand.ConfirmBusinessLicenseByUserId;
using Account.API.Applications.Commands.BusinessUserCommand.CreateBusinessLicenseByUserId;
using Account.API.Applications.Commands.BusinessUserCommand.CreateBusinessUser;
using Account.API.Applications.Commands.BusinessUserCommand.DeleteBusinessUserByUserId;
using Account.API.Applications.Commands.BusinessUserCommand.RejectBusinessLicenseByUserId;
using Account.API.Applications.Commands.BusinessUserCommand.RemoveBusinessLicenseByUserId;
using Account.API.Applications.Commands.BusinessUserCommand.UpdateBusinessUserByUserId;
using Account.API.Applications.Commands.StaffCommand.ConfirmStaffEmailByStaffId;
using Account.API.Applications.Commands.StaffCommand.ConfirmStaffPhoneNumberByStaffId;
using Account.API.Applications.Commands.StaffCommand.CreateStaffBusinessUserByUserId;
using Account.API.Applications.Commands.StaffCommand.ForceSetDeviceIdByStaffId;
using Account.API.Applications.Commands.StaffCommand.RemoveStaffByUserId;
using Account.API.Applications.Commands.StaffCommand.ResetDeviceIdByStaffId;
using Account.API.Applications.Commands.StaffCommand.SetDeviceIdByStaffId;
using Account.API.Applications.Commands.StaffCommand.UpdateStaffByStaffId;
using Account.API.Applications.Commands.StaffCommand.UpdateStaffEmailByStaffId;
using Account.API.Applications.Commands.StaffCommand.UpdateStaffPhoneNumberByStaffId;
using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Applications.Queries.GetBusinessUserByUserId;
using Account.API.Applications.Queries.GetDeviceIdByStaffId;
using Account.API.Applications.Queries.GetEmailByStaffId;
using Account.API.Applications.Queries.GetPaginatedBusinessLicenseByUserId;
using Account.API.Applications.Queries.GetPaginatedBusinessUser;
using Account.API.Applications.Queries.GetPaginatedStaffByUserId;
using Account.API.Applications.Queries.GetPhoneNumberByStaffId;
using Account.API.Applications.Queries.GetStaffByUserIdAndStaffId;
using Account.API.Applications.Queries.GetTimeZoneByStaffId;
using Account.API.Applications.Services; 
using Account.API.CustomAttributes;
using Account.API.Extensions;
using Core.Dtos;
using Core.Enumerations;
using Core.Messages;
using Core.SeedWorks;
using Core.Services.Interfaces;
using FluentValidation; 
using Microsoft.AspNetCore.Mvc;
namespace Account.API.API;

public static class BusinessUserApi
{
    const int _cacheExpiry = 10;
    public static IEndpointRouteBuilder MapBusinessUserApi(this IEndpointRouteBuilder route)
    {
        var app = route.MapGroup("api/v1/users/business");
        
        app.MapGet("/", GetPaginatedBusinessUser)
            .RequireAuthorization(PolicyName.AdministratorPolicy.ToString())
            .CacheOutput(config =>
            {
                config.Expire(TimeSpan.FromSeconds(_cacheExpiry));
                config.SetVaryByQuery(PaginatedRequestDto.PageNumberName, PaginatedRequestDto.PageSizeName);
            });

        app.MapGet("/{businessUserId}/staffs", GetPaginatedStaff) 
            .RequireAuthorization(
                PolicyName.AdministratorPolicy.ToString(), 
                PolicyName.BusinessUserPolicy.ToString())
            .CacheOutput(config =>
            {
                config.Expire(TimeSpan.FromSeconds(_cacheExpiry));
                config.SetVaryByQuery(PaginatedRequestDto.PageNumberName, PaginatedRequestDto.PageSizeName);
            });

        app.MapGet("/{businessUserId}/business-licenses", GetPaginatedBusinessLicense)
            .RequireAuthorization(
                PolicyName.AdministratorPolicy.ToString(), 
                PolicyName.BusinessUserPolicy.ToString())
            .CacheOutput(config =>
            {
                config.Expire(TimeSpan.FromSeconds(_cacheExpiry));
                config.SetVaryByQuery(PaginatedRequestDto.PageNumberName, PaginatedRequestDto.PageSizeName);
            });

        app.MapGet("/{businessUserId}", GetBusinessUserById)
            .CacheOutput()
            .RequireAuthorization(
                PolicyName.AdministratorPolicy.ToString(),
                PolicyName.BusinessUserPolicy.ToString());

        app.MapGet("/{businessUserId}/staffs/{staffId}", GetStaffByUserIdAndStaffId)
            .CacheOutput()
            .RequireAuthorization(
                PolicyName.AdministratorPolicy.ToString(), 
                PolicyName.BusinessUserPolicy.ToString(), 
                PolicyName.StaffUserPolicy.ToString()); 



        app.MapGet("/staffs/email", GetEmailByStaffId)
            .CacheOutput()
            .RequireAuthorization(
                PolicyName.StaffUserPolicy.ToString());

        app.MapGet("/staffs/phone-number", GetPhoneNumberByStaffId)
            .CacheOutput()
            .RequireAuthorization(
                PolicyName.StaffUserPolicy.ToString());

        app.MapGet("/staffs/time-zone", GetTimeZoneByStaffId)
            .CacheOutput()
            .RequireAuthorization(
                PolicyName.StaffUserPolicy.ToString());

        app.MapGet("/staffs/device-id", GetDeviceIdByStaffId)
            .CacheOutput()
            .RequireAuthorization(
                PolicyName.StaffUserPolicy.ToString());


        app.MapPut("/{businessUserId}", UpdateBusinessUser)
            .RequireAuthorization(
                PolicyName.AdministratorPolicy.ToString(), 
                PolicyName.BusinessUserPolicy.ToString());

        app.MapPut("/{businessUserId}/staffs/{staffId}", UpdateStaffByStaffId)
            .RequireAuthorization(
                PolicyName.AdministratorPolicy.ToString(),
                PolicyName.BusinessUserPolicy.ToString(), 
                PolicyName.StaffUserPolicy.ToString());


        app.MapPost("/", CreateBusinessUser)
            .WithMetadata(new IdempotencyAttribute(nameof(CreateBusinessUser)))
            .RequireAuthorization(PolicyName.AdministratorPolicy.ToString());

        app.MapPost("/{businessUserId}/staffs", CreateStaffBusinessUserById)
            .WithMetadata(new IdempotencyAttribute(nameof(CreateStaffBusinessUserById)))
            .RequireAuthorization(
                PolicyName.BusinessUserPolicy.ToString(), 
                PolicyName.AdministratorPolicy.ToString());

        app.MapPost("/{businessUserId}/business-licenses", CreateBusinessLicenseBusinessUserById)
            .WithMetadata(new IdempotencyAttribute(nameof(CreateBusinessLicenseBusinessUserById)))
            .RequireAuthorization(PolicyName.BusinessUserPolicy.ToString(), PolicyName.AdministratorPolicy.ToString());


        app.MapPatch("/staffs/device-id", SetDeviceIdByStaffId)
            .RequireAuthorization(PolicyName.StaffUserPolicy.ToString());

        app.MapPatch("/staffs/{staffId}/device-id/reset", ResetDeviceIdByStaffId)
            .RequireAuthorization(PolicyName.StaffUserPolicy.ToString(), PolicyName.AdministratorPolicy.ToString());

        app.MapPatch("/staffs/{staffId}/device-id/force-set", ForceSetDeviceIdByStaffId)
            .RequireAuthorization(PolicyName.StaffUserPolicy.ToString(), PolicyName.AdministratorPolicy.ToString());

        app.MapPatch("/staffs/email", SetStaffEmailByStaffId)
            .RequireAuthorization(PolicyName.StaffUserPolicy.ToString());

        app.MapPatch("/staffs/email/confirm", ConfirmStaffEmailByStaffId)
            .RequireAuthorization(PolicyName.StaffUserPolicy.ToString());

        app.MapPatch("/staffs/phone-number", SetStaffPhoneNumberByStaffId)
            .RequireAuthorization(PolicyName.StaffUserPolicy.ToString());

        app.MapPatch("/staffs/phone-number/confirm", ConfirmStaffPhoneNumberByStaffId)
            .RequireAuthorization(PolicyName.StaffUserPolicy.ToString());

        app.MapPatch("/{businessUserId}/business-licenses/{businessLicenseId}/reject", RejectBusinessLicenseByUserId)
            .RequireAuthorization(PolicyName.AdministratorPolicy.ToString());

        app.MapPatch("/{businessUserId}/business-licenses/{businessLicenseId}/confirm", ConfirmBusinessLicenseByUserId)
            .RequireAuthorization(PolicyName.AdministratorPolicy.ToString());


        app.MapDelete("/{businessUserId}", DeleteBusinessUserById)
            .RequireAuthorization(PolicyName.AdministratorPolicy.ToString());

        app.MapDelete("/{businessUserId}/staffs/{staffId}", RemoveStaffBusinessUserById)
            .RequireAuthorization(PolicyName.AdministratorPolicy.ToString(), PolicyName.BusinessUserPolicy.ToString());

        app.MapDelete("/{businessUserId}/business-licenses/{businessLicenseId}", RemoveBusinessLicenseBusinessUserById)
            .RequireAuthorization(PolicyName.AdministratorPolicy.ToString(), PolicyName.BusinessUserPolicy.ToString());

        return route;
    }

    private static async Task<IResult> GetDeviceIdByStaffId( 
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

            var result = await service.Mediator.Send(
                new GetDeviceIdByStaffIdQuery(userClaim.Sub));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> GetTimeZoneByStaffId( 
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

            var result = await service.Mediator.Send(new GetTimeZoneByStaffIdQuery(userClaim.Sub));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> GetPhoneNumberByStaffId( 
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

            var result = await service.Mediator.Send(new GetPhoneNumberByStaffIdQuery(userClaim.Sub));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> GetEmailByStaffId( 
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

            var result = await service.Mediator.Send(new GetEmailByStaffIdQuery(userClaim.Sub));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    } 



    private static async Task<IResult> GetStaffByUserIdAndStaffId(
        Guid businessUserId, 
        Guid staffId,
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

            if (userClaim.CurrentUserType == UserType.BusinessUser && businessUserId != userClaim.Sub) 
            {
                return TypedResults.Forbid();
            }

            if (userClaim.CurrentUserType == UserType.StaffUser && staffId != userClaim.Sub)
            {
                return TypedResults.Forbid();
            }

            var result = await service.Mediator.Send(new GetStaffByUserIdAndStaffIdQuery(businessUserId, staffId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> GetPaginatedBusinessLicense(
        Guid businessUserId, 
        [AsParameters] PaginatedRequestDto request,
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

            if (userClaim.CurrentUserType != UserType.Administrator && businessUserId != userClaim.Sub)
            {
                return TypedResults.Forbid();
            }

            var result = await service.Mediator.Send(new GetPaginatedBusinessLicenseByUserIdQuery(
                businessUserId,
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

    private static async Task<IResult> GetPaginatedStaff(
        Guid businessUserId, 
        [AsParameters] PaginatedRequestDto request,
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

            if (userClaim.CurrentUserType != UserType.Administrator && businessUserId != userClaim.Sub)
            {
                return TypedResults.Forbid();
            } 

            var result = await service.Mediator.Send(new GetPaginatedStaffByUserIdQuery(
                businessUserId, 
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

    private static async Task<IResult> GetPaginatedBusinessUser(
        [AsParameters] PaginatedRequestDto request,
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

            var result = await service.Mediator.Send(new GetPaginatedBusinessUserQuery(
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

    private static async Task<IResult> ConfirmStaffPhoneNumberByStaffId( 
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

            var result = await service.Mediator.Send(new ConfirmStaffPhoneNumberByStaffIdCommand(userClaim.Sub)); 
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> SetStaffEmailByStaffId( 
        [FromBody] EmailRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<EmailRequestDto> validator,
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

            var result = await service.Mediator.Send(new UpdateStaffEmailByStaffIdCommand(userClaim.Sub, request.Email)); 
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> ConfirmStaffEmailByStaffId( 
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

            var result = await service.Mediator.Send(new ConfirmStaffEmailByStaffIdCommand(userClaim.Sub)); 
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> SetStaffPhoneNumberByStaffId( 
        [FromBody] PhoneNumberRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<PhoneNumberRequestDto> validator,
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

            var result = await service.Mediator.Send(new UpdateStaffPhoneNumberByStaffIdCommand(userClaim.Sub, request.PhoneNumber)); 
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    } 

    private static async Task<IResult> ForceSetDeviceIdByStaffId( 
        Guid staffId,
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

            if (userClaim.CurrentUserType == UserType.StaffUser && staffId != userClaim.Sub)
            {
                return TypedResults.Forbid();
            }

            var validation = await validator.ValidateAsync(request); 
            if (!validation.IsValid)
            {
                return TypedResults.BadRequest(validation.Errors);
            }

            var result = await service.Mediator.Send(new ForceSetDeviceIdByStaffIdCommand(staffId, request.DeviceId)); 
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> ResetDeviceIdByStaffId( 
        Guid staffId, 
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
             
            if (userClaim.CurrentUserType == UserType.StaffUser && staffId != userClaim.Sub)
            {
                return TypedResults.Forbid();
            }

            var result = await service.Mediator.Send(new ResetDeviceIdByStaffIdCommand(staffId)); 
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> SetDeviceIdByStaffId( 
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
             

            var result = await service.Mediator.Send(new SetDeviceIdByStaffIdCommand(userClaim.Sub, request.DeviceId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> ConfirmBusinessLicenseByUserId(
        Guid businessUserId,
        Guid businessLicenseId,
        [FromServices] ApplicationService service,
        [FromServices] IUserInfoService userInfoService)
    {
        try
        {
            var result = await service.Mediator.Send(new ConfirmBusinessLicenseByUserIdCommand(businessUserId, businessLicenseId));
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> RejectBusinessLicenseByUserId(
        Guid businessUserId,
        Guid businessLicenseId,
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

            var result = await service.Mediator.Send(new RejectBusinessLicenseByUserIdCommand(businessUserId, businessLicenseId));
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> CreateBusinessLicenseBusinessUserById(
        Guid businessUserId,
        [FromBody] BusinessLicenseRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<BusinessLicenseRequestDto> validator,
        [FromServices] IUserInfoService userInfoService)
    {
        try
        {
            var userClaim = userInfoService.GetUserInfoAsync();
            if (userClaim is null)
            {
                return TypedResults.Unauthorized();
            }

            if (userClaim.CurrentUserType == UserType.BusinessUser && businessUserId != userClaim.Sub)
            {
                return TypedResults.Forbid();
            } 

            var validate = await validator.ValidateAsync(request); 
            if (!validate.IsValid)
            {
                var error = validate.Errors;
                return TypedResults.BadRequest(error);
            }

            var result = await service.Mediator.Send(new CreateBusinessLicenseByUserIdCommand(businessUserId, request.LicenseNumber, request.Name, request.Description)); 
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> RemoveBusinessLicenseBusinessUserById(
        Guid businessUserId,
        Guid businessLicenseId,
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


            if (userClaim.CurrentUserType == UserType.BusinessUser && businessUserId != userClaim.Sub)
            {
                return TypedResults.Forbid();
            }  

            var result = await service.Mediator.Send(new RemoveBusinessLicenseByUserIdCommand(businessUserId, businessLicenseId)); 
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }


    private static async Task<IResult> CreateStaffBusinessUserById(
        Guid businessUserId,
        [FromBody] StaffRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<StaffRequestDto> validator,
        [FromServices] IUserInfoService userInfoService)
    {
        try
        {
            var userClaim = userInfoService.GetUserInfoAsync();
            if (userClaim is null)
            {
                return TypedResults.Unauthorized();
            }


            if (userClaim.CurrentUserType == UserType.BusinessUser && businessUserId != userClaim.Sub)
            {
                return TypedResults.Forbid();
            } 


            var validate = await validator.ValidateAsync(request); 
            if (!validate.IsValid)
            {
                var error = validate.Errors;
                return TypedResults.BadRequest(error);
            }

            var result = await service.Mediator.Send(new CreateStaffBusinessUserByUserIdCommand(
                businessUserId, 
                request.PhoneNumber, 
                request.Name,
                request.Address,
                request.TimeZoneId,
                request.Password));   

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> RemoveStaffBusinessUserById(
        Guid businessUserId,
        Guid staffId,
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

            if (userClaim.CurrentUserType == UserType.BusinessUser && businessUserId != userClaim.Sub)
            {
                return TypedResults.Forbid();
            } 

            var result = await service.Mediator.Send(new RemoveStaffByUserIdCommand(businessUserId, staffId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }



    private static async Task<IResult> CreateBusinessUser(
        [FromBody] BusinessUserRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<BusinessUserRequestDto> validator,
        [FromServices] IUserInfoService userInfoService)
    {
        try
        {
            var userClaim = userInfoService.GetUserInfoAsync();
            if (userClaim is null)
            {
                return TypedResults.Unauthorized();
            }

            var validate = await validator.ValidateAsync(request); 
            if (!validate.IsValid)
            {
                var error = validate.Errors;
                return TypedResults.BadRequest(error);
            } 

            var result = await service.Mediator.Send(new CreateBusinessUserCommand(  
                request.PhoneNumber,
                request.TimeZoneId,
                request.Address,
                request.BusinessName,
                request.ContactPerson,
                request.TaxId,
                request.WebsiteUrl,
                request.BusinessDescription,
                request.ReferralCode,
                request.Password,
                request.BusinessLicenses,
                request.Staffs));
            
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }
     
    private static async Task<IResult> UpdateStaffByStaffId(
        Guid businessUserId,
        Guid staffId,
        [FromBody] UpdateStaffRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<UpdateStaffRequestDto> validator,
        [FromServices] IUserInfoService userInfoService)
    {
        try
        {
            var userClaim = userInfoService.GetUserInfoAsync();
            if (userClaim is null)
            {
                return TypedResults.Unauthorized();
            }

            if (userClaim.CurrentUserType == UserType.BusinessUser && businessUserId != userClaim.Sub)
            {
                return TypedResults.Forbid();
            }

            if (userClaim.CurrentUserType == UserType.StaffUser && staffId != userClaim.Sub)
            {
                return TypedResults.Forbid();
            }


            var validate = await validator.ValidateAsync(request); 
            if (!validate.IsValid)
            {
                var error = validate.Errors;
                return TypedResults.BadRequest(error);
            }

            var result = await service.Mediator.Send(new UpdateStaffByStaffIdCommand(businessUserId, staffId, request.Name, request.Address, request.TimeZoneId)); 
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> UpdateBusinessUser(
        Guid businessUserId,
        [FromBody] UpdateBusinessUserRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<UpdateBusinessUserRequestDto> validator,
        [FromServices] IUserInfoService userInfoService)
    {
        try
        {
            var userClaim = userInfoService.GetUserInfoAsync();
            if (userClaim is null)
            {
                return TypedResults.Unauthorized();
            }

            if (userClaim.CurrentUserType != UserType.Administrator && businessUserId != userClaim.Sub)
            {
                return TypedResults.Forbid();
            }
             

            var validate = await validator.ValidateAsync(request); 
            if (!validate.IsValid)
            {
                var error = validate.Errors; 
                return TypedResults.BadRequest(error);
            }

            var result = await service.Mediator.Send(new UpdateBusinessUserByUserIdCommand(
                businessUserId,
                request.BusinessName,
                request.ContactPerson,
                request.TaxId,
                request.WebsiteUrl,
                request.Description,
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
    
    private static async Task<IResult> GetBusinessUserById(
        Guid businessUserId,
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

            if (userClaim.CurrentUserType != UserType.Administrator && businessUserId != userClaim.Sub)
            {
                return TypedResults.Forbid();
            }

            var query = new GetBusinessUserByUserIdQuery(businessUserId);
            var result = await service.Mediator.Send(query);

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        } 
    }

    private static async Task<IResult> DeleteBusinessUserById(
        Guid businessUserId,
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

            var command = new DeleteBusinessUserByUserIdCommand(businessUserId);
            var result = await service.Mediator.Send(command); 
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }
}
