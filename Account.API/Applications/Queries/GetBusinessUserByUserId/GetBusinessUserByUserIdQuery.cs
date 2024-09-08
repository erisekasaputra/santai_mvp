using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Queries.GetBusinessUserByUserId;

public record GetBusinessUserByUserIdQuery(Guid Id) : IRequest<Result>;
