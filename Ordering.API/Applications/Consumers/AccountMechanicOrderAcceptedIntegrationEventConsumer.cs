using Core.Events.Account;
using Core.Exceptions;
using Core.Utilities;
using MassTransit;
using Ordering.Domain.Aggregates.OrderAggregate;
using Ordering.Domain.SeedWork;
using System.Data;

namespace Ordering.API.Applications.Consumers;

public class AccountMechanicOrderAcceptedIntegrationEventConsumer(
    IUnitOfWork unitOfWork,
    ILogger<AccountMechanicOrderAcceptedIntegrationEventConsumer> logger) : IConsumer<AccountMechanicOrderAcceptedIntegrationEvent>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<AccountMechanicOrderAcceptedIntegrationEventConsumer> _logger = logger;
    public async Task Consume(ConsumeContext<AccountMechanicOrderAcceptedIntegrationEvent> context)
    {
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted); 
        try
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(context.Message.OrderId);

            if (order is null)
            {
                return;
            }

            order.AssignMechanic(new Mechanic(
                context.Message.OrderId,
                context.Message.MechanicId,
                context.Message.MechanicName,
                context.Message.MechanicImageUrl,
                context.Message.Performance));

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (DomainException ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogInformation(ex.Message);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            LoggerHelper.LogError(_logger, ex);
            throw;
        }
    }
}
