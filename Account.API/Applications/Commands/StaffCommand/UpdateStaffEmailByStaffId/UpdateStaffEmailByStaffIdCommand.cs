using Core.Results; 
using MediatR;

namespace Account.API.Applications.Commands.StaffCommand.UpdateStaffEmailByStaffId;

public record UpdateStaffEmailByStaffIdCommand(Guid StaffId, string Email) : IRequest<Result>;
