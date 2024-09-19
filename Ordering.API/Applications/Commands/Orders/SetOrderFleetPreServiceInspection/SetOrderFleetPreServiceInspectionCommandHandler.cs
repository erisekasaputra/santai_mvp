using Core.Exceptions;
using Core.Results;
using Core.Messages;
using MediatR; 
using Ordering.Domain.SeedWork;
using System.Data;

namespace Ordering.API.Applications.Commands.Orders.SetOrderFleetPreServiceInspection;

public class SetOrderFleetPreServiceInspectionCommandHandler(
    ILogger<SetOrderFleetPreServiceInspectionCommandHandler> logger,
    IUnitOfWork unitOfWork) : IRequestHandler<SetOrderFleetPreServiceInspectionCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<SetOrderFleetPreServiceInspectionCommandHandler> _logger = logger;
    public async Task<Result> Handle(SetOrderFleetPreServiceInspectionCommand request, CancellationToken cancellationToken)
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
                    .WithError(new ($"Fleet.FleetId[0]", $"Fleet id {request.FleetId} not found"));
            }

            foreach (var preServiceInspection in request.PreServiceInspections)
            {
                if (!fleet.PutPreServiceInspection(
                    preServiceInspection.Parameter,
                    preServiceInspection.Rating,
                    preServiceInspection.PreServiceInspectionResults.Select(x =>
                    (
                        x.Parameter,
                        x.IsWorking
                    ))))
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.Failure("Pre-Service inspection parameter not found", ResponseStatus.NotFound);
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
