using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Commands.StaffCommand.UpdateStaffPhoneNumberByStaffId;

public record UpdateStaffPhoneNumberByStaffIdCommand(Guid StaffId, string PhoneNumber) : IRequest<Result>;
