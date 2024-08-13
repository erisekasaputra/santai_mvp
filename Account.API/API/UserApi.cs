using Account.API.Applications.Commands.UserCommand.ConfirmUserEmailByUserId;
using Account.API.Applications.Commands.UserCommand.ConfirmUserPhoneNumberByUserId;
using Account.API.Applications.Commands.UserCommand.UpdateUserEmailByUserId;
using Account.API.Applications.Commands.UserCommand.UpdateUserPhoneNumberByUserId;
using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Extensions;
using Account.API.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Account.API.API;

public static class UserApi
{
    public static IEndpointRouteBuilder MapUserApi(this IEndpointRouteBuilder route)
    {
        var app = route.MapGroup("api/v1/users"); 

        app.MapPatch("/{userId}/email", UpdateUserEmailByUserId);
        app.MapPatch("/{userId}/email/confirm", ConfirmUserEmailByUserId);
        app.MapPatch("/{userId}/phone-number", UpdateUserPhoneNumberByUserId);
        app.MapPatch("/{userId}/phone-number/confirm", ConfirmUserPhoneNumberByUserId);

        return app;
    } 

    private static async Task<IResult> UpdateUserEmailByUserId(Guid userId, [FromBody] EmailRequestDto request, ApplicationService service, IValidator<EmailRequestDto> validator)
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

            var result = await service.Mediator.Send(new UpdateUserEmailByUserIdCommand(userId, request.Email)); 
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> ConfirmUserEmailByUserId(Guid userId, ApplicationService service, IValidator<EmailRequestDto> validator)
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
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> UpdateUserPhoneNumberByUserId(Guid userId, [FromBody] PhoneNumberRequestDto request, ApplicationService service, IValidator<PhoneNumberRequestDto> validator)
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

            var result = await service.Mediator.Send(new UpdateUserPhoneNumberByUserIdCommand(userId, request.PhoneNumber)); 
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> ConfirmUserPhoneNumberByUserId(Guid userId, ApplicationService service)
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
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }
}
