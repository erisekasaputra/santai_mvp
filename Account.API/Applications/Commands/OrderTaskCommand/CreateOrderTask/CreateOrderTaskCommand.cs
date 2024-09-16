using Core.Results;
using MediatR;

namespace Account.API.Applications.Commands.OrderTaskCommand.CreateOrderTask;

public record CreateOrderTaskCommand(Guid BuyerId, Guid OrderId, double Latitude, double Longitude) : IRequest<Result>;