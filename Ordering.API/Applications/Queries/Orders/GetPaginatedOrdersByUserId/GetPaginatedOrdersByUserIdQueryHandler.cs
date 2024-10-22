using Core.CustomMessages;
using Core.Dtos;
using Core.Exceptions;
using Core.Results;
using MediatR;
using Ordering.API.Applications.Dtos.Responses;
using Ordering.API.Extensions;
using Ordering.Domain.SeedWork;

namespace Ordering.API.Applications.Queries.Orders.GetPaginatedOrdersByUserId;

public class GetPaginatedOrdersByUserIdQueryHandler(
    ILogger<GetPaginatedOrdersByUserIdQueryHandler> logger,
    IUnitOfWork unitOfWork) : IRequestHandler<GetPaginatedOrdersByUserIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<GetPaginatedOrdersByUserIdQueryHandler> _logger = logger;

    public async Task<Result> Handle(GetPaginatedOrdersByUserIdQuery request, CancellationToken cancellationToken)
    { 
        try
        {
            (var totalCount, var totalPages, var items) = await _unitOfWork.Orders.GetPaginatedOrders(request.UserId, request.PageNumber, request.PageSize);

            if (items is null)
            {
                return Result.Failure("Data not found", ResponseStatus.NotFound);
            } 

            var response = new PaginatedResponseDto<OrderResponseDto>(request.PageNumber, request.PageSize, totalCount, totalPages, items.Select(x => x.ToOrderResponseDto()));

            return Result.Success(response , ResponseStatus.Ok);
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
