using Account.API.Applications.Services.Interfaces;
using Account.Domain.SeedWork;
using Core.Events.Ordering;
using MassTransit;

namespace Account.API.Applications.Consumers;

public class ServiceIncompletedIntegrationEventConsumer(
    IMechanicCache mechanicCache,
    IUnitOfWork unitOfWork) : IConsumer<ServiceIncompletedIntegrationEvent>
{
    private readonly IMechanicCache _mechanicCache = mechanicCache;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task Consume(ConsumeContext<ServiceIncompletedIntegrationEvent> context)
    {
        await _unitOfWork.BeginTransactionAsync();

        (var isSuccess, _) = await _mechanicCache.CompleteOrder(
         context.Message.OrderId.ToString(),
         context.Message.MechanicId.ToString());

        if (!isSuccess)
        {
            throw new Exception($"Failed when completing the order {context.Message.OrderId}");
        }

        var buyer = await _unitOfWork.BaseUsers.GetByIdAsync(context.Message.BuyerId);
        if (buyer is not null)
        {
            buyer.AddLoyaltyPoint(1000);
            _unitOfWork.BaseUsers.Update(buyer);
        }

        var mechanic = await _unitOfWork.BaseUsers.GetMechanicUserByIdAsync(context.Message.MechanicId);
        if (mechanic is not null)
        {
            mechanic.SetIncompleteJob();
            _unitOfWork.BaseUsers.Update(mechanic);
        }

        await _unitOfWork.CommitTransactionAsync();
    }
}
