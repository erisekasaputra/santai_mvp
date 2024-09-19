using Core.Enumerations;
using Core.Exceptions;
using Core.Messages;
using Core.Results;
using MediatR;
using Ordering.API.Applications.Commands.Orders.CreateOrder;
using Ordering.API.Applications.Commands.Orders.PayOrderPaymentByOrderId; 
using Ordering.Domain.Aggregates.OrderAggregate;
using Ordering.Domain.SeedWork;
using System.Data;

namespace Ordering.API.Applications.Commands.Orders.PayOrder;

public class PayOrderPaymentByOrderIdCommandHandler(
    ILogger<CreateOrderCommandHandler> logger, 
    IUnitOfWork unitOfWork) : IRequestHandler<PayOrderPaymentByOrderIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<CreateOrderCommandHandler> _logger = logger; 

    public async Task<Result> Handle(PayOrderPaymentByOrderIdCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        try
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId, cancellationToken); 
            if (order is null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure("Data not found", ResponseStatus.NotFound);
            }

            order.SetPaymentPaid(new Payment(
                request.OrderId,
                request.Amount,
                request.Currency,
                request.PaidAt,
                request.PaymentMethod,
                request.BankReference));

            _unitOfWork.Orders.Update(order); 

            if (order.IsScheduled && order.IsShouldRequestPayment)
            {
                var scheduledOrder = await _unitOfWork.ScheduledOrders.GetByOrderIdAsync(order.Id);
                if (scheduledOrder is not null)
                {
                    if (order.Payment!.CreatedAt >= scheduledOrder.ScheduledAt)
                    {
                        scheduledOrder.SetNow();
                    } 
                    scheduledOrder.MarkAsPaid();
                    _unitOfWork.ScheduledOrders.Update(scheduledOrder);
                }
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result.Success("OK", ResponseStatus.Ok);
        }
        catch (ArgumentNullException ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (DomainException ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (NotImplementedException ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "An error occured , Error: {error}, Detail: {Detail}", ex.Message, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "An error occured , Error: {error}, Detail: {Detail}", ex.Message, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }
}
