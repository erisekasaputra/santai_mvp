using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Commands.StaffCommand.UpdateStaffEmailByStaffId;

public record UpdateStaffEmailByStaffIdCommand(Guid StaffId, string Email) : IRequest<Result>;
