using Core.Results;
using MediatR;

namespace Account.API.Applications.Commands.MechanicUserCommand.RejectMechanicUserByUserId;

public record RejectMechanicUserByUserIdCommand(Guid UserId) : IRequest<Result>;
