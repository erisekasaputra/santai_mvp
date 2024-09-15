using Core.Enumerations;

namespace Ordering.API.Applications.Dtos.Requests;

public class OrderRequest
{
    public string AddressLine { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public Currency Currency { get; set; }
    public bool IsScheduled { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public string CouponCode { get; set; }
    public IEnumerable<LineItemRequest> LineItems { get; set; }
    public IEnumerable<FleetRequest> Fleets { get; set; }

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
