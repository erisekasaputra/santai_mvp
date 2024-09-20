using Account.API.Applications.Commands.FleetCommand.AssignFleetsToStaff;
using Account.API.Applications.Commands.UserCommand.ResetDeviceIdByUserId;
using Account.API.Applications.Commands.UserCommand.SetDeviceIdByUserId;
using Account.API.Applications.Dtos.RequestDtos; 
using Account.API.Applications.Queries.GetEmailByUserId; 
using Account.API.Applications.Queries.GetPhoneNumberByUserId; 
using Account.API.Applications.Queries.GetTimeZoneByUserId;
using Account.API.Applications.Queries.GetUserByUserTypeAndUserId;
using Account.API.Applications.Services; 
using Account.API.Extensions; 
using Core.Enumerations;
using Core.Messages; 
using Core.SeedWorks;
using Core.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Account.API.API;

public static class UserApi
{ 
    public static IEndpointRouteBuilder MapUserApi(this IEndpointRouteBuilder route)
    {
        var app = route.MapGroup("api/v1/users");   
        
        app.MapPatch("/{userId}/staff/{staffId}/fleets", AssignFleetsToStaff)
            .RequireAuthorization(
                PolicyName.BusinessUserAndAdministratorUserPolicy.ToString());

        app.MapGet("/email", GetEmailByUserId)
            .RequireAuthorization();

        app.MapGet("/phone-number", GetPhoneNumberByUserId)
            .RequireAuthorization();

        app.MapGet("/time-zone", GetTimeZoneByUserId)
            .RequireAuthorization();

        app.MapPatch("/{userId}/device-id/set", SetDeviceIdByUserId)
           .RequireAuthorization();

        app.MapPatch("/{userId}/device-id/reset", ResetDeviceIdByUserId)
           .RequireAuthorization();

        app.MapPost("{userId}/info", GetByUserId)
            .RequireAuthorization(PolicyName.ServiceToServiceOnlyPolicy.ToString());

        return app;
    } 

