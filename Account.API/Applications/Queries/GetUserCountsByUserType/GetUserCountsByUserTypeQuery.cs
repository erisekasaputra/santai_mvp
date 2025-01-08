using Core.Results;
using MediatR;

namespace Account.API.Applications.Queries.GetUserCountsByUserType;

public record GetUserCountsByUserTypeQuery : IRequest<Result>;