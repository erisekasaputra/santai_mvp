using Core.Results;
using MediatR;

namespace Account.API.Applications.Commands.MechanicUserCommand.DeactivateMechanicStatusByUserId;

public record DeactivateMechanicStatusByUserIdCommand(Guid MechanicId) : IRequest<Result>;
