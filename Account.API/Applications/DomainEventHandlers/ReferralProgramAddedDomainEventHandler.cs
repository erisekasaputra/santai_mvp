using Account.API.Applications.Services;
using Account.Domain.Aggregates.ReferredAggregate;
using Account.Domain.Events;
using Account.Domain.SeedWork;
using MediatR;
namespace Account.API.Applications.DomainEventHandlers;

public class ReferralProgramAddedDomainEventHandler(IUnitOfWork unitOfWork, AppService service) : INotificationHandler<ReferralProgramAddedDomainEvent>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly AppService _service = service; 

    public Task Handle(ReferralProgramAddedDomainEvent notification, CancellationToken cancellationToken)
    { 
        return Task.CompletedTask;
    }
}
