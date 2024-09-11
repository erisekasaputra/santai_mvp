using Core.Results;
using MediatR;

namespace Order.API.Applications.Commands.Orders.PayOrder;

public record PayOrderCommand(Guid OrderId) : IRequest<Result>;
