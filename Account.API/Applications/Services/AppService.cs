using MediatR;

namespace Account.API.Applications.Services;

public class AppService
{
    public readonly IMediator Mediator;
     
    public readonly HttpContext HttpContext;

    public readonly ILogger<AppService> Logger;
    public AppService(IMediator mediator, IHttpContextAccessor context, ILogger<AppService> logger)
    {
        Mediator = mediator; 
        HttpContext = context.HttpContext!;
        Logger = logger;
    }   
}
