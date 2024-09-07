using MediatR;
using Order.API.Applications.Dtos.Requests;
using Order.API.SeedWorks;
using Order.Domain.Enumerations;

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
