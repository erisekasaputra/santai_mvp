using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.ConfirmUserPhoneNumberByUserId;

public record ConfirmUserPhoneNumberByUserIdCommand(Guid Id) : IRequest<Result>;
