using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.ResetDeviceIdByUserId;

public record ResetDeviceIdByUserIdCommand(Guid UserId) : IRequest<Result>;
