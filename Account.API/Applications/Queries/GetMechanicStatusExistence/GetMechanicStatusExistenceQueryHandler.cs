
using Account.API.Applications.Models;
using Account.API.Applications.Services.Interfaces;
using Core.CustomMessages;
using Core.Results;
using Core.Utilities;
using MediatR;

namespace Account.API.Applications.Queries.GetMechanicStatusExistence;

public class GetMechanicStatusExistenceQueryHandler
    (IMechanicCache mechanicCache, ILogger logger) : IRequestHandler<GetMechanicStatusExistenceQuery, Result>
{
    private readonly IMechanicCache _mechanicCache = mechanicCache;
    private readonly ILogger _logger = logger;
    public async Task<Result> Handle(GetMechanicStatusExistenceQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var mechanic = await _mechanicCache.GetMechanicExistence(request.MechanicId.ToString());

            if (mechanic == null)
            {
                return Result.Failure("Mechanic not found", ResponseStatus.NotFound);
            }

            OrderTaskMechanicConfirm? task = null;

            if (!string.IsNullOrEmpty(mechanic.OrderId))
            {
                task = await _mechanicCache.GetOrderWaitingMechanicConfirmationAsync(mechanic.OrderId);
            }

            return Result.Success(ToResponse(mechanic, task), ResponseStatus.Ok);
        }
        catch (Exception ex) 
        { 
            LoggerHelper.LogError(_logger, ex); 
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }

    private static object ToResponse(MechanicExistence mechanic, OrderTaskMechanicConfirm? task)
    {
        return new 
        {
            mechanic.MechanicId,
            mechanic.Latitude,
            mechanic.Longitude,
            mechanic.OrderId,
            mechanic.Status,
            RemainingTime = task is null ? 0 : (int)(task.ExpiredAtUtc - DateTime.UtcNow).TotalSeconds,
        };
    }
}
