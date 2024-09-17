using Core.Results;
using MediatR;

namespace Ordering.API.Applications.Queries.Orders.GetOrderByIdAndUserId;

public record GetOrderByIdAndUserIdQuery(Guid OrderId, Guid UserId) : IRequest<Result>; 
