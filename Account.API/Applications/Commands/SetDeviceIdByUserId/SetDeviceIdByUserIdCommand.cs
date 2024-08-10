using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.SetDeviceIdByUserId;

public record SetDeviceIdByUserIdCommand(Guid UserId, string DeviceId) : IRequest<Result>;