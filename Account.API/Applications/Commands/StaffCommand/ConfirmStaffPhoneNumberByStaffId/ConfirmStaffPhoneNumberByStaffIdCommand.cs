using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.StaffCommand.ConfirmStaffPhoneNumberByStaffId;

public record ConfirmStaffPhoneNumberByStaffIdCommand(Guid BusinessUserId, Guid StaffId) : IRequest<Result>;
