using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.StaffCommand.RemoveStaffByUserId;

public record RemoveStaffByUserIdCommand(Guid BusinessUserId, Guid StaffId) : IRequest<Result>;
