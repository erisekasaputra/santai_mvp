using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Commands.MechanicUserCommand.ResetDeviceIdByMechanicUserId;

public record ResetDeviceIdByMechanicUserIdCommand(Guid UserId) : IRequest<Result>;
