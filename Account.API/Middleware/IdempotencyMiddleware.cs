using Account.API.Applications.Services;
using Account.API.SeedWork; 

namespace Account.API.Middleware;

public class IdempotencyMiddleware
{
    private readonly RequestDelegate _next;  

    public IdempotencyMiddleware(RequestDelegate next)
    {
        _next = next;  
    }

    public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<IIdempotencyService>();

        var endPoint = context.GetEndpoint();

        if (endPoint == null)
        {
            await _next(context);
            return;
        }

        var hasIdempotencyAttribute = endPoint?.Metadata.GetMetadata<IdempotencyAttribute>() != null;

        if (!hasIdempotencyAttribute) 
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("X-Idempotency-Key", out var key) || !Guid.TryParse(key, out var idempotencyKey))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            var errorResponse = new { ErrorMessage = "X-Idempotency-Key Header is required"};
            await context.Response.WriteAsJsonAsync(errorResponse);
            return;
        }

        if (await service.CheckIdempotencyKeyAsync(idempotencyKey.ToString()))
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            context.Response.ContentType = "application/json";
            var errorResponse = new { ErrorMessage = "Can not set the idempotency key" };
            await context.Response.WriteAsJsonAsync(errorResponse);
            return;
        } 

        try
        {
            await _next(context);

            if (context.Response.StatusCode is >= 200 and <= 299)
            { 
                await service.SetIdempotencyKeyAsync(idempotencyKey.ToString(), TimeSpan.FromDays(1)); 
            }
        }
        catch(Exception)
        {
            throw;
        }
    }
}
