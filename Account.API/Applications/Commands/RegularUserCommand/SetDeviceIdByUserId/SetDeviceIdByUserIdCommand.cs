using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.RegularUserCommand.SetDeviceIdByUserId;

public record SetDeviceIdByUserIdCommand(Guid UserId, string DeviceId) : IRequest<Result>;