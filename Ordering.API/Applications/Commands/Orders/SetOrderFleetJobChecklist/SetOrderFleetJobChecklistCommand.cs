using Core.Results;
using MediatR;
using Ordering.API.Applications.Dtos.Requests;

namespace Ordering.API.Applications.Commands.Orders.SetOrderFleetJobChecklist;

public record SetOrderFleetJobChecklistCommand(
    Guid OrderId,
    Guid FleetId,
    IEnumerable<JobChecklistRequest> JobChecklists) : IRequest<Result>;

