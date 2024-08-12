using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.StaffCommand.SetDeviceIdByStaffId;

public record SetDeviceIdByStaffIdCommand(Guid UserId, Guid StaffId, string DeviceId) : IRequest<Result>;

