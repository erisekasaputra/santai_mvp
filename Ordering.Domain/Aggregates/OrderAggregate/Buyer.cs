using Core.Enumerations;
using Ordering.Domain.SeedWork;

namespace Ordering.Domain.Aggregates.OrderAggregate;

public class Buyer : Entity
{
    public Guid OrderId { get; private set; }
    public Guid BuyerId { get; private set; }
    public Order Order { get; private set; }
    public string Name { get; private set; }
    public string ImageUrl { get; private set; }
    public string? Email { get; private set; }
    public string? PhoneNumber { get; private set; }
    public UserType BuyerType { get; private set; }
    public Buyer(
        Guid orderId,
        Guid buyerId,
        string name,
        string imageUrl,
        string? email,
        string? phoneNumber,
        UserType buyerType)
    {
        OrderId = orderId;
        BuyerId = buyerId;
        Name = name;
        ImageUrl = imageUrl;
        Email = email;
        PhoneNumber = phoneNumber;
        BuyerType = buyerType;
        Order = null!;
    }
}
