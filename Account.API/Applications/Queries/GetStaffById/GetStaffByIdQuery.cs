using Core.Results;
using MediatR;

namespace Account.API.Applications.Queries.GetStaffById;

public record GetStaffByIdQuery(Guid StaffId) : IRequest<Result>;