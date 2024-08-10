using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.UpdateStaffPhoneNumberByStaffId;

public class UpdateStaffPhoneNumberByStaffIdCommandHandler : IRequestHandler<UpdateStaffPhoneNumberByStaffIdCommand, Result>
{
    public Task<Result> Handle(UpdateStaffPhoneNumberByStaffIdCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
