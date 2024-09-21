using Core.Results; 
using MediatR;

namespace Account.API.Applications.Commands.StaffCommand.AssignStaffEmailByUserId;

public record AssignStaffEmailByUserIdCommand(Guid UserId, string Email) : IRequest<Result>; 
