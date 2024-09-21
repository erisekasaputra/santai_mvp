using Core.Results; 
using MediatR;

namespace Account.API.Applications.Commands.RegularUserCommand.DeleteRegularUserByUserId;

public record DeleteRegularUserByUserIdCommand(Guid UserId) : IRequest<Result>;
