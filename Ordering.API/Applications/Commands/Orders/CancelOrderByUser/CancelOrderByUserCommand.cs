using Core.Results;
using MediatR;

namespace Ordering.API.Applications.Commands.Orders.CancelOrderByUser;

public record CancelOrderByUserCommand(Guid OrderId, Guid BuyerId) : IRequest<Result>;
