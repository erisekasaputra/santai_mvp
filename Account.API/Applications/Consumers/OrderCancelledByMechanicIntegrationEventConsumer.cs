
using Account.API.Applications.Services.Interfaces;
using Account.Domain.SeedWork;
using Core.Events.Ordering;
using MassTransit;
using System.Data;

namespace Account.API.Applications.Consumers;

public class OrderCancelledByMechanicIntegrationEventConsumer(
    IMechanicCache mechanicCache,
    IUnitOfWork unitOfWork) : IConsumer<OrderCancelledByMechanicIntegrationEvent>
{
    private readonly IMechanicCache _mechanicCache = mechanicCache;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task Consume(ConsumeContext<OrderCancelledByMechanicIntegrationEvent> context)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadUncommitted);

            (var isSuccess, var buyerId) = await _mechanicCache.CancelOrderByMechanic(
                context.Message.OrderId.ToString(),
                context.Message.MechanicId.ToString());

            if (!isSuccess)
            {
                throw new Exception($"Failed to cancel the order by mechanic for order id {context.Message.OrderId}");
            }

            var mechanic = await _unitOfWork.BaseUsers.GetMechanicUserByIdAsync(context.Message.MechanicId);
            if (mechanic is not null)
            {
                mechanic.CancelByMechanic();
                _unitOfWork.BaseUsers.Update(mechanic);
            }

            await _unitOfWork.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;  
        }
    }
}
