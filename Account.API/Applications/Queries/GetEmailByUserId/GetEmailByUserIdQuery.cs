using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Queries.GetEmailByUserId;

public record GetEmailByUserIdQuery(Guid UserId) : IRequest<Result>;
