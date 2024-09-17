using Core.Results;
using MediatR;

namespace Ordering.API.Applications.Queries.Orders.GetOrderSecretByOrderId;

public record GetOrderSecretByOrderIdByUserIdQuery(Guid OrderId, Guid UserId) : IRequest<Result>;