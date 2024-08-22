using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.StaffCommand.ResetDeviceIdByStaffId;

public record ResetDeviceIdByStaffIdCommand(Guid StaffId) : IRequest<Result>;