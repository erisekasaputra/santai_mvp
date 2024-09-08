using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Commands.StaffCommand.ResetPhoneNumberByStaffId;

public record ResetPhoneNumberByStaffIdCommand(Guid Id) : IRequest<Result>;
