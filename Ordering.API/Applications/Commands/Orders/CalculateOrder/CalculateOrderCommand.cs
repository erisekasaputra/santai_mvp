using Core.Enumerations;
using Core.Results;
using MediatR;
using Ordering.API.Applications.Dtos.Requests;

namespace Ordering.API.Applications.Commands.Orders.CalculateOrder;

public record CalculateOrderCommand(
    Guid BuyerId,
    UserType BuyerType,
    string Address,
    double Latitude,
    double Longitude,
    Currency Currency,
    bool IsOrderScheduled,
    DateTime? ScheduledOn,
    string? CouponCode,
    IEnumerable<CalculateLineItemRequest> LineItems,
    IEnumerable<CalculateFleetRequest> Fleets) : IRequest<Result>;