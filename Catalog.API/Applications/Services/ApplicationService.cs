using MediatR;

namespace Catalog.API.Applications.Services;

public class ApplicationService(IMediator mediator, ILogger<ApplicationService> logger, LinkGenerator linkGenerator)
{
    public IMediator Mediator { get; set; } = mediator;
    public ILogger<ApplicationService> Logger { get; set; } = logger;
    public LinkGenerator LinkGenerator { get; set; } = linkGenerator;
}
