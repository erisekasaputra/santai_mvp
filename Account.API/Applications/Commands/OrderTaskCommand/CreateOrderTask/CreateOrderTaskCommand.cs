using Core.Results;
using MediatR;

namespace Account.API.Applications.Commands.OrderTaskCommand.CreateOrderTask;

public record CreateOrderTaskCommand(Guid OrderId, double Latitude, double Longitude) : IRequest<Result>;