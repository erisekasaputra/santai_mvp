using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Queries.GetRegularUserByUserId;

public record GetRegularUserByUserIdQuery(Guid UserId) : IRequest<Result>;
