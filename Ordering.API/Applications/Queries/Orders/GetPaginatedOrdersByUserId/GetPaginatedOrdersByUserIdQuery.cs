using Core.Results;
using MediatR;

namespace Ordering.API.Applications.Queries.Orders.GetPaginatedOrdersByUserId;

public record GetPaginatedOrdersByUserIdQuery(Guid? UserId, int PageNumber, int PageSize) : IRequest<Result>;
