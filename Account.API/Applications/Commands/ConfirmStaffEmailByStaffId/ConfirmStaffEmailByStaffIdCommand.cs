using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.ConfirmStaffEmailByStaffId;

public record ConfirmStaffEmailByStaffIdCommand(Guid BusinessUserId, Guid StaffId) : IRequest<Result>;
