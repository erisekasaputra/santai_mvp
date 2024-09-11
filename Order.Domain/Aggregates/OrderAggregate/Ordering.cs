using Core.Enumerations;
using Core.Exceptions;
using Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Order.Domain.Aggregates.BuyerAggregate;
using Order.Domain.Aggregates.MechanicAggregate;
using Order.Domain.Enumerations;
using Order.Domain.Events; 
using Order.Domain.SeedWork;
using Order.Domain.ValueObjects;

namespace Order.Domain.Aggregates.OrderAggregate;

public class Ordering : Entity
{  
    public Address Address { get; private set; } 
    public Buyer Buyer { get; private set; } 
    public Mechanic? Mechanic { get; private set; } 
    public DateTime? MechanicWaitingAcceptTime { get; private set; }
    public OrderStatus Status { get; private set; } 
    public ICollection<LineItem> LineItems { get; private set; }
    public ICollection<Fleet> Fleets { get; private set; } 
    public int TotalCanceledByMechanic { get; private set; } 
    public DateTime CreatedAtUtc { get; private init; }  
    public bool IsScheduled { get; private set; }
    public DateTime? ScheduledOnUtc { get; private set; }
    public Payment? Payment { get; private set; }
    public string? PaymentUrl { get; private set; }  
    public DateTime PaymentExpiration { get; private set; }
    public Currency Currency { get; private set; }
    public decimal OrderAmount { get; private set; }
    public Coupon? Coupon { get; private set; }
    public Money GrandTotal { get; private set; } 
    public Rating? Rating { get; private set; }
    public ICollection<string>? RatingImages { get; private set; }
    public ICollection<Fee> Fees { get; private set; } 
    public Cancellation? Cancellation { get; private set; } 
    public bool IsShouldRequestPayment 
    {
        get 
        {
            if (Buyer.BuyerType is UserType.BusinessUser or UserType.StaffUser)
            {
                return false;
            }

            return true;
        }   
        private init
        {
            if (value is true)
            {
                Status = OrderStatus.PaymentPending;
            }
            else
            {
                Status = OrderStatus.PaymentPaid;
            }
        }
    } 
    public bool IsPaid => Payment is not null && Payment.Amount.Amount > 0.00M; 
    public bool IsRated => Rating is not null && Rating.Value > 0.0M; 
    public bool IsPaymentExpire => PaymentExpiration < DateTime.UtcNow; 

    public Ordering()
    {
        Address = null!;
        Buyer = null!;
        LineItems = null!;
        Fleets = null!; 
        GrandTotal = null!;
        Fees = null!; 
    }

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

        Currency = currency;
        OrderAmount = 0;
        GrandTotal = new Money(0, currency); 
        Address = new Address(addressLine, latitude, longitude);
        Buyer = new Buyer(Id, buyerId, buyerName, buyerType);
        LineItems = [];
        Fleets = [];
        RatingImages = [];
        Fees = [];
        ScheduledOnUtc = null;

        IsScheduled = isScheduled; 
        if (isScheduled)
        {
            ScheduledOnUtc = scheduledOnUtc;
        }  

        IsShouldRequestPayment = !(buyerType is UserType.BusinessUser or UserType.StaffUser); 
        CreatedAtUtc = DateTime.UtcNow;
        PaymentExpiration = DateTime.UtcNow.AddHours(24);
        TotalCanceledByMechanic = 0;  
        RaiseOrderCreatedDomainEvent();

