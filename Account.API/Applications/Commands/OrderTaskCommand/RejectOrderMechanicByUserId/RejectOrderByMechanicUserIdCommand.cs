using Core.Results;
using MediatR;

namespace Account.API.Applications.Commands.OrderTaskCommand.RejectOrderMechanicByUserId;

public record RejectOrderByMechanicUserIdCommand(Guid OrderId, Guid MechanicId) : IRequest<Result>;
