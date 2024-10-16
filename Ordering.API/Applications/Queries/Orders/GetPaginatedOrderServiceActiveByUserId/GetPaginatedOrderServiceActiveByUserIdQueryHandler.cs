using Core.CustomMessages;
using Core.Results;
using Core.Utilities;
using MediatR;
using Ordering.API.Extensions;
using Ordering.Domain.SeedWork;

namespace Ordering.API.Applications.Queries.Orders.GetPaginatedOrderServiceActiveByUserId;

public class GetPaginatedOrderServiceActiveByUserIdQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<GetPaginatedOrderServiceActiveByUserIdQueryHandler> logger) : IRequestHandler<GetPaginatedOrderServiceActiveByUserIdQuery, Result>
{
    private readonly ILogger<GetPaginatedOrderServiceActiveByUserIdQueryHandler> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result> Handle(GetPaginatedOrderServiceActiveByUserIdQuery request, CancellationToken cancellationToken)
    {
        try 
        {
            var orders = await _unitOfWork.Orders.GetOrderServicesActive(request.UserId);

            if (orders == null) 
            {
                return Result.Failure("You have no active order", ResponseStatus.NotFound);
            }

            return Result.Success(orders.ToOrderActivesDto(), ResponseStatus.Ok);
        } 
        catch (Exception ex)
        { 
            LoggerHelper.LogError(_logger, ex);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
        
    }
}
