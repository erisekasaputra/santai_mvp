using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.DeleteBusinessUserByUserId;

public record DeleteBusinessUserByUserIdCommand(Guid Id) : IRequest<Result>;
