using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.DeleteRegularUserByUserId;

public record DeleteRegularUserByUserIdCommand(Guid UserId) : IRequest<Result>;
