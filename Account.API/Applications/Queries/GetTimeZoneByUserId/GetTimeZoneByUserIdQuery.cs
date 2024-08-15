using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Queries.GetTimeZoneByUserId;

public record GetTimeZoneByUserIdQuery(Guid UserId) : IRequest<Result>;
