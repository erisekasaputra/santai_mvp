using Core.Enumerations;
using Core.Results;
using MediatR;

namespace Account.API.Applications.Queries.GetUserByUserTypeAndUserId;

public record GetUserByUserTypeAndUserIdQuery(Guid UserId, UserType UserType, IEnumerable<Guid>? Fleets) : IRequest<Result>;
