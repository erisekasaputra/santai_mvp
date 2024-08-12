using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.UserCommand.ResetDeviceIdByUserId;

public record ResetDeviceIdByUserIdCommand(Guid UserId) : IRequest<Result>;
