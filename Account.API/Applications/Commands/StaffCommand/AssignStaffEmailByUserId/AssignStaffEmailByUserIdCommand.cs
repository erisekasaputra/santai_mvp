using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Commands.StaffCommand.AssignStaffEmailByUserId;

public record AssignStaffEmailByUserIdCommand(Guid UserId, string Email) : IRequest<Result>; 
