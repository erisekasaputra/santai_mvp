using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Commands.RegularUserCommand.ForceSetDeviceIdByUserId;

public record ForceSetDeviceIdByUserIdCommand(Guid UserId, string DeviceId) : IRequest<Result>;