using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Queries.GetStaffByUserIdAndStaffId;

public record GetStaffByUserIdAndStaffIdQuery(Guid UserId, Guid StaffId) : IRequest<Result>;