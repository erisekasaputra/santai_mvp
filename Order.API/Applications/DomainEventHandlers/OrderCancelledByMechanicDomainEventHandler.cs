using Core.Events; 
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Domain.Events;
using Order.Domain.SeedWork;
using Polly;
using Polly.Retry;
using System.Data;

namespace Order.API.Applications.DomainEventHandlers;

public class OrderCancelledByMechanicDomainEventHandler : INotificationHandler<OrderCancelledByMechanicDomainEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly ILogger<OrderPaymentPaidDomainEventHandler> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    public OrderCancelledByMechanicDomainEventHandler(
        IUnitOfWork unitOfWork,
        IMediator mediator,
        ILogger<OrderPaymentPaidDomainEventHandler> logger)
    { 
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _logger = logger;

        _retryPolicy = Policy
                      .Handle<DBConcurrencyException>()
                      .Or<DbUpdateConcurrencyException>()
                      .Or<DbUpdateException>() 
                      .WaitAndRetryAsync(
                          retryCount: 3,
                          sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                          onRetry: (exception, timeSpan, retryCount, context) =>
                              logger.LogInformation("Retry {retryCount} encountered an exception: {Message}. Waiting {timeSpan} before next retry.", retryCount, exception.Message, timeSpan));
    }

    public async Task Handle(OrderCancelledByMechanicDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

                var orderAggregate = await _unitOfWork.Orders.GetByIdAsync(notification.Order.Id, cancellationToken);

                if (orderAggregate is null)
                {
                    return;
                }

                orderAggregate.SetFindingMechanic();

                _unitOfWork.Orders.Update(orderAggregate);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                await _mediator.Publish(new OrderCancelledByMechanicIntegrationEvent(), cancellationToken);
            }); 
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "An error has occured \r\nError: {error} \r\nDetail: {detail}", ex.Message, ex.InnerException?.Message);
        }
    }
}