    private static async Task<IResult> GetByUserId(
        Guid userId,
        [FromBody] FleetsRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IUserInfoService userInfoService)
    {
        try
        {
            var userClaim = userInfoService.GetServiceInfo();
            if (userClaim is null)
            {
                return TypedResults.Unauthorized();
            }

            var result = await service.Mediator.Send(
                new GetUserByUserTypeAndUserIdQuery(userId, request.Fleets));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }


    private static async Task<IResult> GetTimeZoneByUserId( 
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

            var result = await service.Mediator.Send(new GetTimeZoneByUserIdQuery(userClaim.Sub));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> GetPhoneNumberByUserId( 
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

            var result = await service.Mediator.Send(new GetPhoneNumberByUserIdQuery(userClaim.Sub));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> GetEmailByUserId( 
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

            var result = await service.Mediator.Send(new GetEmailByUserIdQuery(userClaim.Sub));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> AssignFleetsToStaff(
        Guid userId, 
        Guid staffId,
        [FromBody] AssignFleetsToStaffRequestDto request,
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

            if (userClaim.CurrentUserType == UserType.BusinessUser && userId != userClaim.Sub) 
            {
                return TypedResults.Forbid();
            }

            var result = await service.Mediator.Send(
                new AssignFleetsToStaffCommand(userId, staffId, request.FleetIds));
            
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }




    private static async Task<IResult> ResetDeviceIdByUserId(
        Guid userId,
        [FromBody] DeviceIdRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<DeviceIdRequestDto> validator,
        [FromServices] IUserInfoService userInfoService)
    {
        try
        { 
            var validation = await validator.ValidateAsync(request); 
            if (!validation.IsValid)
            {
                return TypedResults.BadRequest(validation.Errors);
            }

            var result = await service.Mediator.Send(new ResetDeviceIdByUserIdCommand(userId, request.DeviceId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> SetDeviceIdByUserId(
        Guid userId,
        [FromBody] DeviceIdRequestDto request,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<DeviceIdRequestDto> validator,
        [FromServices] IUserInfoService userInfoService)
    {
        try
        { 
            var validation = await validator.ValidateAsync(request); 
            if (!validation.IsValid)
            {
                return TypedResults.BadRequest(validation.Errors);
            }

            var result = await service.Mediator.Send(new SetDeviceIdByUserIdCommand(
                userId,
                request.DeviceId));

            return result.ToIResult();
        }
        catch (Exception ex)
        { 
            service.Logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    //private static async Task<IResult> UpdateUserEmailByUserId(
    //    Guid userId, 
    //    [FromBody] EmailRequestDto request, 
    //    [FromServices] ApplicationService service, 
    //    [FromServices] IValidator<EmailRequestDto> validator,
    //    [FromServices] IUserInfoService userInfoService)
    //{
    //    // later on , user id is from user claims at authentication level  
    //    try
    //    {
    //        var userClaim = userInfoService.GetUserInfo();
    //        if (userClaim is null)
    //        {
    //            return TypedResults.Unauthorized();
    //        }

    //        var validate = await validator.ValidateAsync(request); 
    //        if (!validate.IsValid)
    //        {
    //            var error = validate.Errors;
    //            return TypedResults.BadRequest(error);
    //        }

    //        var result = await service.Mediator.Send(new UpdateUserEmailByUserIdCommand(userId, request.Email)); 
    //        return result.ToIResult();
    //    }
    //    catch (Exception ex)
    //    {
    //        service.Logger.LogError(ex, ex.InnerException?.Message);
    //        return TypedResults.InternalServerError(Messages.InternalServerError);
    //    }
    //}

    //private static async Task<IResult> ConfirmUserEmailByUserId(
    //    Guid userId,
    //    [FromServices] ApplicationService service,
    //    [FromServices] IValidator<EmailRequestDto> validator,
    //    [FromServices] IUserInfoService userInfoService)
    //{
    //    // later on , user id is from user claims at authentication level  
    //    try
    //    {
    //        var userClaim = userInfoService.GetUserInfo();
    //        if (userClaim is null)
    //        {
    //            return TypedResults.Unauthorized();
    //        }

    //        var result = await service.Mediator.Send(new ConfirmUserEmailByUserIdCommand(userId)); 
    //        return result.ToIResult();
    //    }
    //    catch (Exception ex)
    //    {
    //        service.Logger.LogError(ex, ex.InnerException?.Message);
    //        return TypedResults.InternalServerError(Messages.InternalServerError);
    //    }
    //}

    //private static async Task<IResult> UpdateUserPhoneNumberByUserId(
    //    Guid userId,
    //    [FromBody] PhoneNumberRequestDto request,
    //    [FromServices] ApplicationService service,
    //    [FromServices] IValidator<PhoneNumberRequestDto> validator,
    //    [FromServices] IUserInfoService userInfoService)
    //{
    //    // later on , user id is from user claims at authentication level  
    //    try
    //    {
    //        var userClaim = userInfoService.GetUserInfo();
    //        if (userClaim is null)
    //        {
    //            return TypedResults.Unauthorized();
    //        }

    //        var validate = await validator.ValidateAsync(request);  
    //        if (!validate.IsValid)
    //        {
    //            var error = validate.Errors;
    //            return TypedResults.BadRequest(error);
    //        }

    //        var result = await service.Mediator.Send(new UpdateUserPhoneNumberByUserIdCommand(userId, request.PhoneNumber)); 
    //        return result.ToIResult();
    //    }
    //    catch (Exception ex)
    //    {
    //        service.Logger.LogError(ex, ex.InnerException?.Message);
    //        return TypedResults.InternalServerError(Messages.InternalServerError);
    //    }
    //}

    //private static async Task<IResult> ConfirmUserPhoneNumberByUserId(
    //    Guid userId, 
    //    ApplicationService service,
    //    [FromServices] IUserInfoService userInfoService)
    //{ 
    //    try
    //    {
    //        var userClaim = userInfoService.GetUserInfo();
    //        if (userClaim is null)
    //        {
    //            return TypedResults.Unauthorized();
    //        }

    //        var result = await service.Mediator.Send(new ConfirmUserPhoneNumberByUserIdCommand(userId)); 
    //        return result.ToIResult();
    //    }
    //    catch (Exception ex)
    //    {
    //        service.Logger.LogError(ex, ex.InnerException?.Message);
    //        return TypedResults.InternalServerError(Messages.InternalServerError);
    //    }
    //}
}
