using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Queries.GetFleetByIdByUserId;

public record GetFleetByIdByUserIdQuery(Guid UserId, Guid FleetId) : IRequest<Result>; 
