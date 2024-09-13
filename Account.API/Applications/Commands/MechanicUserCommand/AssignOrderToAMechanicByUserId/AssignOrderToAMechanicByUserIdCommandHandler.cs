using Core.Results;
using MediatR;

namespace Account.API.Applications.Commands.MechanicUserCommand.AssignOrderToAMechanicByUserId;

public class AssignOrderToAMechanicByUserIdCommandHandler : IRequestHandler<AssignOrderToAMechanicByUserIdCommand, Result>
{
    public Task<Result> Handle(AssignOrderToAMechanicByUserIdCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
