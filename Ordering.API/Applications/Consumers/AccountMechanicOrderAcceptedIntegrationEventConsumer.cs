using Core.Events;
using Core.Exceptions;
using Core.Utilities;
using MassTransit;
using Ordering.Domain.SeedWork;
using System.Data;
using Ordering.Domain.Aggregates.MechanicAggregate;

namespace Ordering.API.Applications.Consumers;

public class AccountMechanicOrderAcceptedIntegrationEventConsumer(
    IUnitOfWork unitOfWork,
    ILogger<AccountMechanicOrderAcceptedIntegrationEventConsumer> logger) : IConsumer<AccountMechanicOrderAcceptedIntegrationEvent>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<AccountMechanicOrderAcceptedIntegrationEventConsumer> _logger = logger;
    public async Task Consume(ConsumeContext<AccountMechanicOrderAcceptedIntegrationEvent> context)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted); 
            var order = await _unitOfWork.Orders.GetByIdAsync(context.Message.OrderId);

            if (order is null)
            {
                return;
            }

            order.AssignMechanic(new Mechanic(
                context.Message.OrderId,
                context.Message.MechanicId,
                context.Message.Name,
                context.Message.Performance));

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (DomainException ex)
        {
            _logger.LogInformation(ex.Message);
        }
        catch (Exception ex) 
        {
            LoggerHelper.LogError(_logger, ex);
            throw;
        }
    }
}
