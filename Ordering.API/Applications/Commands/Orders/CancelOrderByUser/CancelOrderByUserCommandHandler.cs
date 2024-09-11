using Core.Enumerations;
using Core.Exceptions;
using Core.Messages;
using Core.Results;
using MediatR;
using Ordering.API.Applications.Commands.Orders.CreateOrder;
using Ordering.API.Applications.Services.Interfaces;
using Ordering.Domain.SeedWork;
using System.Data;

namespace Ordering.API.Applications.Commands.Orders.CancelOrderByUser;

public class CancelOrderByUserCommandHandler(
    ILogger<CreateOrderCommandHandler> logger,
    IPaymentService paymentService,
    IUnitOfWork unitOfWork) : IRequestHandler<CancelOrderByUserCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<CreateOrderCommandHandler> _logger = logger;
    private readonly IPaymentService _paymentService = paymentService;

    private const Currency GlobalCurrency = Currency.MYR;
    public async Task<Result> Handle(CancelOrderByUserCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        try
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId, cancellationToken);

            if (order is null)
            {
                return Result.Failure("Data not found", ResponseStatus.NotFound);
            }

            order.CancelByBuyer(request.BuyerId);

            _unitOfWork.Orders.Update(order);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result.Success("We are sorry you are cancelling the order", ResponseStatus.Ok);
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
