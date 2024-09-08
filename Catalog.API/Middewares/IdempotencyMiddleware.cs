using Catalog.API.Configurations;
using Catalog.API.CustomAttributes;
using Catalog.API.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Catalog.API.Middewares;

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
            var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
            var idempotencyOptions = scope.ServiceProvider.GetRequiredService<IOptionsMonitor<IdempotencyOptions>>();

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
                var errorResponse = new { ErrorMessage = "Idempotency key is required" };
                await context.Response.WriteAsJsonAsync(errorResponse);
                return;
            }

            var idempotencyKey = $"{resourceName}#{idempotencyKeyHeader}";

            if (await cacheService.CheckIdempotencyKeyAsync(idempotencyKey))
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
                await cacheService.SetIdempotencyKeyAsync(
                    idempotencyKey, TimeSpan.FromSeconds(idempotencyOptions.CurrentValue.TTL));
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
