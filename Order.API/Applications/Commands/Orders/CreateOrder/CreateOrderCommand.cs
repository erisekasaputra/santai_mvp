using MediatR;
using Order.API.SeedWorks;

namespace Order.API.Applications.Commands.Orders.CreateOrder;

public record CreateOrderCommand : IRequest<Result>;
