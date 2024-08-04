using Account.API.SeedWork;
using Account.Domain.Aggregates.UserAggregate; 
using MediatR;

namespace Account.API.Applications.Commands;

public record CreateBusinessUserCommand(
        
    ) : IRequest<Result<User>>;
