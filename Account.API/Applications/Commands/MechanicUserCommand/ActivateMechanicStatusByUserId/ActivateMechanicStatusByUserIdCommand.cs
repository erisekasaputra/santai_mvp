using Core.Results;
using MediatR;

namespace Account.API.Applications.Commands.MechanicUserCommand.ActivateMechanicStatusByUserId;

public record ActivateMechanicStatusByUserIdCommand(Guid MechanicId) : IRequest<Result>;
