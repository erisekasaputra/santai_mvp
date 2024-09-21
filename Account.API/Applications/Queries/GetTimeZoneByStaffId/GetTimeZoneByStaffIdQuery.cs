using Core.Results; 
using MediatR;

namespace Account.API.Applications.Queries.GetTimeZoneByStaffId;

public record GetTimeZoneByStaffIdQuery(Guid StaffId) : IRequest<Result>;
