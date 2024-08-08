using Account.API.Applications.Commands.ConfirmBusinessLicenseByUserId;
using Account.API.Applications.Commands.CreateBusinessLicenseByUserId;
using Account.API.Applications.Commands.CreateBusinessUser;
using Account.API.Applications.Commands.CreateStaffBusinessUserByUserId;
using Account.API.Applications.Commands.DeleteBusinessUserByUserId;
using Account.API.Applications.Commands.RejectBusinessLicenseByUserId;
using Account.API.Applications.Commands.RemoveBusinessLicenseByUserId;
using Account.API.Applications.Commands.RemoveStaffByUserId;
using Account.API.Applications.Commands.UpdateBusinessUserByUserId;
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

        app.MapPost("/", CreateBusinessUser).WithMetadata(new IdempotencyAttribute());
        app.MapPut("/{businessUserId}", UpdateBusinessUser);
        
        app.MapGet("/{businessUserId}", GetBusinessUserById);
        app.MapDelete("/{businessUserId}", DeleteBusinessUserById);

        app.MapPost("/{businessUserId}/staffs", CreateStaffBusinessUserById).WithMetadata(new IdempotencyAttribute());
        app.MapDelete("/{businessUserId}/staffs/{staffId}", RemoveStaffBusinessUserById);
        
        app.MapPost("/{businessUserId}/business-licenses", CreateBusinessLicenseBusinessUserById).WithMetadata(new IdempotencyAttribute());
        app.MapDelete("/{businessUserId}/business-licenses/{businessLicenseId}", RemoveBusinessLicenseBusinessUserById);
        app.MapPatch("/{businessUserId}/business-licenses/{businessLicenseId}/confirm", ConfirmBusinessLicenseByUserId);
        app.MapPatch("/{businessUserId}/business-licenses/{businessLicenseId}/reject", RejectBusinessLicenseByUserId); 

        return route;
    }

    private static async Task<IResult> ConfirmBusinessLicenseByUserId(Guid businessUserId, Guid businessLicenseId, [FromServices] AppService service)
    {
        try
        {
            var result = await service.Mediator.Send(new ConfirmBusinessLicenseByUserIdCommand(businessUserId, businessLicenseId));
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message);
            return TypedResults.InternalServerError("An error occurred while confirming business license");
        }
    }

    private static async Task<IResult> RejectBusinessLicenseByUserId(Guid businessUserId, Guid businessLicenseId, [FromServices] AppService service)
    {
        try
        {
            var result = await service.Mediator.Send(new RejectBusinessLicenseByUserIdCommand(businessUserId, businessLicenseId));
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message);
            return TypedResults.InternalServerError();
        }
    }

    private static async Task<IResult> CreateBusinessLicenseBusinessUserById(Guid businessUserId, [FromBody] BusinessLicenseRequestDto request, [FromServices] AppService service, [FromServices] IValidator<BusinessLicenseRequestDto> validator)
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
            service.Logger.LogError(ex.Message);
            return TypedResults.InternalServerError();
        }
    }

    private static async Task<IResult> RemoveBusinessLicenseBusinessUserById(Guid businessUserId, Guid businessLicenseId, [FromServices] AppService service)
    {
        try
        {
            var result = await service.Mediator.Send(new RemoveBusinessLicenseByUserIdCommand(businessUserId, businessLicenseId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message);
            return TypedResults.InternalServerError();
        }
    }


    private static async Task<IResult> CreateStaffBusinessUserById(Guid businessUserId, [FromBody] StaffRequestDto request, [FromServices] AppService service, [FromServices] IValidator<StaffRequestDto> validator)
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
            service.Logger.LogError(ex.Message);
            return TypedResults.InternalServerError();
        }
    }

    private static async Task<IResult> RemoveStaffBusinessUserById(Guid businessUserId, Guid staffId, [FromServices] AppService service)
    {
        try
        { 
            var result = await service.Mediator.Send(new RemoveStaffByUserIdCommand(businessUserId, staffId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message);
            return TypedResults.InternalServerError();
        }
    }



    private static async Task<IResult> CreateBusinessUser([FromBody] BusinessUserRequestDto request, [FromServices] AppService service, [FromServices] IValidator<BusinessUserRequestDto> validator, [FromServices] LinkGenerator linkGenerator)
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
            service.Logger.LogError(ex.Message);
            return TypedResults.InternalServerError();
        }
    }
    
    private static async Task<IResult> UpdateBusinessUser(Guid businessUserId, [FromBody] UpdateBusinessUserByUserIdCommand request, [FromServices] AppService service, [FromServices] IValidator<UpdateBusinessUserByUserIdCommand> validator)
    {
        try
        {
            if (businessUserId != request.Id)
            {
                TypedResults.BadRequest("Business user id does not match");
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
            service.Logger.LogError(ex.Message);
            return TypedResults.InternalServerError();
        } 
    }
    
    private static async Task<IResult> GetBusinessUserById(Guid businessUserId, [FromServices] AppService service)
    {
        try
        {
            var query = new GetBusinessUserByUserIdQuery(businessUserId);
            var result = await service.Mediator.Send(query);

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message);
            return TypedResults.InternalServerError();
        } 
    }

    private static async Task<IResult> DeleteBusinessUserById(Guid businessUserId, [FromServices] AppService service)
    {
        try
        {
            var command = new DeleteBusinessUserByUserIdCommand(businessUserId);
            var result = await service.Mediator.Send(command);

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message);
            return TypedResults.InternalServerError();
        }
    }
}
