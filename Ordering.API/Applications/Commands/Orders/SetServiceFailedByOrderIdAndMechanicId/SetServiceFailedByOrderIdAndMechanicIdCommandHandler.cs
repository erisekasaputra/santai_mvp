using Core.Results; 
using MediatR;  
using Ordering.Domain.SeedWork; 
using Core.Exceptions;
using System.Data;
using Core.Messages;

namespace Ordering.API.Applications.Commands.Orders.SetServiceFailedByOrderIdAndMechanicId;

public class SetServiceFailedByOrderIdAndMechanicIdCommandHandler(
    ILogger<SetServiceFailedByOrderIdAndMechanicIdCommandHandler> logger, 
    IUnitOfWork unitOfWork) : IRequestHandler<SetServiceFailedByOrderIdAndMechanicIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<SetServiceFailedByOrderIdAndMechanicIdCommandHandler> _logger = logger;
    public async Task<Result> Handle(SetServiceFailedByOrderIdAndMechanicIdCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        try
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId, cancellationToken);

            if (order is null)
            {
                return Result.Failure("Data not found", ResponseStatus.NotFound);
            }

            order.SetServiceIncompleted(request.MechanicId, request.Secret, request.FleetId);

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
