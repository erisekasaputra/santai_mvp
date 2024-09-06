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
    public Guid BuyerId { get; private set; }
    public Buyer Buyer { get; private set; }
    public Guid? MechanicId { get; private set; }
    public Mechanic? Mechanic { get; private set; } 
    public DateTime? MechanicWaitingAcceptTime { get; private set; }
    public bool IsScheduled { get; private set; }
    public DateTime? ScheduledOnUtc { get; private set; }
    public OrderStatus Status { get; private set; } 
    public ICollection<LineItem> LineItems { get; private set; }
    public ICollection<Fleet> Fleets { get; private set; } 
    public int TotalCanceledByMechanic { get; private set; } 
    public DateTime CreatedAtUtc { get; private init; }  
    public Payment? Payment { get; private set; }
    public Money OrderAmount { get; private set; }
    public Money GrandTotal { get; private set; }
    public bool IsPaid { get; private set; }
    public Rating? Rating { get; private set; }
    public bool IsRated { get; private set; }
    public ICollection<string>? RatingImages { get; private set; }
    public ICollection<Fee> Fees { get; private set; }
    public ICollection<Fee>? Charges { get; private set; }

    public Ordering(
        Currency currency,
        string addressLine,
        double latitude,
        double longitude,
        Guid buyerId,
        string buyerName,
        UserType buyerType,
        bool isScheduled,
        DateTime? scheduledOnUtc)
    { 
        if (isScheduled && scheduledOnUtc is null || scheduledOnUtc <= DateTime.UtcNow)
        {
            throw new DomainException("Scheduled date can not in the past and can not be null");
        }

        OrderAmount = new Money(0, currency);
        GrandTotal = new Money(0, currency);
        Address = new Address(addressLine, latitude, longitude);
        Buyer = new Buyer(buyerId, buyerName, buyerType);
        LineItems = [];
        Fleets = [];
        RatingImages = [];
        Fees = [];
        Charges = [];

        IsScheduled = isScheduled; 
        if (isScheduled)
        {
            ScheduledOnUtc = scheduledOnUtc;
        }
        else
        {
            ScheduledOnUtc = null;
        }

        if (buyerType is UserType.BusinessUser || buyerType is UserType.StaffUser)
        {
            IsPaid = true;
            Status = OrderStatus.PaymentPaid;
        }
        else
        {
            IsPaid = false;
            Status = OrderStatus.PaymentPending; 
        }
         

        CreatedAtUtc = DateTime.UtcNow;
        TotalCanceledByMechanic = 0; 
        IsRated = false;
        RaiseOrderCreatedDomainEvent();
    }

    public void CalculateOrderAmount()
    { 
        var totalAmount = LineItems.Sum(lineItem => lineItem.CalculateTotalPrice().Amount); 
        OrderAmount = new Money(totalAmount, OrderAmount.Currency);
    }
     
    public void AddFeeByValue(FeeDescription feeDescription, decimal amount, Currency currency)
    {
        if (OrderAmount.Currency != currency)
        { 
            throw new DomainException($"Fee currency ({currency}) does not match order currency ({OrderAmount.Currency}).");
        }

        if (amount <= 0)
        {
            throw new DomainException("Fee amount can not less than or equal with 0");
        }

        if (Fees.Select(x => x.FeeDescription == feeDescription).Any()) 
        {
            return;
        }

        Fees.Add(Fee.CreateByValue(feeDescription, amount, currency));
    }


    public void AddFeeByPercentage(FeeDescription feeDescription, decimal amount)
    { 
        if (amount <= 0)
        {
            throw new DomainException("Fee percentage can not less than or equal with 0");
        }

        if (amount < 1 && amount > 100)
        {
            throw new DomainException("Fee percentage must be between 1 and 100");
        }  

        if (OrderAmount.Amount <= 0)
        {
            CalculateOrderAmount(); 
        }

        if (OrderAmount.Amount <=0)
        {
            throw new DomainException("There is no order item you have made");
        }

        if (Fees.Select(x => x.FeeDescription == feeDescription).Any())
        {
            return;
        }

        Fees.Add(Fee.CreateByPercentage(feeDescription, amount, OrderAmount));
    }

    public void CalculateGrandTotal()
    { 
        if (OrderAmount.Amount <= 0)
        {
            throw new DomainException("Order amount can not less than or equal with 0");
        }

        GrandTotal += OrderAmount;

        foreach (var fe in Fees)
        {
            GrandTotal += fe.Amount;
        }
    }

    public void AddOrderItem(Guid id, string name, string sku, decimal price, Currency currency, int quantity)
    {
        LineItems ??= [];

        if (currency != OrderAmount.Currency)
        {
            throw new DomainException($"Item price currency ({currency}) does not match order currency ({OrderAmount.Currency}).");
        }

        var lineItemPrice = new Money(price, OrderAmount.Currency); 

        var orderItem = new LineItem(id, name, sku, lineItemPrice, quantity) ?? throw new DomainException("Order item cannot be null.");

        var items = LineItems.Where(x => x.Id == orderItem.Id).FirstOrDefault();

        if (items is not null)
        {
            items.AddQuantity(orderItem.Quantity);
            return;
        }

        LineItems.Add(orderItem); 
    }

    public void ApplyDiscount(Coupon coupon)
    {
        if (coupon is null)
            throw new ArgumentNullException(nameof(coupon), "Coupon cannot be null."); 

        if (coupon.CouponValueType == PercentageOrValueType.Value)
        {
            if (coupon.Value is null)
            {
                throw new DomainException($"Coupon value can not be empty.");
            }

            if (coupon.Value.Currency != OrderAmount.Currency)
            {
                throw new DomainException($"Coupon currency ({coupon.Value?.Currency}) does not match line order currency ({OrderAmount.Currency}).");
            }
        }  

        foreach (var item in LineItems)
        {
            item.ApplyDiscount(coupon);
        }
    }

    public void ApplyTax(Tax tax)
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


    public void RemoveFleet(Fleet fleet) 
    {
        Fleets ??= [];

        if (fleet is null)
            throw new ArgumentNullException(nameof(fleet), "Fleet cannot be null.");

        Fleets.Remove(fleet);
    }
     
     
    private bool IsCancelableByMechanic(Guid mechanicId, out string errorMessage)
    {
        if (Status is not OrderStatus.MechanicAssigned &&
            Status is not OrderStatus.MechanicDispatched &&
            Status is not OrderStatus.MechanicArrived)
        {
            errorMessage = $"Could not canceling order by mechanic when order status is {Status}";
            return false;
        }

        if (Status is OrderStatus.OrderCanceledByUser)
        {
            errorMessage = "Order is already canceled by user";
            return false;
        }

        if (mechanicId == Guid.Empty)
        {
            errorMessage = "Mechanic ID can not be empty";
            return false;
        }

        if (MechanicId is null || !MechanicId.Equals(mechanicId))
        {
            errorMessage = "The mechanic ID must match if the mechanic is initiating the cancellation.";
            return false;
        } 

        errorMessage = "Success";
        return true;
    }


    public void CancelByMechanic(Guid mechanicId)
    { 
        if (!IsCancelableByMechanic(mechanicId, out string errorMessage))
        {
            throw new DomainException(errorMessage);
        }

        MechanicId = null;
        Mechanic = null; 
        Status = OrderStatus.FindingMechanic; 
        TotalCanceledByMechanic++;
        MechanicWaitingAcceptTime = null;

        RaiseOrderCanceledByMechanicDomainEvent();
    } 

    private bool IsCancelableByBuyer(Guid buyerId, out string errorMessage) 
    {
        if (buyerId == Guid.Empty || BuyerId == Guid.Empty) 
        {
            errorMessage = "Buyer ID can not be empty";
            return false;
        }
         
        if (!BuyerId.Equals(buyerId)) 
        {
            errorMessage = "Order cancellation is not allowed: The user ID associated with the order does not match the user ID of the requester.";
            return false;
        } 

        if (Status is OrderStatus.ServiceInProgress or OrderStatus.ServiceCompleted or OrderStatus.ServiceIncompleted)
        {
            errorMessage = $"Can not canceling the order when the status is {Status}";
            return false;
        } 

        if (Status is OrderStatus.OrderCanceledByUser)
        {
            errorMessage = "Order is already canceled by user";
            return false;
        } 

        errorMessage = "Success";
        return true;
    }

    // belum clear ----------------------------------------------------------------------------------------------

    public void ProcessService()
    {

    }

    public void ProcessInspection()
    {

    }
     
    
    public void CancelByBuyer(Guid buyerId)
    { 
        if (!IsCancelableByBuyer(buyerId, out string errorMessage))
        {
            throw new DomainException(errorMessage);
        }

        if (!IsPaid)
        {
            Status = OrderStatus.OrderCanceledByUser;
            RaiseOrderCanceledByBuyerWithNoRefundDomainEvent();
            return;
        }

        if (IsPaid && (Buyer.BuyerType == UserType.BusinessUser || Buyer.BuyerType == UserType.StaffUser))
        { 
            Status = OrderStatus.OrderCanceledByUser;
            RaiseOrderCanceledByBuyerWithNoRefundDomainEvent();
            return;
        }
 
        if (IsPaid && Status is OrderStatus.PaymentPending or OrderStatus.PaymentPaid or OrderStatus.FindingMechanic)
        {

        }

        Status = OrderStatus.OrderCanceledByUser;
        RaiseOrderCanceledByBuyerWithRefundDomainEvent();
        return; 
    }  

    public void SetFindingMechanic()
    { 
        if (Status is OrderStatus.OrderCanceledByUser)
        {
            throw new DomainException($"Order is already canceled by user");
        } 

        if (Status is not OrderStatus.PaymentPaid)
        {
            throw new DomainException($"Could not set to finding mechanic, Order status has to be {OrderStatus.PaymentPaid}");
        }

        Status = OrderStatus.FindingMechanic;

        RaiseOrderFindingMechanicDomainEvent();
    }

    public void SetOrderRating(decimal rating, string comment, IEnumerable<string>? images)
    {
        if (Status is not OrderStatus.ServiceCompleted && Status is not OrderStatus.ServiceIncompleted)
        {
            throw new DomainException($"Set order rating only available when the order has completed");
        }

        if (IsRated) 
        {
            throw new DomainException($"Can not set order rating twice");
        }

        if (rating < 1 && rating > 5) 
        {
            throw new DomainException("Rating must be between 1 and 5");
        } 

        Rating = new Rating(rating, comment);
        IsRated = true;
        RatingImages = images?.ToList();

        RaiseOrderRatedDomainEvent();
    }

  

    public void SetPaymentPaid(
        decimal amount,
        Currency currency,
        DateTime transactionAt,
        string? paymentMethod,
        string? bankReference)
    {
        if (Status is OrderStatus.OrderCanceledByUser)
        {
            throw new DomainException($"Order is already canceled by user");
        } 

        if (Status is not OrderStatus.PaymentPending || IsPaid)
        { 
            throw new DomainException($"Could not set payment to {OrderStatus.PaymentPending}");
        }

        Payment = new Payment(
            amount,
            currency,
            transactionAt,
            paymentMethod,
            bankReference);

        Status = OrderStatus.PaymentPaid; 
        IsPaid = true; 
        RaiseOrderPaymentPaidDomainEvent();
    }

    private bool IsOrderMechanicAssignable(Guid mechanicId, out string errorMessage)
    {
        if (Status is OrderStatus.OrderCanceledByUser)
        { 
            errorMessage = "Order is already canceled by user";
            return false;
        } 

        if (Status is not OrderStatus.FindingMechanic)
        {
            errorMessage = $"Order status must be {OrderStatus.FindingMechanic}";
            return false;
        } 

        if (IsScheduled && DateTime.UtcNow < ScheduledOnUtc)
        {
            string? formattedDate = ScheduledOnUtc?.ToString("dddd, MMMM d, yyyy, h:mm tt");
            errorMessage = $"The mechanic can only be assigned on or after {formattedDate} UTC";
            return false;
        }

        if (MechanicId.HasValue && MechanicId != Guid.Empty && MechanicId is not null) 
        {
            errorMessage = "Mechanic is already assigned";
            return false;
        }

        if (mechanicId == Guid.Empty)
        {
            errorMessage = "Mechanic id can not be empty";
            return false;
        }

        errorMessage = "Success";
        return true;
    }

    public void AcceptOrderByMechanic(Guid mechanicId)
    { 
        if (Status is not OrderStatus.MechanicAssigned) 
        {
            throw new DomainException($"Order status must be {OrderStatus.MechanicAssigned}");
        }

        if (MechanicId != mechanicId)
        {
            throw new DomainException($"Mechanic ID is missmatch");
        }

        if (MechanicWaitingAcceptTime is null)
        {  
            throw new InvalidOperationException($"MechanicWaitingAcceptTime is empty, inconsistent aggregate is occured");
        }

        if (MechanicWaitingAcceptTime < DateTime.UtcNow)
        {
            throw new DomainException("Can not accept the order, because the waiting time is expired");
        }

        Status = OrderStatus.MechanicAcceptedOrder; 
        RaiseOrderAcceptedByMechanicDomainEvent();
    }

   
    public void AssignMechanic(Guid mechanicId, string name, decimal performance, int mechanicWaitingAcceptTimeInSeconds = 60)
    {
        if (!IsOrderMechanicAssignable(mechanicId, out string errorMessage))
        {
            throw new DomainException(errorMessage);
        }

        MechanicWaitingAcceptTime = DateTime.UtcNow.AddSeconds(mechanicWaitingAcceptTimeInSeconds); 
        Status = OrderStatus.MechanicAssigned;
        MechanicId = mechanicId;
        Mechanic = new Mechanic(mechanicId, name, performance);

        RaiseMechanicAssignedDomainEvent();
    }

    public void SetDispatchMechanic(Guid mechanicId)
    {
        if (Status is OrderStatus.OrderCanceledByUser)
        { 
            throw new DomainException("Order is already canceled by user");
        }

        if (Status is not OrderStatus.MechanicAcceptedOrder)
        {
            throw new DomainException($"Order status must be {OrderStatus.MechanicAcceptedOrder}");
        }

        if (MechanicId != mechanicId)
        {
            throw new DomainException($"Mechanic ID is missmatch");
        }

        Status = OrderStatus.MechanicDispatched;

        RaiseMechanicDispatchedDomainEvent();
    }

    public void SetArrivedMechanic(Guid mechanicId)
    {
        if (Status is OrderStatus.OrderCanceledByUser)
        {
            throw new DomainException("Order is already canceled by user");
        }

        if (Status is not OrderStatus.MechanicDispatched)
        {
            throw new DomainException($"Order status must be {OrderStatus.MechanicDispatched}");
        }

        if (MechanicId != mechanicId)
        {
            throw new DomainException($"Mechanic ID is missmatch");
        }

        Status = OrderStatus.MechanicArrived;

        RaiseMechanicArrivedDomainEvent();
    }

    private void RaiseMechanicArrivedDomainEvent()
    {
        throw new NotImplementedException();
    }

    private void RaiseMechanicDispatchedDomainEvent()
    {
        throw new NotImplementedException();
    }

    private void RaiseOrderRatedDomainEvent()
    {
        throw new NotImplementedException();
    }

    private void RaiseOrderAcceptedByMechanicDomainEvent()
    {
        throw new NotImplementedException();
    }

    private void RaiseOrderCanceledByBuyerWithRefundDomainEvent()
    {
        throw new NotImplementedException();
    }

    private void RaiseMechanicAssignedDomainEvent()
    {
        throw new NotImplementedException();
    }

    private void RaiseOrderCanceledByMechanicDomainEvent()
    {
        throw new NotImplementedException();
    }
    private void RaiseOrderCanceledByBuyerWithNoRefundDomainEvent()
    {
        throw new NotImplementedException();
    }
    private void RaiseOrderPaymentPaidDomainEvent()
    {
        throw new NotImplementedException();
    }

    private void RaiseOrderFindingMechanicDomainEvent()
    {
        throw new NotImplementedException();
    }
    public void RaiseOrderCreatedDomainEvent()
    {
        AddDomainEvent(new OrderCreatedDomainEvent(this));
    }
}
