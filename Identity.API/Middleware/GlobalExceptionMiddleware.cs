
using Core.Messages; 

namespace Identity.API.Middleware;

public class GlobalExceptionMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    public GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex.InnerException?.Message);

            if (!context.Response.HasStarted)
            {
                context.Response.Clear();
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new 
                {
                    Message = Messages.InternalServerError,
                    Detail = "Internal Server Error",
                    Code = 500
                });
            }
        }
    }
}
