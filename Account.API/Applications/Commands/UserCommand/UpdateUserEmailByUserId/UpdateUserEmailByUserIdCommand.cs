using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Commands.UserCommand.UpdateUserEmailByUserId;

public record UpdateUserEmailByUserIdCommand(Guid Id, string Email) : IRequest<Result>;
