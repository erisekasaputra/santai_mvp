using Core.Results; 
using MediatR;

namespace Account.API.Applications.Queries.GetPhoneNumberByStaffId;

public record GetPhoneNumberByStaffIdQuery(Guid StaffId) : IRequest<Result>;