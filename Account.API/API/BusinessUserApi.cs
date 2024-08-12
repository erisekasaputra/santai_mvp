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
using Account.API.Extensions;
using Account.API.SeedWork;
using Account.API.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
namespace Account.API.API;

public static class BusinessUserApi
{
    public static IEndpointRouteBuilder MapBusinessUserApi(this IEndpointRouteBuilder route)
    {
        var app = route.MapGroup("api/v1/users/business");

        app.MapGet("/{businessUserId}", GetBusinessUserById); 
        app.MapPut("/{businessUserId}", UpdateBusinessUser);
        app.MapPut("/{businessUserId}/staffs/{staffId}", UpdateStaffByStaffId);

        app.MapPost("/", CreateBusinessUser).WithMetadata(new IdempotencyAttribute());
        app.MapPost("/{businessUserId}/staffs", CreateStaffBusinessUserById).WithMetadata(new IdempotencyAttribute());
        app.MapPost("/{businessUserId}/business-licenses", CreateBusinessLicenseBusinessUserById).WithMetadata(new IdempotencyAttribute()); 
     
        app.MapPatch("/{businessUserId}/staffs/{staffId}/device-id", SetDeviceIdByStaffId);
        app.MapPatch("/{businessUserId}/staffs/{staffId}/device-id/reset", ResetDeviceIdByStaffId);
        app.MapPatch("/{businessUserId}/staffs/{staffId}/device-id/force-set", ForceSetDeviceIdByStaffId); 
        app.MapPatch("/{businessUserId}/staffs/{staffId}/email", SetStaffEmailByStaffId);
        app.MapPatch("/{businessUserId}/staffs/{staffId}/email/confirm", ConfirmStaffEmailByStaffId);
        app.MapPatch("/{businessUserId}/staffs/{staffId}/phone-number", SetStaffPhoneNumberByStaffId);
        app.MapPatch("/{businessUserId}/staffs/{staffId}/phone-number/confirm", ConfirmStaffPhoneNumberByStaffId); 
        app.MapPatch("/{businessUserId}/business-licenses/{businessLicenseId}/reject", RejectBusinessLicenseByUserId); 
        app.MapPatch("/{businessUserId}/business-licenses/{businessLicenseId}/confirm", ConfirmBusinessLicenseByUserId);
          
        app.MapDelete("/{businessUserId}", DeleteBusinessUserById);
        app.MapDelete("/{businessUserId}/staffs/{staffId}", RemoveStaffBusinessUserById);
        app.MapDelete("/{businessUserId}/business-licenses/{businessLicenseId}", RemoveBusinessLicenseBusinessUserById);

        return route;
    }

