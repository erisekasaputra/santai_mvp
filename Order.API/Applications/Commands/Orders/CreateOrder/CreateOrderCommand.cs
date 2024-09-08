
using Core.Enumerations;
using Core.Results;
using MediatR;
using Order.API.Applications.Dtos.Requests; 

namespace Order.API.Applications.Commands.Orders.CreateOrder;

public record CreateOrderCommand(
    Guid BuyerId,
    string Address,
    double Latitude,
    double Longitude,
    UserType UserType,
    bool IsOrderScheduled,
    DateTime? ScheduledOn,
    string CouponCode,
    IEnumerable<LineItemRequest> LineItems,
    IEnumerable<FleetRequest> Fleets) : IRequest<Result>;
