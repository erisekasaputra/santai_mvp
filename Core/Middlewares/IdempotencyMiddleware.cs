using Core.Configurations;
using Core.CustomAttributes;
using Core.Results;
using Core.Services.Interfaces; 
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection; 
using Microsoft.Extensions.Options;

namespace Core.Middlewares;

public class IdempotencyMiddleware(
    RequestDelegate next, 
    IServiceProvider serviceProvider)
{
    private readonly RequestDelegate _next = next;
    private readonly IServiceProvider _serviceProvider = serviceProvider; 

    public async Task InvokeAsync(HttpContext context)
    {
        using var scope = _serviceProvider.CreateScope();
        var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
        var idempotencyOptions = scope.ServiceProvider.GetRequiredService<IOptionsMonitor<IdempotencyConfiguration>>();

        var endPoint = context.GetEndpoint();

        if (endPoint is null)
        {
            await _next(context);
            return;
        }

        if (!idempotencyOptions.CurrentValue.IsActive)
        {
            await _next(context);
            return;
        }

        var attribute = endPoint?.Metadata.GetMetadata<IdempotencyAttribute>();
        var hasIdempotencyAttribute = attribute is not null;
        var resourceName = attribute?.Name;
        if (!hasIdempotencyAttribute)
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("X-Idempotency-Key", out var key) || !Guid.TryParse(key, out var idempotencyKeyHeader))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var errorResponse = Result.Failure("Idempotency key is required", ResponseStatus.BadRequest);
            await context.Response.WriteAsJsonAsync(errorResponse);
            return;
        }

        var idempotencyKey = $"{resourceName}#{idempotencyKeyHeader}";

        if (await cacheService.CheckIdempotencyKeyAsync(idempotencyKey))
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            context.Response.ContentType = "application/json";
            var errorResponse = Result.Failure("Idempotency key is invalid", ResponseStatus.BadRequest);
            await context.Response.WriteAsJsonAsync(errorResponse);
            return;
        }

        await _next(context);

        if (context.Response.StatusCode is >= 200 and <= 299)
        {
            await cacheService.SetIdempotencyKeyAsync(
                idempotencyKey, TimeSpan.FromSeconds(idempotencyOptions.CurrentValue.TTL));
        }
    }
}
