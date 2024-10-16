using Core.Results;
using MediatR;

namespace Ordering.API.Applications.Queries.Orders.GetPaginatedOrderServiceActiveByUserId;

public record GetPaginatedOrderServiceActiveByUserIdQuery(
    Guid UserId) : IRequest<Result>;
