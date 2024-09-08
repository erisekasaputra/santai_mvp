using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Commands.StaffCommand.RemoveStaffByUserId;

public record RemoveStaffByUserIdCommand(Guid BusinessUserId, Guid StaffId) : IRequest<Result>;
