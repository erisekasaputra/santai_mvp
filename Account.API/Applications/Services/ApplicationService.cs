using MediatR;

namespace Account.API.Applications.Services;

public class ApplicationService
{
    public readonly IMediator Mediator;

    public readonly ILogger<ApplicationService> Logger;
    public ApplicationService(IMediator mediator, ILogger<ApplicationService> logger)
    {
        Mediator = mediator;
        Logger = logger;
    }
}
