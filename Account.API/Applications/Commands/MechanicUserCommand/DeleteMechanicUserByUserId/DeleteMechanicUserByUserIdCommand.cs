using Core.Results;
using MediatR;

namespace Account.API.Applications.Commands.MechanicUserCommand.DeleteMechanicUserByUserId;

public record DeleteMechanicUserByUserIdCommand(Guid UserId) : IRequest<Result>;
