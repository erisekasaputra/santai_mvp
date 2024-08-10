using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.UpdateStaffEmailByStaffId;

public record UpdateStaffEmailByStaffIdCommand(Guid BusinessUserId, Guid StaffId, string Email) : IRequest<Result>;
