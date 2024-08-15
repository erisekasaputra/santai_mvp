using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Queries.GetStaffByUserIdAndStaffId;

public record GetStaffByUserIdAndStaffIdQuery(Guid UserId, Guid StaffId) : IRequest<Result>;