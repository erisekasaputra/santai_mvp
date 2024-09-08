using Core.Results;
using MediatR;

namespace Account.API.Applications.Commands.MechanicUserCommand.ForceSetDeviceIdByMechanicUserId;

public record ForceSetDeviceIdByMechanicUserIdCommand(Guid UserId, string DeviceId) : IRequest<Result>;
