
using Core.Results;
using MediatR;

namespace Account.API.Applications.Commands.OrderTaskCommand.CancelOrderByMechanicByIdByOrderId;

public record CancelOrderByMechanicByIdByOrderIdCommand(Guid MechanicId, Guid OrderId) : IRequest<Result>;