using Core.Results; 
using MediatR;

namespace Account.API.Applications.Commands.StaffCommand.RemoveStaffByUserId;

public record RemoveStaffByUserIdCommand(Guid StaffId) : IRequest<Result>;
