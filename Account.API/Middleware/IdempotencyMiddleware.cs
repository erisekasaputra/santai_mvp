using Account.API.Extensions;
using Account.API.Infrastructures;
using Account.API.SeedWork;

namespace Account.API.Middleware;

public class IdempotencyMiddleware
{
    private readonly RequestDelegate _next;  
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;

    public IdempotencyMiddleware(RequestDelegate next, IServiceProvider serviceProvider, ILogger<IdempotencyMiddleware> logger)
    {
        _next = next;  
        _serviceProvider = serviceProvider;
        _logger = logger; 
    }

    public async Task InvokeAsync(HttpContext context)
    { 
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var idempotencyService = scope.ServiceProvider.GetRequiredService<IIdempotencyService>();

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
                var errorResponse = new { ErrorMessage = "X-Idempotency-Key Header is required" };
                await context.Response.WriteAsJsonAsync(errorResponse);
                return;
            }

            if (await idempotencyService.CheckIdempotencyKeyAsync(idempotencyKey.ToString()))
            {
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                context.Response.ContentType = "application/json";
                var errorResponse = new { ErrorMessage = "Can not set the idempotency key" };
                await context.Response.WriteAsJsonAsync(errorResponse);
                return;
            }

            await _next(context);

            if (context.Response.StatusCode is >= 200 and <= 299)
            {
                await idempotencyService.SetIdempotencyKeyAsync(idempotencyKey.ToString(), TimeSpan.FromDays(1));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex.InnerException?.Message);

            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest; 
            }
        } 
    }
}
