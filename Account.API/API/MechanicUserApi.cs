using Account.API.Applications.Commands.MechanicUserCommand.CreateMechanicUser;
using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Extensions;
using Account.API.Options;
using Account.API.Services;
using FluentValidation;
using MassTransit.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Account.API.API;

public static class MechanicUserApi
{
    public static IEndpointRouteBuilder MapMechanicUserApi(this IEndpointRouteBuilder builder)
    {
        var app = builder.MapGroup("api/v1/users/mechanic");

        app.MapPost("/", CreateMechanicUser);
        app.MapGet("/{mechanicUserId}", GetMechanicUserById);

        return app;
    }

    private static async Task<IResult> GetMechanicUserById(string mechanicUserId, [FromServices] IKeyManagementService _kms, [FromServices] IOptionsMonitor<KeyManagementServiceOption> options)
    {
        object newResult = new {
            Identity = options.CurrentValue.Id,
            Secret = options.CurrentValue.SecretKey
        };

        var result = await _kms.EncryptAsync("Eris Eka Saputra");

        return TypedResults.Ok(result);
    }

    private static async Task<IResult> CreateMechanicUser([FromBody] MechanicUserRequestDto request, ApplicationService service, IValidator<MechanicUserRequestDto> validator)
    {
        try
        { 
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                return TypedResults.BadRequest(validation.Errors);
            }

            var result = await service.Mediator.Send(new CreateMechanicUserCommand(
                request.IdentityId,
                request.Username,
                request.Email,
                request.PhoneNumber,
                request.TimeZoneId,
                request.Address,
                request.Certifications,
                request.DrivingLicenseRequestDto,
                request.NationalIdentityRequestDto,
                request.DeviceId));   
            return result.ToIResult();
        }
        catch (Exception ex) 
        {
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }
}
