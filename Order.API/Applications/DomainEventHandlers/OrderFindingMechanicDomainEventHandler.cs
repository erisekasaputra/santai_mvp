using Core.Events;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Domain.Events;
using Order.Domain.SeedWork;
using Polly;
using Polly.Retry;
using System.Data;

namespace Order.API.Applications.DomainEventHandlers;

public class OrderFindingMechanicDomainEventHandler : INotificationHandler<OrderFindingMechanicDomainEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly ILogger<OrderPaymentPaidDomainEventHandler> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderFindingMechanicDomainEventHandler(
        IUnitOfWork unitOfWork,
        IMediator mediator,
        ILogger<OrderPaymentPaidDomainEventHandler> logger,
        IPublishEndpoint publishEndpoint)
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
        _publishEndpoint = publishEndpoint;
    }
    public async Task Handle(OrderFindingMechanicDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {
             
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "An error has occured \r\nError: {error} \r\nDetail: {detail}", ex.Message, ex.InnerException?.Message);
        }
    }
}
