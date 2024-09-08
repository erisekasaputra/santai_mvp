using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Commands.MechanicUserCommand.SetDeviceIdByMechanicUserId;

public record SetDeviceIdByMechanicUserIdCommand(Guid UserId, string DeviceId) : IRequest<Result>;