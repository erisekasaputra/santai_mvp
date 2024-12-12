using Core.Results;
using MediatR;

namespace Account.API.Applications.Commands.MechanicUserCommand.BlockMechanicStatusByUserId;

public record BlockMechanicStatusByUserIdCommand(Guid UserId, string Reason) : IRequest<Result>;
