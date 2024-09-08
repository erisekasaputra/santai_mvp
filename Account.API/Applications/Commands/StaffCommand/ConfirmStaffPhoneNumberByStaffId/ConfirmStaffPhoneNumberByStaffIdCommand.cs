using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Commands.StaffCommand.ConfirmStaffPhoneNumberByStaffId;

public record ConfirmStaffPhoneNumberByStaffIdCommand(Guid StaffId) : IRequest<Result>;
