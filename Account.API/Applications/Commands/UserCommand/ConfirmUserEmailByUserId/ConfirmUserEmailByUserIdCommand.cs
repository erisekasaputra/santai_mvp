using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.UserCommand.ConfirmUserEmailByUserId;

public record ConfirmUserEmailByUserIdCommand(Guid Id) : IRequest<Result>;
