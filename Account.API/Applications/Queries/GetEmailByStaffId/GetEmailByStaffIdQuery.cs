using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Queries.GetEmailByStaffId;

public record GetEmailByStaffIdQuery(Guid UserId, Guid StaffId) : IRequest<Result>; 