    private static async Task<IResult> ConfirmStaffPhoneNumberByStaffId(
        Guid businessUserId,
        Guid staffId, 
        [FromServices] ApplicationService service)
    {
        try
        { 
            var result = await service.Mediator.Send(new ConfirmStaffPhoneNumberByStaffIdCommand(businessUserId, staffId)); 
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> SetStaffEmailByStaffId(
        Guid businessUserId,
        Guid staffId,
        [FromBody] EmailRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<EmailRequestDto> validator)
    {
        try
        {
            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
            {
                return TypedResults.BadRequest(validation.Errors);
            }

            var result = await service.Mediator.Send(new UpdateStaffEmailByStaffIdCommand(businessUserId, staffId, request.Email)); 
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> ConfirmStaffEmailByStaffId(
        Guid businessUserId,
        Guid staffId, 
        [FromServices] ApplicationService service)
    {
        try
        {
            var result = await service.Mediator.Send(new ConfirmStaffEmailByStaffIdCommand(businessUserId, staffId)); 
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> SetStaffPhoneNumberByStaffId(
        Guid businessUserId,
        Guid staffId,
        [FromBody] PhoneNumberRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<PhoneNumberRequestDto> validator)
    {
        try
        {
            var validation = await validator.ValidateAsync(request); 
            if (!validation.IsValid)
            { 
                return TypedResults.BadRequest(validation.Errors);
            }

            var result = await service.Mediator.Send(new UpdateStaffPhoneNumberByStaffIdCommand(businessUserId, staffId, request.PhoneNumber)); 
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    } 

    private static async Task<IResult> ForceSetDeviceIdByStaffId(
        Guid businessUserId,
        Guid staffId,
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

            var result = await service.Mediator.Send(new ForceSetDeviceIdByStaffIdCommand(businessUserId, staffId, request.DeviceId)); 
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> ResetDeviceIdByStaffId(
        Guid businessUserId,
        Guid staffId, 
        [FromServices] ApplicationService service)
    {
        try
        { 
            var result = await service.Mediator.Send(new ResetDeviceIdByStaffIdCommand(businessUserId, staffId)); 
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> SetDeviceIdByStaffId(
        Guid businessUserId,
        Guid staffId,
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
             

            var result = await service.Mediator.Send(new SetDeviceIdByStaffIdCommand(businessUserId, staffId, request.DeviceId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> ConfirmBusinessLicenseByUserId(
        Guid businessUserId,
        Guid businessLicenseId,
        [FromServices] ApplicationService service)
    {
        try
        {
            var result = await service.Mediator.Send(new ConfirmBusinessLicenseByUserIdCommand(businessUserId, businessLicenseId));
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> RejectBusinessLicenseByUserId(
        Guid businessUserId,
        Guid businessLicenseId,
        [FromServices] ApplicationService service)
    {
        try
        {
            var result = await service.Mediator.Send(new RejectBusinessLicenseByUserIdCommand(businessUserId, businessLicenseId));
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> CreateBusinessLicenseBusinessUserById(
        Guid businessUserId,
        [FromBody] BusinessLicenseRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<BusinessLicenseRequestDto> validator)
    {
        try
        { 
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
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> RemoveBusinessLicenseBusinessUserById(
        Guid businessUserId,
        Guid businessLicenseId,
        [FromServices] ApplicationService service)
    {
        try
        {
            var result = await service.Mediator.Send(new RemoveBusinessLicenseByUserIdCommand(businessUserId, businessLicenseId)); 
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }


    private static async Task<IResult> CreateStaffBusinessUserById(
        Guid businessUserId,
        [FromBody] StaffRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<StaffRequestDto> validator)
    {
        try
        {  
            var validate = await validator.ValidateAsync(request); 
            if (!validate.IsValid)
            {
                var error = validate.Errors;
                return TypedResults.BadRequest(error);
            }

            var result = await service.Mediator.Send(new CreateStaffBusinessUserByUserIdCommand(
                businessUserId,
                request.Username,
                request.PhoneNumber,
                request.Email,
                request.Name,
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

    private static async Task<IResult> RemoveStaffBusinessUserById(
        Guid businessUserId,
        Guid staffId,
        [FromServices] ApplicationService service)
    {
        try
        { 
            var result = await service.Mediator.Send(new RemoveStaffByUserIdCommand(businessUserId, staffId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }



    private static async Task<IResult> CreateBusinessUser(
        [FromBody] BusinessUserRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<BusinessUserRequestDto> validator)
    {
        try
        {
            var validate = await validator.ValidateAsync(request); 
            if (!validate.IsValid)
            {
                var error = validate.Errors;
                return TypedResults.BadRequest(error);
            }

            var result = await service.Mediator.Send(new CreateBusinessUserCommand(
                request.IdentityId,
                request.Username,
                request.Email,
                request.PhoneNumber,
                request.TimeZoneId,
                request.Address,
                request.BusinessName,
                request.ContactPerson,
                request.TaxId,
                request.WebsiteUrl,
                request.BusinessDescription,
                request.ReferralCode,
                request.BusinessLicenses,
                request.Staffs
                ));  
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }
     
    private static async Task<IResult> UpdateStaffByStaffId(
        Guid businessUserId,
        Guid StaffId,
        [FromBody] UpdateStaffRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<UpdateStaffRequestDto> validator)
    {
        try
        {  
            var validate = await validator.ValidateAsync(request); 
            if (!validate.IsValid)
            {
                var error = validate.Errors;
                return TypedResults.BadRequest(error);
            }

            var result = await service.Mediator.Send(new UpdateStaffByStaffIdCommand(businessUserId, StaffId, request.Name, request.Address, request.TimeZoneId)); 
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> UpdateBusinessUser(
        Guid businessUserId,
        [FromBody] UpdateBusinessUserRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<UpdateBusinessUserRequestDto> validator)
    {
        try
        { 
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
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        } 
    }
    
    private static async Task<IResult> GetBusinessUserById(
        Guid businessUserId,
        [FromServices] ApplicationService service)
    {
        try
        {
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
        [FromServices] ApplicationService service)
    {
        try
        {
            var command = new DeleteBusinessUserByUserIdCommand(businessUserId);
            var result = await service.Mediator.Send(command); 
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }
}
