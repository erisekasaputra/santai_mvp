using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.StaffCommand.ResetDeviceIdByStaffId;

public record ResetDeviceIdByStaffIdCommand(Guid UserId, Guid StaffId) : IRequest<Result>;