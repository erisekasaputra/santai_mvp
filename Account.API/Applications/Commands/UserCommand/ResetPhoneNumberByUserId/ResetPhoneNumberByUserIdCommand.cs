using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Commands.UserCommand.ResetPhoneNumberByUserId;

public record ResetPhoneNumberByUserIdCommand(Guid Id) : IRequest<Result>;
