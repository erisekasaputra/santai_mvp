using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Queries.GetEmailByStaffId;

public record GetEmailByStaffIdQuery(Guid StaffId) : IRequest<Result>; 
