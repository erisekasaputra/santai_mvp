
using Core.CustomMessages;
using Core.Results;
using Core.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;


namespace Core.Middlewares;

public class GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger) : IMiddleware
{ 
    private readonly ILogger<GlobalExceptionMiddleware> _logger = logger; 
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
            return;
        }
        catch (Exception ex)
        { 
            LoggerHelper.LogError(_logger, ex);
            if (!context.Response.HasStarted)
            {
                context.Response.Clear();
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new
                {
                    Message = Messages.InternalServerError,
                    Code = ResponseStatus.InternalServerError
                });
            }
        }
    }
}
