using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Commands.StaffCommand.ResetDeviceIdByStaffId;

public record ResetDeviceIdByStaffIdCommand(Guid StaffId) : IRequest<Result>;