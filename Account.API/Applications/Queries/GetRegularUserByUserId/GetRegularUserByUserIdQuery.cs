using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Queries.GetRegularUserByUserId;

public record GetRegularUserByUserIdQuery(Guid UserId) : IRequest<Result>;
