using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Queries.GetPaginatedRegularUser;

public record GetPaginatedRegularUserQuery(int PageNumber, int PageSize) : IRequest<Result>;
