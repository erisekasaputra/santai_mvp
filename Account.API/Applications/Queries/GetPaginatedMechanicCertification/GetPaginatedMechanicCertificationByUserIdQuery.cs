using Core.Results;  
using MediatR;

namespace Account.API.Applications.Queries.GetPaginatedMechanicCertificationByUserId;

public record GetPaginatedMechanicCertificationByUserIdQuery(Guid UserId, int PageNumber, int PageSize) : IRequest<Result>;
