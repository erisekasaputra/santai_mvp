using Core.Results;
using MediatR;

namespace Account.API.Applications.Commands.FleetCommand.AssignFleetsToStaff;

public record AssignFleetsToStaffCommand(Guid UserId, Guid StaffId, IEnumerable<Guid> FleetIds) : IRequest<Result>;