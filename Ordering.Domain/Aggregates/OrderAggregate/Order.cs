using Core.Enumerations;
using Core.Exceptions;
using Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Aggregates.FleetAggregate;
using Ordering.Domain.Enumerations;
using Ordering.Domain.Events;
using Ordering.Domain.SeedWork;
using Ordering.Domain.ValueObjects;

namespace Ordering.Domain.Aggregates.OrderAggregate;

public class Order : Entity
{
    public string Secret { get; set; }
    public Address Address { get; private init; }
    public Buyer Buyer { get; private init; }
    public Mechanic? Mechanic { get; private set; } 
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
    public Discount? Discount { get; private set; }
    public Money GrandTotal { get; private set; }
    public Rating? Rating { get; private set; }
    public ICollection<string>? RatingImages { get; private set; }
    public ICollection<Fee> Fees { get; private set; }
    public Cancellation? Cancellation { get; private set; }
    public bool IsShouldRequestPayment
    {
        get
        {
            if (Buyer is null)
            {
                throw new DomainException("Buyer has not been set", "Buyer");
            }

            if (Buyer.BuyerType is UserType.BusinessUser or UserType.StaffUser)
            {
                return false;
            }

            return true;
        } 
    }

    public bool IsPaid => (IsShouldRequestPayment && Payment is not null && Payment.Amount.Amount > 0.00M || !IsShouldRequestPayment);
    public bool IsRated => Rating is not null && Rating.Value > 0.0M;
    public bool IsPaymentExpire => PaymentExpiration < DateTime.UtcNow;

    public Order()
    {
        Secret = string.Empty;
        Address = null!;
        Buyer = null!;
        LineItems = null!;
        Fleets = null!;
        GrandTotal = null!;
        Fees = null!;
    }

    public Order( 
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
        Secret = string.Empty;
        if (isScheduled && scheduledOnUtc is null || scheduledOnUtc <= DateTime.UtcNow)
        {
            if (scheduledOnUtc is null || !scheduledOnUtc.HasValue)
            {
                throw new DomainException("Schedule date can not be empty", "ScheduledDate");
            }

            if (scheduledOnUtc <= DateTime.UtcNow) 
            {
                throw new InvalidDateOperationException(
                    "Schedule date can not in the past",
                    scheduledOnUtc.Value);
            } 
        }

        Currency = currency;
        OrderAmount = 0;
        GrandTotal = new Money(0, currency);
        Address = new Address(addressLine, latitude, longitude);
        Buyer = new Buyer(Id, buyerId, buyerName, buyerType);
        Mechanic = null;
        Payment = null;
        LineItems = [];
        Fleets = [];
        RatingImages = [];
        Fees = [];
        ScheduledOnUtc = null;
        Discount = null;
        Rating = null;

        IsScheduled = isScheduled;
        if (isScheduled) ScheduledOnUtc = scheduledOnUtc; 
         
        CreatedAtUtc = DateTime.UtcNow;
        PaymentExpiration = DateTime.UtcNow.AddHours(24);
        TotalCanceledByMechanic = 0;
        RaiseOrderCreatedDomainEvent();

        if (!IsShouldRequestPayment)
        { 
            Status = OrderStatus.PaymentPaid; 
        }
        else
        {
            Status = OrderStatus.PaymentPending;
        }

        if (!IsScheduled && !IsShouldRequestPayment)
        {  
            Status = OrderStatus.FindingMechanic;    
            RaiseOrderFindingMechanicDomainEvent(Id, buyerId, latitude, longitude);
        }
    }

    public void SetSecretKey(string secret)
    {
        Secret = secret;
    }

