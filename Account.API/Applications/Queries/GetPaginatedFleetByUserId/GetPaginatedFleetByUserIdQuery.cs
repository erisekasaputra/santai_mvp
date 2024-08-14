
using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Queries.GetPaginatedFleetByUserId;

public record GetPaginatedFleetByUserIdQuery(Guid UserId, int PageNumber, int PageSize) : IRequest<Result>;