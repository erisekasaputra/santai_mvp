using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Queries.GetPaginatedMechanicUser;

public record GetPaginatedMechanicUserQuery(int PageNumber, int PageSize) : IRequest<Result>; 
