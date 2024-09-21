using Core.Results; 
using MediatR;

namespace Account.API.Applications.Commands.StaffCommand.UpdateStaffPhoneNumberByStaffId;

public record UpdateStaffPhoneNumberByStaffIdCommand(Guid StaffId, string PhoneNumber) : IRequest<Result>;
