using Core.CustomMessages;
using Core.Exceptions;
using Core.Results;
using MediatR;
using Ordering.API.Applications.Commands.Orders.SetOrderFleetPreServiceInspection;
using Ordering.Domain.SeedWork;
using System.Data;

namespace Ordering.API.Applications.Commands.Orders.SetOrderFleetJobChecklist;

public class SetOrderFleetJobChecklistCommandHandler(
    ILogger<SetOrderFleetJobChecklistCommandHandler> logger,
    IUnitOfWork unitOfWork) : IRequestHandler<SetOrderFleetJobChecklistCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<SetOrderFleetJobChecklistCommandHandler> _logger = logger;
    public async Task<Result> Handle(SetOrderFleetJobChecklistCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        try
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId, cancellationToken);

            if (order is null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure("We could not find the order", ResponseStatus.NotFound);
            }

            var fleet = order.Fleets.FirstOrDefault(x => x.FleetId == request.FleetId);
            if (fleet is null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure("Fleet not found", ResponseStatus.NotFound);
            }

            if (!string.IsNullOrEmpty(request.Comment))
            {
                fleet.UpdateComment(request.Comment);
            }

            foreach (var jobChecklist in request.JobChecklists)
            {
                if (!fleet.PutJobChecklist(
                    jobChecklist.Parameter,
                    jobChecklist.Value))
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.Failure("Job check list inspection parameter not found", ResponseStatus.NotFound);
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
