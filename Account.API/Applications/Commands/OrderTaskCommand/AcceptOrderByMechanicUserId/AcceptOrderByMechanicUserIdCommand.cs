using Core.Results;
using MediatR;

namespace Account.API.Applications.Commands.OrderTaskCommand.AcceptOrderByMechanicUserId;

public record AcceptOrderByMechanicUserIdCommand(Guid MechanicId) : IRequest<Result>; 
