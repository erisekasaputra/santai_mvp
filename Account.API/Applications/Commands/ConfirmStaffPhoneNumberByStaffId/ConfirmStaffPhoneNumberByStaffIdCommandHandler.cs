using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.ConfirmStaffPhoneNumberByStaffId;

public class ConfirmStaffPhoneNumberByStaffIdCommandHandler : IRequestHandler<ConfirmStaffPhoneNumberByStaffIdCommand, Result>
{
    public Task<Result> Handle(ConfirmStaffPhoneNumberByStaffIdCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
