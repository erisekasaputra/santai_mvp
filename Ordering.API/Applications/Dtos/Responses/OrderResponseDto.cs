using Core.Enumerations; 

namespace Ordering.API.Applications.Dtos.Responses;

public class OrderResponseDto
{ 
    public Guid OrderId { get; private set; }
    public string Secret { get; private set; }
    public string AddressLine { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public BuyerResponseDto Buyer { get; private set; }
    public MechanicResponseDto? Mechanic { get; private set; }
    public IEnumerable<LineItemResponseDto> LineItems { get; private set; }
    public IEnumerable<FleetResponseDto> Fleets { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public bool IsScheduled { get; private set; }
    public DateTime? ScheduledAtUtc { get; set; } 
    public PaymentResponseDto? Payment { get; private set; }
    public string? PaymentUrl { get; private set; }
    public DateTime PaymentExpiration { get; private set; }
    public Currency Currency { get; private set; }  
    public decimal OrderAmount { get; private set; }
    public DiscountResponseDto? Discount { get; private set; }
    public decimal GrandTotal { get; private set; }
    public RatingResponseDto? OrderRating { get; private set; }
    public IEnumerable<string>? RatingImages { get; private set; }
    public IEnumerable<FeeResponseDto> Fees { get; private set; }
    public CancellationResponseDto? Cancellation { get; private set; } 
    public bool IsPaid { get; private set; }
    public bool IsRated { get; private set; }
    public bool IsPaymentExpire { get; private set; }

    public OrderResponseDto(
        Guid orderId,
        string secret,
        string addressLine,
        double latitude,
        double longitude,
        BuyerResponseDto buyer,
        MechanicResponseDto? mechanic,
        IEnumerable<LineItemResponseDto> lineItems,
        IEnumerable<FleetResponseDto> fleets,
        DateTime createdAtUtc,
        bool isScheduled,
        DateTime? scheduledAt,
        PaymentResponseDto? payment,
        string? paymentUrl,
        DateTime paymentExpiration,
        Currency currency,
        decimal orderAmount,
        DiscountResponseDto? coupon,
        decimal grandTotal,
        RatingResponseDto? orderRating,
        IEnumerable<string>? ratingImages,
        IEnumerable<FeeResponseDto> fees,
        CancellationResponseDto? cancellation,
        bool isPaid,
        bool isRated,
        bool isPaymentExpire)
    {
        if (string.IsNullOrEmpty(secret)) throw new ArgumentNullException(nameof(secret));

        OrderId = orderId;
        Secret = secret;
        AddressLine = addressLine;
        Latitude = latitude;
        Longitude = longitude; 
        LineItems = lineItems;
        Buyer = buyer;
        Mechanic = mechanic;
        Fleets = fleets;
        CreatedAtUtc = createdAtUtc;
        Payment = payment;
        PaymentExpiration = paymentExpiration;
        Currency = currency;
        OrderAmount = orderAmount;
        Discount = coupon;
        GrandTotal = grandTotal;
        OrderRating = orderRating;
        RatingImages = ratingImages;
        Fees = fees;
        Cancellation = cancellation;
        IsPaid = isPaid;
        IsRated = isRated;
        IsScheduled = isScheduled;
        ScheduledAtUtc = scheduledAt;
        PaymentUrl = paymentUrl;
        IsPaymentExpire = isPaymentExpire;
    }
}
