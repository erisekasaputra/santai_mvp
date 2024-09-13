using Core.Results;
using MediatR;

namespace Account.API.Applications.Commands.MechanicUserCommand.UnassignOrderFromAMechanicByUserId;

public record UnassignOrderFromAMechanicByUserIdCommand(Guid MechanicId) : IRequest<Result>;
