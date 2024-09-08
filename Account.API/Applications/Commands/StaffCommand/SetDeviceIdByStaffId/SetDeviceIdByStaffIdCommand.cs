using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Commands.StaffCommand.SetDeviceIdByStaffId;

public record SetDeviceIdByStaffIdCommand(Guid StaffId, string DeviceId) : IRequest<Result>;

