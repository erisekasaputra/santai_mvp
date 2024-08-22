using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.StaffCommand.ConfirmStaffEmailByStaffId;

public record ConfirmStaffEmailByStaffIdCommand(Guid StaffId) : IRequest<Result>;
