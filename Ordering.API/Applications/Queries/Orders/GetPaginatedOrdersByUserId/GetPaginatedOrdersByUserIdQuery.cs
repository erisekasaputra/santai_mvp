using Core.Results;
using MediatR;
using Ordering.Domain.Enumerations;

namespace Ordering.API.Applications.Queries.Orders.GetPaginatedOrdersByUserId;

public record GetPaginatedOrdersByUserIdQuery(Guid? UserId, int PageNumber, int PageSize, OrderStatus? Status) : IRequest<Result>;
