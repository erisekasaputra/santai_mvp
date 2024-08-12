using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.StaffCommand.ForceSetDeviceIdByStaffId;

public record ForceSetDeviceIdByStaffIdCommand(Guid UserId, Guid StaffId, string DeviceId) : IRequest<Result>;
