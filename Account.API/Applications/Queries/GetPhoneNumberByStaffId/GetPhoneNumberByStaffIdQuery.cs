using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Queries.GetPhoneNumberByStaffId;

public record GetPhoneNumberByStaffIdQuery(Guid StaffId) : IRequest<Result>;