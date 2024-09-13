using Core.Results;
using MediatR;

namespace Account.API.Applications.Commands.MechanicUserCommand.UnassignOrderFromAMechanicByUserId;

public class UnassignOrderFromAMechanicByUserIdCommandHandler : IRequestHandler<UnassignOrderFromAMechanicByUserIdCommand, Result>
{
    public Task<Result> Handle(UnassignOrderFromAMechanicByUserIdCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
