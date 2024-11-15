using Core.CustomMessages;
using Core.Dtos;
using Core.Exceptions;
using Core.Results;
using MediatR;
using Ordering.API.Applications.Dtos.Responses;
using Ordering.API.Applications.Queries.Orders.GetPaginatedOrdersByUserId;
using Ordering.API.Extensions;
using Ordering.Domain.SeedWork;

namespace Ordering.API.Applications.Queries.Orders.GetPaginatedOrdersByMechanicUserId;

public class GetPaginatedOrdersByMechanicUserIdQueryHandler(
    ILogger<GetPaginatedOrdersByMechanicUserIdQueryHandler> logger,
    IUnitOfWork unitOfWork) : IRequestHandler<GetPaginatedOrdersByMechanicUserIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<GetPaginatedOrdersByMechanicUserIdQueryHandler> _logger = logger;

    public async Task<Result> Handle(GetPaginatedOrdersByMechanicUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            (var totalCount, var totalPages, var items) = await _unitOfWork.Orders.GetPaginatedOrdersByMechanicId(request.UserId, request.PageNumber, request.PageSize, request.Status);

            if (items is null)
            {
                return Result.Failure("Data not found", ResponseStatus.NotFound);
            }

            var response = new PaginatedResponseDto<OrderResponseDto>(request.PageNumber, request.PageSize, totalCount, totalPages, items.Select(x => x.ToOrderResponseDto()));

            return Result.Success(response, ResponseStatus.Ok);
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
