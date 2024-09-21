using Core.Results; 
using MediatR;

namespace Account.API.Applications.Commands.UserCommand.SetDeviceIdByUserId;

public record SetDeviceIdByUserIdCommand(Guid UserId, string DeviceId) : IRequest<Result>;