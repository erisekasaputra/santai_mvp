using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.MechanicUserCommand.ResetDeviceIdByMechanicUserId;

public record ResetDeviceIdByMechanicUserIdCommand(Guid UserId) : IRequest<Result>;
