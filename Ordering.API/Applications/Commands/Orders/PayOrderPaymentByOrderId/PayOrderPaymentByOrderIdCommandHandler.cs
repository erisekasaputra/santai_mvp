using Core.Enumerations;
using Core.Exceptions;
using Core.Messages;
using Core.Results;
using MediatR;
using Ordering.API.Applications.Commands.Orders.CreateOrder;
using Ordering.API.Applications.Commands.Orders.PayOrderPaymentByOrderId;
using Ordering.API.Applications.Dtos.Requests;
using Ordering.API.Applications.Services.Interfaces;
using Ordering.Domain.Aggregates.OrderAggregate;
using Ordering.Domain.SeedWork;
using System.Data;

namespace Ordering.API.Applications.Commands.Orders.PayOrder;

public class PayOrderPaymentByOrderIdCommandHandler(
    ILogger<CreateOrderCommandHandler> logger, 
    IPaymentService paymentService,
    IUnitOfWork unitOfWork) : IRequestHandler<PayOrderPaymentByOrderIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<CreateOrderCommandHandler> _logger = logger;
    private readonly IPaymentService _paymentService = paymentService;
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

            if (_paymentService.ValidatePayment(
                new SenangPayPaymentRequest(request.TransactionId, request.OrderId, request.Amount, request.Method, request.Message, request.Status, request.Hash),
                request.Hash))
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure("Failed to authorize hash payment", ResponseStatus.BadRequest);
            }

            order.SetPaymentPaid(new Payment(
                request.OrderId,
                request.Amount,
                order.Currency,
                DateTime.UtcNow,
                request.Method,
                request.TransactionId));

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

            return Result.Success("Payment successfull", ResponseStatus.Ok);
        }
        catch (ArgumentNullException ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "An error occured , Error: {error}, Detail: {Detail}", ex.Message, ex.InnerException?.Message);
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (DomainException ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "An error occured , Error: {error}, Detail: {Detail}", ex.Message, ex.InnerException?.Message);
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
