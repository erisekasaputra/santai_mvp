using Core.Results; 
using MediatR;

namespace Account.API.Applications.Commands.StaffCommand.ResetPhoneNumberByStaffId;

public record ResetPhoneNumberByStaffIdCommand(Guid Id) : IRequest<Result>;
