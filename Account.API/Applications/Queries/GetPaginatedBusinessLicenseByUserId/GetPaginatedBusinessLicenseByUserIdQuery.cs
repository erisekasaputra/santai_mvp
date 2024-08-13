using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Queries.GetPaginatedBusinessLicenseByUserId;

public record GetPaginatedBusinessLicenseByUserIdQuery(Guid UserId, int PageNumber, int PageSize) : IRequest<Result>;
