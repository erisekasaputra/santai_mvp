
using Core.CustomMessages;
using Core.Exceptions;
using Core.Results;
using MediatR;
using Ordering.API.Applications.Commands.Orders.CreateOrder;
using Ordering.API.Applications.Services.Interfaces;
using Ordering.Domain.SeedWork;
using System.Data;

namespace Ordering.API.Applications.Commands.Orders.CancelOrderByBuyer;

public class CancelOrderByBuyerCommandHandler(
    ILogger<CreateOrderCommandHandler> logger, 
    IUnitOfWork unitOfWork,
    IMasterDataServiceAPI masterDataServiceAPI) : IRequestHandler<CancelOrderByBuyerCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<CreateOrderCommandHandler> _logger = logger;
    private readonly IMasterDataServiceAPI _masterDataServiceAPI = masterDataServiceAPI;
     
    public async Task<Result> Handle(CancelOrderByBuyerCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        try
        {
            var cancellationParameter = await _masterDataServiceAPI.GetCancellationFeeParametersMaster();

            if (cancellationParameter is null)
            {
                _logger.LogError("Cancellation fee parameter is null, Master data did not retrive correct data");
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
            }

            var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId, cancellationToken);

            if (order is null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken); 
                return Result.Failure("Data not found", ResponseStatus.NotFound);
            }

            order.CancelByBuyer(request.BuyerId, cancellationParameter.CancellationParameters);

            _unitOfWork.Orders.Update(order);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result.Success(null, ResponseStatus.NoContent);
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
