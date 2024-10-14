using Core.Results; 
using MediatR;

namespace Account.API.Applications.Queries.GetStaffByUserIdAndStaffId;

public record GetStaffByUserIdAndStaffIdQuery(Guid StaffId) : IRequest<Result>;