using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.UserCommand.ForceSetDeviceIdByUserId;

public record ForceSetDeviceIdByUserIdCommand(Guid UserId, string DeviceId) : IRequest<Result>;