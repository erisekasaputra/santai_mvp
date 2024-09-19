using Core.Results;
using MediatR;
using Ordering.API.Applications.Dtos.Requests;

namespace Ordering.API.Applications.Commands.Orders.SetOrderFleetPreServiceInspection;

public record SetOrderFleetPreServiceInspectionCommand(
    Guid OrderId,
    Guid FleetId,
    IEnumerable<PreServiceInspectionRequest> PreServiceInspections) : IRequest<Result>;
