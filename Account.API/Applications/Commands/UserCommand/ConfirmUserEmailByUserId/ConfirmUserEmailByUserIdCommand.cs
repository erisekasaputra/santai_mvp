using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Commands.UserCommand.ConfirmUserEmailByUserId;

public record ConfirmUserEmailByUserIdCommand(Guid Id) : IRequest<Result>;
