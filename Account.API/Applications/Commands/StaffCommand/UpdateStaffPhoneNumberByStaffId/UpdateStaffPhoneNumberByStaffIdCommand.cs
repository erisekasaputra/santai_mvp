using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.StaffCommand.UpdateStaffPhoneNumberByStaffId;

public record UpdateStaffPhoneNumberByStaffIdCommand(Guid BusinessUserId, Guid StaffId, string PhoneNumber) : IRequest<Result>;