    public void SetPaymentPaid(Payment payment)
    {
        if (Status is OrderStatus.OrderCancelledByUser) 
            throw new DomainException($"Order is already canceled by user");

        if (IsShouldRequestPayment)
        {
            if (Status is not OrderStatus.PaymentPending) 
                throw new DomainException($"Could not set payment to {OrderStatus.PaymentPaid}"); 
        }

        if (payment.Amount.Amount <= GrandTotal.Amount) 
            throw new DomainException("Paid amount can not less than total order");

        if (payment.Amount.Currency != Currency) 
            throw new DomainException("Currency for payment is not equal with order currency");

        if (Payment is not null && Payment.Amount.Amount > 0.00M) 
            throw new DomainException("Payment already paid");

        Payment = payment;
        Payment.SetEntityState(EntityState.Added);

        // check if order scheduled and scheduled on is in the past
        if (IsScheduled && payment.CreatedAt >= ScheduledOnUtc) ScheduledOnUtc = payment.CreatedAt;

        if (IsShouldRequestPayment)
        {
            Status = OrderStatus.PaymentPaid;
            RaiseOrderPaymentPaidDomainEvent(Id, Buyer.BuyerId, payment.Amount.Amount, payment.Amount.Currency);

            if (!IsScheduled)
            {
                Status = OrderStatus.FindingMechanic;
                RaiseOrderFindingMechanicDomainEvent(Id, Buyer.BuyerId, Address.Latitude, Address.Longitude);
            }
        }
    }
     

    private bool IsCancelableByMechanic(Guid mechanicId, out string errorMessage)
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

    public void CancelByMechanic(Guid mechanicId)
    {
        if (Status is not OrderStatus.MechanicDispatched &&
            Status is not OrderStatus.MechanicArrived &&
            Status is not OrderStatus.MechanicAssigned) 
            throw new DomainException($"Could not canceling order by mechanic when order status is {Status}"); 

        if (!IsCancelableByMechanic(mechanicId, out string errorMessage)) 
            throw new DomainException(errorMessage); 

        Mechanic = null;
        TotalCanceledByMechanic++;
        Status = OrderStatus.FindingMechanic;   
        RaiseOrderCancelledByMechanicDomainEvent(mechanicId); 
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
        if (Status is OrderStatus.MechanicAssigned or OrderStatus.MechanicDispatched or OrderStatus.MechanicArrived or OrderStatus.MechanicArrived or OrderStatus.ServiceInProgress or OrderStatus.ServiceCompleted or OrderStatus.ServiceIncompleted)
        {
            return true;
        }

        return false;
    }

