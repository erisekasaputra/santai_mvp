using Core.Results;
using MediatR;
using Ordering.Domain.Enumerations;

namespace Ordering.API.Applications.Queries.Orders.GetPaginatedOrdersByMechanicUserId;

public record GetPaginatedOrdersByMechanicUserIdQuery(Guid UserId, int PageNumber, int PageSize, OrderStatus? Status) : IRequest<Result>;
