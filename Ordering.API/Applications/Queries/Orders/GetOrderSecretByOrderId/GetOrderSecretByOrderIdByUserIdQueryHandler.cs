using Core.CustomMessages;
using Core.Exceptions;
using Core.Results;
using MediatR;
using Ordering.API.Applications.Dtos.Responses;
using Ordering.API.Applications.Queries.Orders.GetOrderByIdAndUserId;
using Ordering.Domain.SeedWork;

namespace Ordering.API.Applications.Queries.Orders.GetOrderSecretByOrderId;

public class GetOrderSecretByOrderIdByUserIdQueryHandler(
    ILogger<GetOrderByIdAndUserIdQueryHandler> logger,
    IUnitOfWork unitOfWork) : IRequestHandler<GetOrderSecretByOrderIdByUserIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<GetOrderByIdAndUserIdQueryHandler> _logger = logger;
    public async Task<Result> Handle(GetOrderSecretByOrderIdByUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var secret = await _unitOfWork.Orders.GetOrderSecretByOrderIdAndUserId(request.OrderId, request.UserId, cancellationToken);

            if (secret is null)
            {
                return Result.Failure("Data not found", ResponseStatus.NotFound);
            }

            return Result.Success(new OrderSecretResponse(secret), ResponseStatus.Ok);
        }
        catch (ArgumentNullException ex)
        {
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (NotImplementedException ex)
        {
            _logger.LogError(ex, "An error occured , Error: {error}, Detail: {Detail}", ex.Message, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured , Error: {error}, Detail: {Detail}", ex.Message, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }
}
