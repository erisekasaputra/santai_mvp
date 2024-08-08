using Account.API.Applications.Commands.ConfirmUserEmailByUserId;
using Account.API.Applications.Commands.ConfirmUserPhoneNumberByUserId;
using Account.API.Applications.Commands.UpdateUserEmailByUserId;
using Account.API.Applications.Commands.UpdateUserPhoneNumberByUserId;
using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Applications.Services;
using Account.API.Extensions; 
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Account.API.API;

public static class UserApi
{
    public static IEndpointRouteBuilder MapUserApi(this IEndpointRouteBuilder route)
    {
        var app = route.MapGroup("api/v1/user");

        app.MapPatch("/{userId}/email", UpdateUserEmailByUserId);
        app.MapPatch("/{userId}/email/confirm", ConfirmUserEmailByUserId);
        app.MapPatch("/{userId}/phone-number", UpdateUserPhoneNumberByUserId);
        app.MapPatch("/{userId}/phone-number/confirm", ConfirmUserPhoneNumberByUserId);

        return app;
    } 

    private static async Task<IResult> UpdateUserEmailByUserId(Guid userId, [FromBody] EmailRequestDto request, AppService service, IValidator<EmailRequestDto> validator)
    {
        // later on , user id is from user claims at authentication level  
        try
        {
            var validate = await validator.ValidateAsync(request);

            if (!validate.IsValid)
            {
                var error = validate.Errors;
                return TypedResults.BadRequest(error);
            }

            var result = await service.Mediator.Send(new UpdateUserEmailByUserIdCommand(userId, request));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message);
            return TypedResults.InternalServerError();
        }
    }

    private static async Task<IResult> ConfirmUserEmailByUserId(Guid userId, AppService service, IValidator<EmailRequestDto> validator)
    {
        // later on , user id is from user claims at authentication level  
        try
        { 
            var result = await service.Mediator.Send(new ConfirmUserEmailByUserIdCommand(userId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message);
            return TypedResults.InternalServerError();
        }
    }

    private static async Task<IResult> UpdateUserPhoneNumberByUserId(Guid userId, [FromBody] PhoneNumberRequestDto request, AppService service, IValidator<PhoneNumberRequestDto> validator)
    {
        // later on , user id is from user claims at authentication level  
        try
        {
            var validate = await validator.ValidateAsync(request);

            if (!validate.IsValid)
            {
                var error = validate.Errors;
                return TypedResults.BadRequest(error);
            }

            var result = await service.Mediator.Send(new UpdateUserPhoneNumberByUserIdCommand(userId, request));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message);
            return TypedResults.InternalServerError();
        }
    }

    private static async Task<IResult> ConfirmUserPhoneNumberByUserId(Guid userId, AppService service)
    {
        // later on , user id is from user claims at authentication level  
        try
        { 
            var result = await service.Mediator.Send(new ConfirmUserPhoneNumberByUserIdCommand(userId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message);
            return TypedResults.InternalServerError();
        }
    }
}
