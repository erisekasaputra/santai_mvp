using Core.Exceptions;
using Core.Results;
using MediatR;
using Ordering.Domain.SeedWork;
using System.Data;
using Core.CustomMessages;

namespace Ordering.API.Applications.Commands.Orders.SetOrderFleetBasicInspection;

public class SetOrderFleetBasicInspectionCommandHandler(
    ILogger<SetOrderFleetBasicInspectionCommandHandler> logger,
    IUnitOfWork unitOfWork) : IRequestHandler<SetOrderFleetBasicInspectionCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<SetOrderFleetBasicInspectionCommandHandler> _logger = logger;
    public async Task<Result> Handle(SetOrderFleetBasicInspectionCommand request, CancellationToken cancellationToken)
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


            var fleet = order.Fleets.FirstOrDefault(x => x.FleetId == request.FleetId);
            if (fleet is null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure("Data not found", ResponseStatus.NotFound)
                    .WithError(new($"Fleet.FleetId[0]", $"Fleet id {request.FleetId} not found"));
            }

            foreach (var basicInspection in request.BasicInspections)
            {
                if (!fleet.PutBasicInspection(
                    basicInspection.Parameter,
                    basicInspection.Value))
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.Failure("Basic inspection parameter not found", ResponseStatus.NotFound);
                }
            }


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
