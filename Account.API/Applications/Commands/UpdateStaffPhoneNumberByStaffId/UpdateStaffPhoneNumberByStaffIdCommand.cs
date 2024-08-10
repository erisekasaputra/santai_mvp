using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.UpdateStaffPhoneNumberByStaffId;

public record UpdateStaffPhoneNumberByStaffIdCommand(Guid BusinessUserId, Guid StaffId, string PhoneNumber) : IRequest<Result>;
