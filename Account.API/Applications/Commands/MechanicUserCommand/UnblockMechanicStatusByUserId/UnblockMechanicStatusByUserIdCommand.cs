using Core.Results;
using MediatR;

namespace Account.API.Applications.Commands.MechanicUserCommand.UnblockMechanicStatusByUserId;

public record UnblockMechanicStatusByUserIdCommand(Guid UserId) : IRequest<Result>;
