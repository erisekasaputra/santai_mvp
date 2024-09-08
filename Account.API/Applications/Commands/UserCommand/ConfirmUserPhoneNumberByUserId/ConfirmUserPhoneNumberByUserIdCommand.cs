using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Commands.UserCommand.ConfirmUserPhoneNumberByUserId;

public record ConfirmUserPhoneNumberByUserIdCommand(Guid Id) : IRequest<Result>;
