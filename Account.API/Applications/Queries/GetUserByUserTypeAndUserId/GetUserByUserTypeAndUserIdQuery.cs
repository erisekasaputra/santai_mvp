using Core.Results;
using MediatR;

namespace Account.API.Applications.Queries.GetUserByUserTypeAndUserId;

public record GetUserByUserTypeAndUserIdQuery(Guid UserId, IEnumerable<Guid>? Fleets) : IRequest<Result>;
