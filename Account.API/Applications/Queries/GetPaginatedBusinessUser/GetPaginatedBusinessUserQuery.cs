using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Queries.GetPaginatedBusinessUser;

public record GetPaginatedBusinessUserQuery(int PageNumber, int PageSize) : IRequest<Result>;
