using Core.Results;
using MediatR;

namespace Account.API.Applications.Commands.MechanicUserCommand.VerifyMechanicUserByUserId;

public record VerifyMechanicUserByUserIdCommand(Guid UserId) : IRequest<Result>;
