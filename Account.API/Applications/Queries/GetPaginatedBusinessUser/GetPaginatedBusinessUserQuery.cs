using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Queries.GetPaginatedBusinessUser;

public record GetPaginatedBusinessUserQuery(int PageNumber, int PageSize) : IRequest<Result>;
