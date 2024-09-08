using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Queries.GetTimeZoneByUserId;

public record GetTimeZoneByUserIdQuery(Guid UserId) : IRequest<Result>;
