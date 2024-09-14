using Core.Enumerations;
using Core.Events.Models;
using MediatR;

namespace Core.Events;

public record OrderCreatedIntegrationEvent( 
    Guid OrderId,
    Guid BuyerId,
    string BuyerName,
    string BuyerAddressLine,
    double Latitude,
    double Longitude,
    IEnumerable<OrderLineItemEventModel> LineItems,
    IEnumerable<OrderFleetEventModel> Fleets,
    bool IsScheduled,
    DateTime? ScheduledOnUtc,
    decimal OrderAmount,
    Currency Currency,
    string? PaymentUrl,
    DateTime PaymentExpiration,
    decimal DiscountAmount,
    decimal GrandTotal) : INotification;
