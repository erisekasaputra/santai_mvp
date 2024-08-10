using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.ConfirmStaffEmailByStaffId;

public class ConfirmStaffEmailByStaffIdCommandHandler : IRequestHandler<ConfirmStaffEmailByStaffIdCommand, Result>
{
    public Task<Result> Handle(ConfirmStaffEmailByStaffIdCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
