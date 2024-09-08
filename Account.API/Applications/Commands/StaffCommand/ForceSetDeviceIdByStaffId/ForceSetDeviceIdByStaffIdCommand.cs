using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Commands.StaffCommand.ForceSetDeviceIdByStaffId;

public record ForceSetDeviceIdByStaffIdCommand(Guid StaffId, string DeviceId) : IRequest<Result>;
