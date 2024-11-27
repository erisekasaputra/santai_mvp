using Account.API.Applications.Services.Interfaces;
using Account.Domain.SeedWork;
using Core.Events.Ordering;
using MassTransit;
using System.Data;

namespace Account.API.Applications.Consumers;

public class ServiceCompletedIntegrationEventConsumer(
    IMechanicCache mechanicCache,
    IUnitOfWork unitOfWork) : IConsumer<ServiceCompletedIntegrationEvent>
{
    private readonly IMechanicCache _mechanicCache = mechanicCache;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task Consume(ConsumeContext<ServiceCompletedIntegrationEvent> context)
    { 
        try 
        {
            await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadUncommitted);

            (var isSuccess, _) = await _mechanicCache.CompleteOrder(
                context.Message.OrderId.ToString(), 
                context.Message.MechanicId.ToString());

            if (!isSuccess)
            {
                throw new Exception($"Failed when completing the order {context.Message.OrderId}");
            }
         
            var buyer = await _unitOfWork.BaseUsers.GetByIdAsync(context.Message.BuyerId);
            if(buyer is not null)
            {
                buyer.AddLoyaltyPoint(1000);
                _unitOfWork.BaseUsers.Update(buyer);
            }

            var mechanic = await _unitOfWork.BaseUsers.GetMechanicUserByIdAsync(context.Message.MechanicId);
            if (mechanic is not null)
            {
                mechanic.SetCompleteJob();
                _unitOfWork.BaseUsers.Update(mechanic);
            } 

            await _unitOfWork.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw; // Rethrow the exception to ensure it's logged or handled further.
        }
    }
}
