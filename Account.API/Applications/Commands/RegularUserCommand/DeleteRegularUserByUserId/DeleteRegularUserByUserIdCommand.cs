using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Commands.RegularUserCommand.DeleteRegularUserByUserId;

public record DeleteRegularUserByUserIdCommand(Guid UserId) : IRequest<Result>;
