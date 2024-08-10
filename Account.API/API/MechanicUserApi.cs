using Account.API.Applications.Services;
using Account.API.Extensions;

namespace Account.API.API;

public static class MechanicUserApi
{
    public static IEndpointRouteBuilder MapMechanicUserApi(this IEndpointRouteBuilder builder)
    {
        var app = builder.MapGroup("api/v1/users/mechanic");

        app.MapPost("/", CreateMechanicUser);

        return app;
    }

    private static async Task<IResult> CreateMechanicUser(AppService service)
    {
        try
        {
            await Task.Delay(10);
            return TypedResults.Ok();
        }
        catch (Exception ex) 
        {
            service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }
}
