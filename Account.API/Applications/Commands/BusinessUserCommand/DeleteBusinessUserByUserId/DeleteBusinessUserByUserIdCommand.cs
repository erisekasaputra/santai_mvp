using Core.Results;
using MediatR;

namespace Account.API.Applications.Commands.BusinessUserCommand.DeleteBusinessUserByUserId;

public record DeleteBusinessUserByUserIdCommand(Guid Id) : IRequest<Result>;
