using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Queries.GetPaginatedStaffByUserId;

public record GetPaginatedStaffByUserIdQuery(Guid UserId, int PageNumber, int PageSize) : IRequest<Result>;
