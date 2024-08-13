using Account.API.SeedWork; 
using MediatR;

namespace Account.API.Applications.Queries.GetPaginatedStaffByUserId;

public record GetPaginatedStaffByUserIdQuery(Guid UserId, int PageNumber, int PageSize) : IRequest<Result>;
