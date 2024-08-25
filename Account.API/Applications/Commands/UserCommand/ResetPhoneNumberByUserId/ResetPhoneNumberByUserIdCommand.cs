using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.UserCommand.ResetPhoneNumberByUserId;

public record ResetPhoneNumberByUserIdCommand(Guid Id) : IRequest<Result>;
