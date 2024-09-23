using Core.Enumerations;

namespace Ordering.API.Applications.Dtos.Requests;

public class OrderRequest
{
    public required string AddressLine { get; set; }
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
    public required Currency Currency { get; set; }
    public required bool IsScheduled { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public required string CouponCode { get; set; }
    public required IEnumerable<LineItemRequest> LineItems { get; set; }
    public required IEnumerable<FleetRequest> Fleets { get; set; }

    public OrderRequest(
        string addressLine,
        double latitude,
        double longitude,
        Currency currency,
        bool isScheduled,
        DateTime? scheduledAt,
        string couponCode,
        IEnumerable<LineItemRequest> lineItems,
        IEnumerable<FleetRequest> fleets)
    {
        AddressLine = addressLine;
        Latitude = latitude;
        Longitude = longitude;
        Currency = currency;
        IsScheduled = isScheduled;
        ScheduledAt = scheduledAt;
        CouponCode = couponCode;
        LineItems = lineItems;
        Fleets = fleets;
    }
}
