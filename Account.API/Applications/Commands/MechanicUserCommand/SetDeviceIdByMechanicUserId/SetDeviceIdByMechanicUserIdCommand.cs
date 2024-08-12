using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.MechanicUserCommand.SetDeviceIdByMechanicUserId;

public record SetDeviceIdByMechanicUserIdCommand(Guid UserId, string DeviceId) : IRequest<Result>;