        if (!IsShouldRequestPayment)
        {
            RaiseOrderPaymentPaidDomainEvent();
        }
    }

    public void SetPaymentPaid(Payment payment)
    {
        bool updateState = true;

        if (Status is OrderStatus.OrderCancelledByUser)
        {
            throw new DomainException($"Order is already canceled by user");
        }

        if (IsShouldRequestPayment)
        {
            if (Status is not OrderStatus.PaymentPending || IsPaid)
            {
                throw new DomainException($"Could not set payment to {OrderStatus.PaymentPaid}");
            }
        }
        else
        {
            updateState = false;
        }


        if (payment.Amount.Amount != GrandTotal.Amount)
        {
            throw new DomainException("Paid amount is not equal with grand total amount");
        }

        if (payment.Amount.Currency != Currency)
        {
            throw new DomainException("Currency for payment is not equal with order currency");
        }

        if (Payment is not null && Payment.Amount.Amount > 0.00M)
        {
            throw new DomainException("Payment already paid");
        }

        Payment = payment;
        Payment.SetEntityState(EntityState.Added);

        if (IsScheduled && payment.CreatedAt >= ScheduledOnUtc) 
        {
            ScheduledOnUtc = payment.CreatedAt;
        }

        if (updateState)
        {
            Status = OrderStatus.PaymentPaid;
            RaiseOrderPaymentPaidDomainEvent(); 
        }  
    }

    public void SetPaymentUrl(string url)
    {
        if (Status != OrderStatus.PaymentPending)
        {
            throw new DomainException($"Status order must be in {OrderStatus.PaymentPending}");
        }
        
        PaymentUrl = url;   
    }
      
    public void ApplyFee(Fee fee)
    {   
        if (fee is null)
        {
            throw new DomainException("Fee can not be empty");
        }

        if (fee.PercentageOrValueType == PercentageOrValueType.Value)
        {
            if (fee.ValueAmount <= 0)
            {
                throw new DomainException("Fee amount can not be null");
            }

            if (fee.Currency != Currency)
            {
                throw new DomainException($"Fee currency {fee.Currency} does not match order currency ({Currency}).");
            } 
        }

        var isFeeExists = Fees.Where(x => x.FeeDescription == fee.FeeDescription).ToList().Count > 0;

        if (isFeeExists)
        {
            return;
        }

        Fees.Add(fee);
    } 

    public void CalculateGrandTotal()
    {
        if (GrandTotal.Currency != Currency)
        {
            throw new DomainException("Grand total currency with Order Amount currency is missmatch, incosistent state has occured");
        }

        if (LineItems is null || LineItems.Count == 0) 
        {
            throw new DomainException("Line items should not be empty");
        }

        var totalAmount = LineItems.Sum(lineItem => lineItem.CalculateTotalPrice().Amount);
        OrderAmount = totalAmount;

        if (OrderAmount <= 0)
        {
            throw new DomainException("Order amount can not less than or equal with 0");
        }

        GrandTotal.SetAmount(totalAmount, Currency); 
        GrandTotal -= Coupon?.Apply(totalAmount, Currency) ?? new Money(0, Currency);

        var holdedGrandTotal = GrandTotal;

        foreach (var fee in Fees)
        {
            GrandTotal += fee.Apply(holdedGrandTotal.Amount, Currency);
        }
    }

    public void AddOrderItem(LineItem lineItem)
    {
        LineItems ??= [];

        if (lineItem is null)
        {
            throw new DomainException("Line item can not be null");
        }

        if (lineItem.SubTotal.Currency != Currency)
        {
            throw new DomainException($"Item price currency ({lineItem.SubTotal.Currency}) does not match order currency ({Currency}).");
        } 

        var items = LineItems.Where(x => x.Id == lineItem.Id).FirstOrDefault();

        if (items is not null)
        {
            items.AddQuantity(lineItem.Quantity);
            return;
        }

        lineItem.SetEntityState(EntityState.Added);
        LineItems.Add(lineItem); 
    }

    public void ApplyDiscount(Coupon coupon)
    {
        if (coupon is null)
            throw new ArgumentNullException(nameof(coupon), "Coupon cannot be null.");

        if (coupon.CouponValueType == PercentageOrValueType.Value)
        {
            if (coupon.ValueAmount <= 0)
            {
                throw new DomainException("Coupon value can not be null");
            } 
        }

        if (coupon.Currency != Currency) 
        { 
            throw new DomainException($"Coupon currency does not match order currency ({Currency}).");
        }

        if (Coupon is not null)
        {
            return;
        }

        Coupon = coupon;
    }

    public void ApplyTax(Tax tax)
    {
        if (tax is null)
            throw new ArgumentNullException(nameof(tax), "Tax cannot be null."); 

        if (tax.Rate <= 0)
        {
            throw new DomainException("Tax can not be null"); 
        }

        if (tax.TaxAmount.Currency != Currency)
        {
            throw new DomainException($"Tax currency does not match order currency ({Currency}).");
        }

        foreach (var item in LineItems)
        {  
            item.ApplyTax(tax);
        }
    }

    public void AddFleet(Fleet fleet)
    {
        if (fleet.Id == Guid.Empty)
            throw new ArgumentNullException(nameof(fleet.Id), "Fleet id cannot be null.");

        ArgumentNullException.ThrowIfNullOrWhiteSpace(nameof(fleet.Brand), "Brand can not be empty");
        ArgumentNullException.ThrowIfNullOrWhiteSpace(nameof(fleet.Model), "Model can not be empty");
        ArgumentNullException.ThrowIfNullOrWhiteSpace(nameof(fleet.RegistrationNumber), "Registration number can not be empty");
        ArgumentNullException.ThrowIfNullOrWhiteSpace(nameof(fleet.ImageUrl), "Image url can not be empty");

        if (Fleets.Where(x => x.Id == fleet.Id).ToList().Count > 0)
        {
            return;
        }

        fleet.SetEntityState(EntityState.Added);
        Fleets.Add(fleet);
    }   


    private bool IsRejectionValidForMechanic(Guid mechanicId, out string errorMessage)
    {  
        if (mechanicId == Guid.Empty)
        {
            errorMessage = "Mechanic ID can not be empty";
            return false;
        }

        if (Status is OrderStatus.OrderCancelledByUser)
        {
            errorMessage = "Order is already canceled by user";
            return false;
        } 

        if (Mechanic is null)
        {
            errorMessage = "Mechanic has not been set";
            return false;
        }

        if (!Mechanic.MechanicId.Equals(mechanicId))
        {
            errorMessage = "The mechanic ID must match if the mechanic is initiating the cancellation.";
            return false;
        } 


        errorMessage = "Success";
        return true;
    }

    public void RejectOrderByMechanic(Guid mechanicId)
    { 
        if (Status is not OrderStatus.MechanicAssigned)
        {
            throw new DomainException($"Order status must be {OrderStatus.MechanicAssigned}");
        } 

        if (!IsRejectionValidForMechanic(mechanicId, out string errorMessage))
        {
            throw new DomainException(errorMessage);
        }

        if (MechanicWaitingAcceptTime is null)
        {
            throw new InvalidOperationException($"Mechanic waiting accept time is empty, inconsistent aggregate is occured");
        }

        if (MechanicWaitingAcceptTime < DateTime.UtcNow)
        {
            throw new DomainException("Can not accept the order, because the waiting time is expired");
        }
         
        Mechanic = null;
        Status = OrderStatus.OrderRejectedOrCancelledByMechanic;
        TotalCanceledByMechanic++;
        MechanicWaitingAcceptTime = null;

        RaiseOrderRejectedByMechanicDomainEvent(); 
    } 
  

    public void CancelByMechanic(Guid mechanicId)
    {
        if (Status is not OrderStatus.MechanicDispatched &&
            Status is not OrderStatus.MechanicArrived)
        {
            throw new DomainException($"Could not canceling order by mechanic when order status is {Status}"); 
        }

        if (!IsRejectionValidForMechanic(mechanicId, out string errorMessage))
        {
            throw new DomainException(errorMessage);
        }
         
        Mechanic = null;
        Status = OrderStatus.OrderRejectedOrCancelledByMechanic;
        TotalCanceledByMechanic++;
        MechanicWaitingAcceptTime = null;

        RaiseOrderCanceledByMechanicDomainEvent(); 
    }

    private bool IsCancelableByBuyer(Guid buyerId, out string errorMessage)
    {
        if (Status is OrderStatus.OrderCancelledByUser || Cancellation is not null)
        {
            errorMessage = "Order is already canceled by user";
            return false;
        }

        if (buyerId == Guid.Empty || Buyer.BuyerId == Guid.Empty)
        {
            errorMessage = "Buyer ID can not be empty";
            return false;
        }

        if (!Buyer.BuyerId.Equals(buyerId))
        {
            errorMessage = "Order cancellation is not allowed: The user ID associated with the order does not match the user ID of the requester.";
            return false;
        }

        if (Status is OrderStatus.ServiceCompleted or OrderStatus.ServiceIncompleted)
        {
            errorMessage = $"Can not canceling the order when the status is {Status}";
            return false;
        }

        errorMessage = "Success";
        return true;
    }

    private bool IsChargeableCancellation()
    {
        if (Status is OrderStatus.MechanicAcceptedOrder or OrderStatus.MechanicDispatched or OrderStatus.MechanicArrived or OrderStatus.MechanicArrived or OrderStatus.ServiceInProgress or OrderStatus.ServiceCompleted or OrderStatus.ServiceIncompleted)
        {
            return true;
        }

        return false;
    }

    private bool IsRefundableCancellation()
    {
        // if business user get no refund, otherwise will get refund
        if (Buyer.BuyerType is UserType.BusinessUser or UserType.StaffUser)
        {
            return false;
        }

        // if not already paid, dont give refund
        if (!IsPaid)
        {
            return false;
        }

        return true;
    }


    public void CancelByBuyer(Guid buyerId)
    {  
        if (!IsCancelableByBuyer(buyerId, out string errorMessage))
        {
            throw new DomainException(errorMessage);
        }

        Cancellation = new Cancellation(Id);
        Cancellation.SetEntityState(EntityState.Added);

        if (IsChargeableCancellation())
        {
            Cancellation.ApplyCancellationCharge(
                Fees.Where(x => ChargeCancellation.Charges.Contains(x.FeeDescription)).ToArray());
        }
         
        if (IsRefundableCancellation())
        {
            Cancellation.ApplyCancellationRefund(GrandTotal);
        } 

        Status = OrderStatus.OrderCancelledByUser; 
        RaiseOrderCanceledByBuyerDomainEvent();  
    }

    public void SetServiceInProgress(Guid mechanicId, Guid buyerId, Guid fleetId)
    {
        if (Status is OrderStatus.OrderCancelledByUser)
        {
            throw new DomainException("Order is already canceled by user");
        }

        if (Status is not OrderStatus.MechanicArrived)
        {
            throw new DomainException($"Order status must be {OrderStatus.MechanicArrived}");
        } 

        if (mechanicId ==  Guid.Empty)
        {
            throw new DomainException("Mechanic ID must no be empty");
        }

        if (buyerId == Guid.Empty)
        {
            throw new DomainException("Buyer ID must no be empty");
        }

        if (fleetId == Guid.Empty)
        {
            throw new DomainException("Fleet ID must not be empty");
        }

        var fleetExists = Fleets.Where(x => x.Id == fleetId).ToList().Count > 0;

        if (!fleetExists) 
        {
            throw new DomainException("Fleet ID does not exists");
        }

        if (Mechanic is null)
        {
            throw new DomainException("Mechanic has not been set");
        }

        if (!Mechanic.MechanicId.Equals(mechanicId))
        { 
            throw new DomainException("Mechanic ID is missmatch");
        }
        
        if (!Buyer.BuyerId.Equals(buyerId))
        {
            throw new DomainException("Buyer ID is missmatch");
        }

        Status = OrderStatus.ServiceInProgress;

        RaiseServiceProcessedDomainEvent();
    } 

    public void SetServiceCompleted(Guid mechanicId, Guid buyerId, Guid fleetId)
    {
        if (Status is OrderStatus.OrderCancelledByUser)
        {
            throw new DomainException("Order is already canceled by user");
        }

        if (Status is not OrderStatus.ServiceInProgress)
        {
            throw new DomainException($"Order status must be {OrderStatus.ServiceInProgress}");
        }

        if (mechanicId == Guid.Empty)
        {
            throw new DomainException("Mechanic ID must no be empty");
        }

        if (buyerId == Guid.Empty)
        {
            throw new DomainException("Buyer ID must no be empty");
        }

        if (fleetId == Guid.Empty)
        {
            throw new DomainException("Fleet ID must not be empty");
        }

        var fleetExists = Fleets.Where(x => x.Id == fleetId).ToList().Count > 0;

        if (!fleetExists)
        {
            throw new DomainException("Fleet ID does not exists");
        }

        if (Mechanic is null)
        {
            throw new DomainException("Mechanic has not been set");
        }

        if (!Mechanic.MechanicId.Equals(mechanicId))
        {
            throw new DomainException("Mechanic ID is missmatch");
        }

        if (!Buyer.BuyerId.Equals(buyerId))
        {
            throw new DomainException("Buyer ID is missmatch");
        }

        Status = OrderStatus.ServiceCompleted; 
        RaiseServiceCompletedDomainEvent();
    } 

    public void SetServiceIncompleted(Guid mechanicId, Guid buyerId, Guid fleetId)
    {
        if (Status is OrderStatus.OrderCancelledByUser)
        {
            throw new DomainException("Order is already canceled by user");
        }

        if (Status is not OrderStatus.ServiceInProgress)
        {
            throw new DomainException($"Order status must be {OrderStatus.ServiceInProgress}");
        }

        if (mechanicId == Guid.Empty)
        {
            throw new DomainException("Mechanic ID must no be empty");
        }

        if (buyerId == Guid.Empty)
        {
            throw new DomainException("Buyer ID must no be empty");
        }

        if (fleetId == Guid.Empty)
        {
            throw new DomainException("Fleet ID must not be empty");
        }

        var fleetExists = Fleets.Where(x => x.Id == fleetId).ToList().Count > 0;

        if (!fleetExists)
        {
            throw new DomainException("Fleet ID does not exists");
        }

        if (Mechanic is null)
        {
            throw new DomainException("Mechanic has not been set");
        }

        if (!Mechanic.MechanicId.Equals(mechanicId))
        {
            throw new DomainException("Mechanic ID is missmatch");
        }

        if (!Buyer.BuyerId.Equals(buyerId))
        {
            throw new DomainException("Buyer ID is missmatch");
        }

        Status = OrderStatus.ServiceIncompleted; 
        RaiseServiceIncompletedDomainEvent();
    }
     
  
    public void SetFindingMechanic()
    { 
        if (Status is OrderStatus.OrderCancelledByUser)
        {
            throw new DomainException($"Order is already canceled by user");
        } 

        if (Status is not OrderStatus.PaymentPaid && Status is not OrderStatus.OrderRejectedOrCancelledByMechanic)
        {
            throw new DomainException($"Could not set order status to finding mechanic");
        }

        if (Mechanic is not null)
        {
            throw new DomainException("Mechanic is already assigned, mechanic need to be reset");
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
        RatingImages = images?.ToList(); 
        RaiseOrderRatedDomainEvent();
    }
       
    private bool IsOrderMechanicAssignable(Guid mechanicId, out string errorMessage)
    {
        if (Status is OrderStatus.OrderCancelledByUser)
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

        if (Mechanic is not null) 
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
        if (mechanicId == Guid.Empty)
        {
            throw new DomainException("Mechanic id can not be empty");
        }

        if (Status is OrderStatus.OrderCancelledByUser)
        { 
            throw new DomainException("Order is already canceled by user"); 
        }

        if (Status is not OrderStatus.MechanicAssigned) 
        {
            throw new DomainException($"Order status must be {OrderStatus.MechanicAssigned}");
        }

        if (Mechanic is null)
        {
            throw new DomainException("Mechanic has not been set");
        }

        if (Mechanic.MechanicId != mechanicId)
        {
            throw new DomainException($"Mechanic ID is missmatch");
        }

        if (MechanicWaitingAcceptTime is null)
        {  
            throw new InvalidOperationException($"Mechanic waiting accept time is empty, inconsistent aggregate is occured");
        }

        if (MechanicWaitingAcceptTime < DateTime.UtcNow)
        {
            throw new DomainException("Can not accept the order, because the waiting time is expired");
        }

        Status = OrderStatus.MechanicAcceptedOrder; 
        RaiseOrderAcceptedByMechanicDomainEvent();
    }

   
    public void AssignMechanic(
        Mechanic mechanic, 
        int mechanicWaitingAcceptTimeInSeconds = 120)
    {
        if (mechanic is null)
        {
            throw new DomainException("Mechanic can not be null");
        }

        if (!IsOrderMechanicAssignable(mechanic.Id, out string errorMessage))
        {
            throw new DomainException(errorMessage);
        }

        MechanicWaitingAcceptTime = DateTime.UtcNow.AddSeconds(mechanicWaitingAcceptTimeInSeconds); 
        Status = OrderStatus.MechanicAssigned; 
        Mechanic = mechanic;
        Mechanic.SetEntityState(EntityState.Added); 
        RaiseMechanicAssignedDomainEvent();
    }

    public void SetDispatchMechanic(Guid mechanicId)
    {
        if (Status is OrderStatus.OrderCancelledByUser)
        { 
            throw new DomainException("Order is already canceled by user");
        }

        if (Status is not OrderStatus.MechanicAcceptedOrder)
        {
            throw new DomainException($"Order status must be {OrderStatus.MechanicAcceptedOrder}");
        }

        if (Mechanic is null)
        {
            throw new DomainException("Mechanic has not been set");
        }

        if (Mechanic.MechanicId != mechanicId)
        {
            throw new DomainException($"Mechanic ID is missmatch");
        }

        Status = OrderStatus.MechanicDispatched;

        RaiseMechanicDispatchedDomainEvent();
    }

    public void SetArrivedMechanic(Guid mechanicId)
    {
        if (Status is OrderStatus.OrderCancelledByUser)
        {
            throw new DomainException("Order is already canceled by user");
        }

        if (Status is not OrderStatus.MechanicDispatched)
        {
            throw new DomainException($"Order status must be {OrderStatus.MechanicDispatched}");
        }

        if (Mechanic is null)
        {
            throw new DomainException("Mechanic has not been set");
        }

        if (Mechanic.MechanicId != mechanicId)
        {
            throw new DomainException($"Mechanic ID is missmatch");
        }

        Status = OrderStatus.MechanicArrived; 
        RaiseMechanicArrivedDomainEvent();
    }

    private void RaiseServiceCompletedDomainEvent()
    {
    }

    private void RaiseServiceIncompletedDomainEvent()
    {
    }


    private void RaiseServiceProcessedDomainEvent()
    { 
    }

    private void RaiseMechanicArrivedDomainEvent()
    {
    }

    private void RaiseMechanicDispatchedDomainEvent()
    {
    }

    private void RaiseOrderRatedDomainEvent()
    {
    }

    private void RaiseOrderAcceptedByMechanicDomainEvent()
    {
    }

    private void RaiseOrderCanceledByBuyerDomainEvent()
    {
    }

    private void RaiseMechanicAssignedDomainEvent()
    {
    }

    private void RaiseOrderCanceledByMechanicDomainEvent()
    {
    } 

    private void RaiseOrderPaymentPaidDomainEvent()
    {
        AddDomainEvent(new OrderPaymentPaidDomainEvent(this));
    }

    private void RaiseOrderFindingMechanicDomainEvent()
    {
        AddDomainEvent(new OrderFindingMechanicDomainEvent(this));
    }
    public void RaiseOrderCreatedDomainEvent()
    {
        AddDomainEvent(new OrderCreatedDomainEvent(this));
    }
    private void RaiseOrderRejectedByMechanicDomainEvent()
    {
        AddDomainEvent(new OrderRejectedByMechanicDomainEvent(this));
    }
}
