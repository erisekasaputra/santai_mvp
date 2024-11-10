using Core.Results;
using MediatR;

namespace Account.API.Applications.Queries.GetMechanicStatusExistence;

public record GetMechanicStatusExistenceQuery(Guid MechanicId) : IRequest<Result>;