
using Account.API.Applications.Commands.CreateRegularUser;
using Account.API.Applications.Commands.DeleteRegularUserByUserId;
using Account.API.Applications.Commands.UpdateRegularUserByUserId;
using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Applications.Queries.GetRegularUserByUserId;
using Account.API.Applications.Services;
using Account.API.Extensions;
using Account.API.SeedWork; 
using FluentValidation;
using Microsoft.AspNetCore.Mvc; 

namespace Account.API.API;

public static class RegularUserApi
{
    public static IEndpointRouteBuilder MapRegularUserApi(this IEndpointRouteBuilder builder)
    {
        var app = builder.MapGroup("api/v1/users/regular");

        app.MapPost("/", CreateRegularUser).WithMetadata(new IdempotencyAttribute());
        app.MapGet("/{userId}", GetRegularUserByUserId);
        app.MapPut("/{userId}", UpdateRegularUserByUserId);
        app.MapDelete("/{userId}", DeleteRegularUserByUserId);

        return app;
    }

    private static async Task<IResult> DeleteRegularUserByUserId(Guid userId, [FromServices] AppService service)
    {
        try
        {
            var result = await service.Mediator.Send(new DeleteRegularUserByUserIdCommand(userId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        } 
    }

    private static async Task<IResult> UpdateRegularUserByUserId(Guid userId, [FromBody] UpdateRegularUserByUserIdCommand command, [FromServices] AppService service, IValidator<UpdateRegularUserByUserIdCommand> validator)
    {
        try
        {
            if (userId != command.UserId)
            {
                return TypedResults.BadRequest("User id does not match");
            }

            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return TypedResults.BadRequest(validationResult.Errors);
            }

            var result = await service.Mediator.Send(command);
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> GetRegularUserByUserId(Guid userId, [FromServices] AppService service)
    {
        try
        {
            var result = await service.Mediator.Send(new GetRegularUserByUserIdQuery(userId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    } 

    private static async Task<IResult> CreateRegularUser([FromBody] RegularUserRequestDto request, [FromServices] AppService service, IValidator<RegularUserRequestDto> validator)
    {
        try
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return TypedResults.BadRequest(validationResult.Errors);
            }

            var result = await service.Mediator.Send(new CreateRegularUserCommand(request)); 
            return result.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogError(ex.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }
}
