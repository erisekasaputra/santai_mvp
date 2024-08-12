using MediatR;

namespace Account.API.Services;

public class ApplicationService
{
    public readonly IMediator Mediator;

    public readonly HttpContext HttpContext;

    public readonly ILogger<ApplicationService> Logger;
    public ApplicationService(IMediator mediator, IHttpContextAccessor context, ILogger<ApplicationService> logger)
    {
        Mediator = mediator;
        HttpContext = context.HttpContext!;
        Logger = logger;
    }
}
