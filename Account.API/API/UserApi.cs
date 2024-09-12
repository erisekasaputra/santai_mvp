using Account.API.Applications.Commands.FleetCommand.AssignFleetsToStaff;
using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Applications.Queries.GetBusinessUserByUserId;
using Account.API.Applications.Queries.GetEmailByUserId;
using Account.API.Applications.Queries.GetMechanicUserById;
using Account.API.Applications.Queries.GetPhoneNumberByUserId;
using Account.API.Applications.Queries.GetRegularUserByUserId;
using Account.API.Applications.Queries.GetStaffById;
using Account.API.Applications.Queries.GetStaffByUserIdAndStaffId;
using Account.API.Applications.Queries.GetTimeZoneByUserId;
using Account.API.Applications.Services; 
using Account.API.Extensions; 
using Core.Enumerations;
using Core.Messages;
using Core.Results;
using Core.SeedWorks;
using Core.Services.Interfaces; 
using Microsoft.AspNetCore.Mvc;

namespace Account.API.API;

public static class UserApi
{
    private const int _cacheExpiry = 10;
    public static IEndpointRouteBuilder MapUserApi(this IEndpointRouteBuilder route)
    {
        var app = route.MapGroup("api/v1/users");   
        
        app.MapPatch("/{userId}/staff/{staffId}/fleets", AssignFleetsToStaff)
            .RequireAuthorization(
                PolicyName.BusinessUserAndAdministratorUserPolicy.ToString());

        app.MapGet("/email", GetEmailByUserId) 
            .RequireAuthorization()
            .CacheOutput();
        
        app.MapGet("/phone-number", GetPhoneNumberByUserId)
            .RequireAuthorization()
            .CacheOutput();
        
        app.MapGet("/time-zone", GetTimeZoneByUserId)
            .RequireAuthorization()
            .CacheOutput();

        app.MapGet("{userId}", GetByUserId)
            .RequireAuthorization(PolicyName.ServiceToServiceOnlyPolicy.ToString())
            .CacheOutput(config =>
            {
                config.Expire(TimeSpan.FromSeconds(_cacheExpiry));
                config.SetVaryByQuery("userId");
            });

        return app;
    }

    private static async Task<IResult> GetByUserId(
        Guid userId,
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

            Result result; 
            if (userClaim.CurrentUserType == UserType.StaffUser)
            {
                result = await service.Mediator.Send(
                    new GetStaffByIdQuery(userId));

                return result.ToIResult();
            }
            else if (userClaim.CurrentUserType == UserType.BusinessUser)
            {
                result = await service.Mediator.Send(
                   new GetBusinessUserByUserIdQuery(userId));

                return result.ToIResult();
            }
            else if (userClaim.CurrentUserType == UserType.RegularUser)
            {
                result = await service.Mediator.Send(
                   new GetRegularUserByUserIdQuery(userId));

                return result.ToIResult();
            }

            else if (userClaim.CurrentUserType == UserType.MechanicUser)
            {
                result = await service.Mediator.Send(
                   new GetMechanicUserByIdQuery(userId));

                return result.ToIResult();
            }

            return TypedResults.BadRequest();
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
