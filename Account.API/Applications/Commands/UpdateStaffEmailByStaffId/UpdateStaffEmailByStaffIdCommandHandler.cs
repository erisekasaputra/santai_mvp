using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.UpdateStaffEmailByStaffId;

public class UpdateStaffEmailByStaffIdCommandHandler : IRequestHandler<UpdateStaffEmailByStaffIdCommand, Result>
{
    public Task<Result> Handle(UpdateStaffEmailByStaffIdCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
