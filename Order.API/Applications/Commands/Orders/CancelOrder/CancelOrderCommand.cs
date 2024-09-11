using Core.Results;
using MediatR;

namespace Order.API.Applications.Commands.Orders.CancelOrder;

public record CancelOrderCommand(Guid OrderId, Guid BuyerId) : IRequest<Result>;
