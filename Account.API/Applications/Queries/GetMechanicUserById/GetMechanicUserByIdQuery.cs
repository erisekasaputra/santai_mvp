using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Queries.GetMechanicUserById;

public record GetMechanicUserByIdQuery(Guid Id) : IRequest<Result>; 
