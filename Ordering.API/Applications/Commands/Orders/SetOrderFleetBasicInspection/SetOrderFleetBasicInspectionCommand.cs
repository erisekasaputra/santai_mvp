using Core.Results;
using MediatR;
using Ordering.API.Applications.Dtos.Requests;

namespace Ordering.API.Applications.Commands.Orders.SetOrderFleetBasicInspection;

public record SetOrderFleetBasicInspectionCommand(
    Guid OrderId, 
    Guid FleetId,
    IEnumerable<BasicInspectionRequest> BasicInspections) : IRequest<Result>;
