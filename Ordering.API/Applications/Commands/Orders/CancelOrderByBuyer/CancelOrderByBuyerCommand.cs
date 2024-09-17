using Core.Results;
using MediatR;

namespace Ordering.API.Applications.Commands.Orders.CancelOrderByBuyer;

public record CancelOrderByBuyerCommand(Guid OrderId, Guid BuyerId) : IRequest<Result>;
