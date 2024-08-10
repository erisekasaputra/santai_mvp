using Account.API.Applications.Commands.ConfirmBusinessLicenseByUserId;
using Account.API.Applications.Commands.ConfirmStaffEmailByStaffId;
using Account.API.Applications.Commands.ConfirmStaffPhoneNumberByStaffId;
using Account.API.Applications.Commands.CreateBusinessLicenseByUserId;
using Account.API.Applications.Commands.CreateBusinessUser;
using Account.API.Applications.Commands.CreateStaffBusinessUserByUserId;
using Account.API.Applications.Commands.DeleteBusinessUserByUserId;
using Account.API.Applications.Commands.ForceSetDeviceIdByStaffId;
using Account.API.Applications.Commands.RejectBusinessLicenseByUserId;
using Account.API.Applications.Commands.RemoveBusinessLicenseByUserId;
using Account.API.Applications.Commands.RemoveStaffByUserId;
using Account.API.Applications.Commands.ResetDeviceIdByStaffId;
using Account.API.Applications.Commands.SetDeviceIdByStaffId;
using Account.API.Applications.Commands.UpdateBusinessUserByUserId;
using Account.API.Applications.Commands.UpdateStaffByStaffId;
using Account.API.Applications.Commands.UpdateStaffPhoneNumberByStaffId;
using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Applications.Queries.GetBusinessUserByUserId;
using Account.API.Applications.Services;
using Account.API.Extensions;
using Account.API.SeedWork; 
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

        app.MapPost("/", CreateBusinessUser);//.WithMetadata(new IdempotencyAttribute());
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
        [FromServices] AppService service)
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

    private static async Task<IResult> SetStaffPhoneNumberByStaffId(
        Guid businessUserId,
        Guid staffId,
        [FromBody] EmailRequestDto request,
        [FromServices] AppService service,
        [FromServices] IValidator<EmailRequestDto> validator)
    {
        try
        {
            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
            {
                return TypedResults.BadRequest(validation.Errors);
            }

            var result = await service.Mediator.Send(new UpdateStaffPhoneNumberByStaffIdCommand(businessUserId, staffId, request.Email)); 
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
        [FromServices] AppService service)
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

    private static async Task<IResult> SetStaffEmailByStaffId(
        Guid businessUserId,
        Guid staffId,
        [FromBody] PhoneNumberRequestDto request,
        [FromServices] AppService service,
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
        [FromServices] AppService service,
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
        [FromServices] AppService service)
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
        [FromServices] AppService service,
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
        [FromServices] AppService service)
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
        [FromServices] AppService service)
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
        [FromServices] AppService service,
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

            var result = await service.Mediator.Send(new CreateBusinessLicenseByUserIdCommand(businessUserId, request)); 
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
        [FromServices] AppService service)
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
        [FromServices] AppService service,
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

            var result = await service.Mediator.Send(new CreateStaffBusinessUserByUserIdCommand(businessUserId, request));   
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
        [FromServices] AppService service)
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
        [FromServices] AppService service,
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

            var result = await service.Mediator.Send(new CreateBusinessUserCommand(request));  
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
        [FromBody] UpdateStaffByStaffIdCommand command,
        [FromServices] AppService service,
        [FromServices] IValidator<UpdateStaffByStaffIdCommand> validator)
    {
        try
        {  
            var validate = await validator.ValidateAsync(command); 
            if (!validate.IsValid)
            {
                var error = validate.Errors;
                return TypedResults.BadRequest(error);
            }

            var result = await service.Mediator.Send(command); 
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
        [FromBody] UpdateBusinessUserByUserIdCommand request,
        [FromServices] AppService service,
        [FromServices] IValidator<UpdateBusinessUserByUserIdCommand> validator)
    {
        try
        {
            if (businessUserId != request.Id)
            {
                return TypedResults.BadRequest("Business user id does not match");
            }

            var validate = await validator.ValidateAsync(request); 
            if (!validate.IsValid)
            {
                var error = validate.Errors; 
                return TypedResults.BadRequest(error);
            }

            var result = await service.Mediator.Send(request); 
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
        [FromServices] AppService service)
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
        [FromServices] AppService service)
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
