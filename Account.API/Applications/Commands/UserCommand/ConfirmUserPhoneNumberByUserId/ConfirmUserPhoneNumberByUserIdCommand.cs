using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.UserCommand.ConfirmUserPhoneNumberByUserId;

public record ConfirmUserPhoneNumberByUserIdCommand(Guid Id) : IRequest<Result>;
