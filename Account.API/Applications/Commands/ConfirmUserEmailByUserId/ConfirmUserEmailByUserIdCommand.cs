using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.ConfirmUserEmailByUserId;

public record ConfirmUserEmailByUserIdCommand(Guid Id) : IRequest<Result>;
