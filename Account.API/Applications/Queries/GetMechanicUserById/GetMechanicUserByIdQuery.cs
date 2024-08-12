using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Queries.GetMechanicUserById;

public record GetMechanicUserByIdQuery(Guid Id) : IRequest<Result>; 
