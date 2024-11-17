using Core.Results;
using MediatR;

namespace Ordering.API.Applications.Queries.Orders.GetOrderByIdAndMechanicId;

public record GetOrderByIdAndMechanicIdQuery(Guid OrderId, Guid UserId) : IRequest<Result>; 