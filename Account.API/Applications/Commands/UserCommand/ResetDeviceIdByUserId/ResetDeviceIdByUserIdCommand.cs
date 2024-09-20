using Core.Results; 
using MediatR;

namespace Account.API.Applications.Commands.UserCommand.ResetDeviceIdByUserId;

public record ResetDeviceIdByUserIdCommand (
    Guid UserId, string DeviceId) : IRequest<Result>;
