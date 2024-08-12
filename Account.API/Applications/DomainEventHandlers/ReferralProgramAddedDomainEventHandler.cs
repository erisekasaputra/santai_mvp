using Account.API.Services;
using Account.Domain.Aggregates.ReferredAggregate;
using Account.Domain.Events;
using Account.Domain.SeedWork;
using MediatR;
namespace Account.API.Applications.DomainEventHandlers;

public class ReferralProgramAddedDomainEventHandler(IUnitOfWork unitOfWork, ApplicationService service) : INotificationHandler<ReferralProgramAddedDomainEvent>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service; 

    public Task Handle(ReferralProgramAddedDomainEvent notification, CancellationToken cancellationToken)
    { 
        return Task.CompletedTask;
    }
}
