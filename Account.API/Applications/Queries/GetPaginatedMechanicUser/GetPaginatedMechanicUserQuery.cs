using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Queries.GetPaginatedMechanicUser;

public record GetPaginatedMechanicUserQuery(int PageNumber, int PageSize) : IRequest<Result>; 
