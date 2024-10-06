using Core.Enumerations;

namespace Ordering.API.Applications.Dtos.Requests;

public class CalculateOrderRequest(
    string addressLine,
    double latitude,
    double longitude,
    Currency currency,
    bool isScheduled,
    DateTime? scheduledAt,
    string couponCode,
    IEnumerable<CalculateLineItemRequest> lineItems,
    IEnumerable<CalculateFleetRequest> fleets)
{
    public required string AddressLine { get; set; } = addressLine;
    public required double Latitude { get; set; } = latitude;
    public required double Longitude { get; set; } = longitude;
    public required Currency Currency { get; set; } = currency;
    public required bool IsScheduled { get; set; } = isScheduled;
    public DateTime? ScheduledAt { get; set; } = scheduledAt;
    public required string CouponCode { get; set; } = couponCode;
    public required IEnumerable<CalculateLineItemRequest> LineItems { get; set; } = lineItems;
    public required IEnumerable<CalculateFleetRequest> Fleets { get; set; } = fleets;
}
