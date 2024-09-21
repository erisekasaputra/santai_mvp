using Core.Results; 
using MediatR;

namespace Account.API.Applications.Commands.UserCommand.AssignUserEmailByUserId;

public record AssignUserEmailByUserIdCommand(Guid UserId, string Email) : IRequest<Result>;