    private bool IsRefundableCancellation()
    {
        // if business user get no refund, otherwise will get refund
        if (!IsShouldRequestPayment)
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


    public void CancelByBuyer(Guid buyerId, IEnumerable<string> chargesCancellationFeeDescriptions)
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
                Fees.Where(x => chargesCancellationFeeDescriptions.Contains(x.FeeDescription)).ToArray());
        }

        if (IsRefundableCancellation())
        {
            Cancellation.ApplyCancellationRefund(GrandTotal);
        }

        Status = OrderStatus.OrderCancelledByUser;
        RaiseOrderCancelledByBuyerDomainEvent(Id, buyerId, Mechanic?.MechanicId);
        Mechanic = null;
    }

    public void SetServiceInProgress(Guid mechanicId, string secret, Guid fleetId)
    {
        if (Status is OrderStatus.OrderCancelledByUser)
        {
            throw new DomainException("Order is already canceled by user");
        }

        if (Status is not OrderStatus.MechanicArrived)
        {
            throw new DomainException($"Order status must be {OrderStatus.MechanicArrived}");
        }

        if (mechanicId == Guid.Empty)
        {
            throw new DomainException("Mechanic ID must no be empty");
        } 

        if (fleetId == Guid.Empty)
        {
            throw new DomainException("Fleet ID must not be empty");
        }

        var fleetExists = Fleets.Where(x => x.FleetId == fleetId).ToList().Count > 0;

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

        if (!Secret.Equals(secret))
        {
            throw new DomainException("Order secret is missmatch");
        }

        Status = OrderStatus.ServiceInProgress; 
        RaiseServiceProcessedDomainEvent(Id, Buyer.BuyerId, mechanicId);
    }

    public void SetServiceCompleted(Guid mechanicId, string secret, Guid fleetId)
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

        if (fleetId == Guid.Empty)
        {
            throw new DomainException("Fleet ID must not be empty");
        }

        var fleetExists = Fleets.Where(x => x.FleetId == fleetId).ToList().Count > 0;

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

        if (!Secret.Equals(secret))
        {
            throw new DomainException("Buyer ID is missmatch");
        }

        Status = OrderStatus.ServiceCompleted;
        RaiseServiceCompletedDomainEvent(Id, Buyer.BuyerId, mechanicId);
    }

    public void SetServiceIncompleted(Guid mechanicId, string secret, Guid fleetId)
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

        if (fleetId == Guid.Empty)
        {
            throw new DomainException("Fleet ID must not be empty");
        }

        var fleetExists = Fleets.Where(x => x.FleetId == fleetId).ToList().Count > 0;

        if (!fleetExists)
        {
            throw new DomainException("Fleet ID does not exists");
        }

        if (Mechanic is null)
        {
            throw new DomainException("Mechanic has not been set");
        }

        if (Secret != secret)
        {
            throw new Exception("Order secret is missmatch");
        }  

        if (!Mechanic.MechanicId.Equals(mechanicId))
        {
            throw new DomainException("Mechanic ID is missmatch");
        }
         
        Status = OrderStatus.ServiceIncompleted;
        RaiseServiceIncompletedDomainEvent(Id, Buyer.BuyerId, mechanicId);
    }


    public void SetFindingMechanic()
    {
        if (Status is OrderStatus.OrderCancelledByUser)
        {
            throw new DomainException($"Order is already canceled by user");
        }

        if (Status is not OrderStatus.PaymentPaid)
        {
            throw new DomainException($"Could not set order status to finding mechanic");
        }

        if (Mechanic is not null)
        {
            throw new DomainException("Mechanic is already assigned, mechanic need to be reset");
        }

        Status = OrderStatus.FindingMechanic;
        RaiseOrderFindingMechanicDomainEvent(Id, Buyer.BuyerId, Address.Latitude, Address.Longitude);
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
        RaiseOrderRatedDomainEvent(Id, Buyer.BuyerId, rating, comment);
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

    public void AssignMechanic(Mechanic mechanic)
    {
        if (mechanic is null)
        {
            throw new DomainException("Mechanic can not be null");
        }

        if (!IsOrderMechanicAssignable(mechanic.MechanicId, out string errorMessage))
        {
            throw new DomainException(errorMessage);
        }

        if (IsScheduled && DateTime.UtcNow < ScheduledOnUtc)
        { 
            if (ScheduledOnUtc is null)
            {
                throw new Exception("Inconsistent aggregate is occured");
            }

            throw new InvalidDateOperationException("The mechanic can only be assigned in the range of order schedule date", ScheduledOnUtc.Value);
        } 

        Status = OrderStatus.MechanicAssigned;
        Mechanic = mechanic;
        Mechanic.SetEntityState(EntityState.Added);
        RaiseMechanicAssignedDomainEvent(Id, Buyer.BuyerId, mechanic.MechanicId);
    }

    public void SetDispatchMechanic(Guid mechanicId)
    {
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

        Status = OrderStatus.MechanicDispatched; 
        RaiseMechanicDispatchedDomainEvent(Id, Buyer.BuyerId, mechanicId);
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
        RaiseMechanicArrivedDomainEvent(Id, Buyer.BuyerId, mechanicId);
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
        GrandTotal -= Discount?.Apply(totalAmount, Currency) ?? new Money(0, Currency);

        var holdedGrandTotal = GrandTotal;

        foreach (var fee in Fees)
        {
            GrandTotal += fee.Apply(holdedGrandTotal.Amount, Currency);
        }
    }

    public void PayCancellationRefund(Money money) 
    {
        if (Cancellation is null)
        {
            throw new DomainException("The order does not have cancellation history", "Cancellation");
        }

        Cancellation.SetRefundPaid(money);
        RaiseRefundPaidDomainEvent(
            Id,
            Buyer.BuyerId,
            Cancellation.CancellationRefund!.Amount,
            Cancellation.CancellationRefund!.Currency);
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

        var items = LineItems.Where(x => x.LineItemId == lineItem.LineItemId).FirstOrDefault();

        if (items is not null)
        {
            items.AddQuantity(lineItem.Quantity);
            return;
        }

        lineItem.SetEntityState(EntityState.Added);
        LineItems.Add(lineItem);
    }

    public void ApplyDiscount(Discount coupon)
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
         
        Discount = coupon;
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

    //public void AddPreServiceInspectionDefault(
    //  string description,
    //  string parameter,
    //  int rating,
    //  ICollection<(string description, string parameter, bool isWorking)> preInspectionResults)


    public void AddFleet(
        Fleet fleet,
        IEnumerable<(string description, string parameter, int value)> basicInspectionsDefaults,
        IEnumerable<(string description, string parameter, int rating, IEnumerable<(string description, string parameter, bool isWorking)> result)> preInspectionDefaults)
    {
        if (fleet.FleetId == Guid.Empty)
            throw new ArgumentNullException(nameof(fleet.FleetId), "Fleet id cannot be null.");

        if (fleet is null) throw new ArgumentNullException(nameof(fleet));
        if (fleet.Brand is null) throw new ArgumentNullException(nameof(fleet));
        if (fleet.Model is null) throw new ArgumentNullException(nameof(fleet));
        if (fleet.RegistrationNumber is null) throw new ArgumentNullException(nameof(fleet));
        if (fleet.ImageUrl is null) throw new ArgumentNullException(nameof(fleet));   

        if (Fleets.Where(x => x.FleetId == fleet.FleetId).ToList().Count > 0)
        {
            return;
        }

        fleet.SetEntityState(EntityState.Added);

        foreach(var (description, parameter, value) in basicInspectionsDefaults)
        {
            fleet.AddBasicInspectionDefault(
                description,
                parameter,
                value);
        }

        foreach (var (description, parameter, rating, result) in preInspectionDefaults)
        {
            fleet.AddPreServiceInspectionDefault(
                description,
                parameter,
                rating,
                result);
        }

        Fleets.Add(fleet); 
    }

    private void RaiseServiceCompletedDomainEvent(Guid orderId, Guid buyerId, Guid mechanicId)
    {
        AddDomainEvent(new ServiceCompletedDomainEvent(orderId, buyerId, mechanicId));
    }
     
    private void RaiseServiceIncompletedDomainEvent(Guid orderId, Guid buyerId, Guid mechanicId)
    {
        AddDomainEvent(new ServiceIncompletedDomainEvent(orderId, buyerId, mechanicId));
    } 
    private void RaiseServiceProcessedDomainEvent(Guid orderId, Guid buyerId, Guid mechanicId)
    {
        AddDomainEvent(new ServiceProcessedDomainEvent(orderId, buyerId, mechanicId));
    }

    private void RaiseMechanicArrivedDomainEvent(Guid orderId, Guid buyerId, Guid mechanicId)
    {
        AddDomainEvent(new MechanicArrivedDomainEvent(orderId, buyerId, mechanicId));
    }

    private void RaiseMechanicDispatchedDomainEvent(Guid orderId, Guid buyerId, Guid mechanicId)
    {
        AddDomainEvent(new MechanicDispatchedDomainEvent(orderId, buyerId, mechanicId));
    }

    private void RaiseOrderRatedDomainEvent(Guid orderId, Guid buyerId, decimal value, string? comment)
    {
        AddDomainEvent(new OrderRatedDomainEvent(orderId, buyerId, value, comment));
    } 
    private void RaiseOrderCancelledByBuyerDomainEvent(Guid orderId, Guid buyerId, Guid? mechanicId)
    { 
        AddDomainEvent(new OrderCancelledByBuyerDomainEvent(orderId, buyerId, mechanicId));
    }

    private void RaiseMechanicAssignedDomainEvent(Guid orderId, Guid buyerId, Guid mechanicId)
    {
        AddDomainEvent(new MechanicAssignedDomainEvent(orderId, buyerId, mechanicId));
    }

    private void RaiseOrderCancelledByMechanicDomainEvent(Guid mechanicId)
    {
        AddDomainEvent(new OrderCancelledByMechanicDomainEvent(Id, Buyer.BuyerId, mechanicId));
    }

    private void RaiseOrderPaymentPaidDomainEvent(Guid orderId, Guid buyerId, decimal amount, Currency currency)
    {  
        AddDomainEvent(new OrderPaymentPaidDomainEvent(orderId, buyerId, amount, currency));
    }

    private void RaiseOrderFindingMechanicDomainEvent(Guid orderId, Guid buyerId, double latitude, double longitude)
    {
        AddDomainEvent(new OrderFindingMechanicDomainEvent(orderId, buyerId, latitude, longitude));
    }
    public void RaiseOrderCreatedDomainEvent()
    {
        AddDomainEvent(new OrderCreatedDomainEvent(this));
    }

    private void RaiseRefundPaidDomainEvent(Guid orderId, Guid buyerId, decimal amount, Currency currency)
    {
        AddDomainEvent(new RefundPaidDomainEvent(orderId, buyerId, amount, currency));
    }
}
