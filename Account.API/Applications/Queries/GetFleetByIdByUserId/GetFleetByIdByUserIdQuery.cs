using Core.Results; 
using MediatR;

namespace Account.API.Applications.Queries.GetFleetByIdByUserId;

public record GetFleetByIdByUserIdQuery(Guid UserId, Guid FleetId) : IRequest<Result>; 
