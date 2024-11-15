
using Account.API.Applications.Models;
using Account.API.Applications.Services;
using Account.API.Applications.Services.Interfaces;
using Core.CustomMessages;
using Core.Results;
using Core.Utilities;
using MediatR;

namespace Account.API.Applications.Queries.GetMechanicStatusExistence;

public class GetMechanicStatusExistenceQueryHandler
    (IMechanicCache mechanicCache, ApplicationService service) : IRequestHandler<GetMechanicStatusExistenceQuery, Result>
{
    private readonly IMechanicCache _mechanicCache = mechanicCache;
    private readonly ApplicationService _service = service;     
    public async Task<Result> Handle(GetMechanicStatusExistenceQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var mechanic = await _mechanicCache.GetMechanicExistence(request.MechanicId.ToString());  
            if (mechanic == null)
            {
                return Result.Failure("Mechanic not found", ResponseStatus.NotFound);
            }

            OrderTaskMechanicConfirm? waitingTask = null;
            if (!string.IsNullOrEmpty(mechanic.OrderId))
            {
                waitingTask = await _mechanicCache.GetOrderWaitingMechanicConfirmationAsync(mechanic.OrderId);
            }

            OrderTask? orderTask = null;
            if (mechanic.Status == MechanicStatus.Bussy && !string.IsNullOrEmpty(mechanic.OrderId))
            {
                orderTask = await _mechanicCache.GetOrderTaskByOrderIdAsync(mechanic.OrderId);
            }

            return Result.Success(ToResponse(mechanic, waitingTask, orderTask), ResponseStatus.Ok);
        }
        catch (Exception ex) 
        {
            LoggerHelper.LogError(_service.Logger, ex);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }

    private static object ToResponse(
        MechanicExistence mechanic,
        OrderTaskMechanicConfirm? waitingTask,
        OrderTask? orderTask)
    { 
        return new 
        {
            mechanic.MechanicId,
            mechanic.Latitude,
            mechanic.Longitude,
            mechanic.OrderId,
            mechanic.Status,
            RemainingTime = waitingTask is null ? 0 : (int)(waitingTask.ExpiredAtUtc - DateTime.UtcNow).TotalSeconds,
            OrderTask = orderTask is null ? null : new
            {
                orderTask.BuyerId,
                orderTask.MechanicId,
                orderTask.OrderId,
                orderTask.Latitude,
                orderTask.Longitude,    
                orderTask.OrderStatus
            }
        };
    }
}
