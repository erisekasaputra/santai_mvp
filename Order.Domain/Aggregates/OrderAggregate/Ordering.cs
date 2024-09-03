
using Order.Domain.Aggregates.BuyerAggregate;
using Order.Domain.Aggregates.MechanicAggregate;
using Order.Domain.Enumerations;
using Order.Domain.Events;
using Order.Domain.Exceptions;
using Order.Domain.SeedWork;
using Order.Domain.ValueObjects;

namespace Order.Domain.Aggregates.OrderAggregate;

public class Ordering : Entity
{ 
    public Address Address { get; private set; }
    public Buyer Buyer { get; private set; }
    public Mechanic? Mechanic { get; private set; } 
    public bool IsScheduled { get; private set; }
    public DateTime? ScheduledOn { get; private set; }
    public OrderStatus Status { get; private set; } 
    public ICollection<LineItem> LineItems { get; private set; }
    public ICollection<Fleet> Fleets { get; private set; }
    public bool IsPaid { get; private set; }
    public decimal GrandTotal => LineItems.Sum(item => item.TotalPrice);
    public Ordering(
        string addressLine,
        double latitude,
        double longitude,
        Guid buyerId,
        string buyerName,
        UserType buyerType,
        bool isScheduled,
        DateTime? scheduledOn)
    {
        Address = new Address(addressLine, latitude, longitude);  
        Buyer = new Buyer(buyerId, buyerName, buyerType);
        
        if (isScheduled && scheduledOn is null || scheduledOn <= DateTime.UtcNow)
        {
            throw new DomainException("Scheduled date can not in the past");
        }
        
        IsScheduled = isScheduled;
        ScheduledOn = scheduledOn;
        LineItems = [];
        Fleets = [];

        if (buyerType is UserType.BusinessUser || buyerType is UserType.StaffUser)
        {
            IsPaid = true;
        }
        else
        {
            IsPaid = false;
        }

        RaiseOrderCreatedDomainEvent();
    }

    public void AddOrderItem(LineItem orderItem)
    {
        LineItems ??= [];

        if (orderItem is null)
        { 
            throw new DomainException("Order item cannot be null.");
        }

        var items = LineItems.Where(x => x.Id == orderItem.Id).FirstOrDefault();

        if (items is not null)
        {
            items.AddQuantity(orderItem.Quantity);
            return;
        }

        LineItems.Add(orderItem); 
    }

    public void ApplyDiscountToOrder(Discount discount)
    {
        if (discount is null)
            throw new ArgumentNullException(nameof(discount), "Discount cannot be null.");

        foreach (var item in LineItems)
        {
            item.ApplyDiscount(discount);
        }
    }

    public void ApplyTaxToOrder(Tax tax)
    {
        if (tax is null)
            throw new ArgumentNullException(nameof(tax), "Tax cannot be null.");

        foreach (var item in LineItems)
        {
            item.ApplyTax(tax);
        }
    }

    public void AddFleet(Fleet fleet)
    {
        if (fleet is null)
            throw new ArgumentNullException(nameof(fleet), "Fleet cannot be null.");

        if (Fleets.Where(x => x.Id == fleet.Id).Any())
        {
            return;
        }

        Fleets.Add(fleet);
    }
    











    public void RaiseOrderCreatedDomainEvent()
    {
        AddDomainEvent(new OrderCreatedDomainEvent(this));
    }
}
