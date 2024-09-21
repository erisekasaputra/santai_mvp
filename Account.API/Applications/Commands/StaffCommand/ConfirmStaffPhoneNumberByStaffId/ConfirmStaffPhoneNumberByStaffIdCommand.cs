using Core.Results;  
using MediatR;

namespace Account.API.Applications.Commands.StaffCommand.ConfirmStaffPhoneNumberByStaffId;

public record ConfirmStaffPhoneNumberByStaffIdCommand(Guid StaffId) : IRequest<Result>;
