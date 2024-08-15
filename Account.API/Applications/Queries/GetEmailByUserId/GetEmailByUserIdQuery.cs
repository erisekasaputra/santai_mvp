using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Queries.GetEmailByUserId;

public record GetEmailByUserIdQuery(Guid UserId) : IRequest<Result>;
