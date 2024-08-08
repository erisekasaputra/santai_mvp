using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Queries.GetBusinessUserByUserId;

public record GetBusinessUserByUserIdQuery(Guid Id) : IRequest<Result>;
