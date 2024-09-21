using Core.Exceptions;
using Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Ordering.Domain.SeedWork;
using Ordering.Domain.Enumerations;

namespace Ordering.Domain.Aggregates.OrderAggregate;

public class Cancellation : Entity
{
    public Guid OrderId { get; private set; }
    public ICollection<CancellationFee> CancellationCharges { get; private set; }
    public Money? CancellationRefund { get; private set; }
    public DateTime ShouldRefundAtUt { get; set; }
    public bool IsRefundPaid { get; set; }
    public Cancellation()
    {
        CancellationCharges = [];
    }

    public Cancellation(
        Guid orderId)
    {
        OrderId = orderId;
        CancellationCharges = [];
        ShouldRefundAtUt.AddDays(7);
        IsRefundPaid = false;
    }

    public void SetRefundPaid(Money money)
    {
        if (CancellationRefund is null)
        {
            throw new DomainException("Cancellation refund not allowed");
        }

        if (money > CancellationRefund)
        {
            throw new DomainException("The money you give back to the buyer cannot be greater than the total order that should be refunded.");
        } 
       

        decimal paid = Math.Truncate(money.Amount * 100) / 100;
        decimal refund = Math.Truncate(CancellationRefund.Amount * 100) / 100;
        if (paid > refund)
        {
            throw new DomainException("Paid amount can not be less than the total order");
        }

        if (DateTime.UtcNow > ShouldRefundAtUt) 
        {
            throw new InvalidDateOperationException("Cannot process the refund when the date is still outside the range", ShouldRefundAtUt);
        }

        IsRefundPaid = true;
    }

    public void ApplyCancellationCharge(IEnumerable<Fee> cancellationCharges)
    {
        CancellationCharges ??= [];

        if (cancellationCharges == null || !cancellationCharges.Any())
        {
            throw new DomainException("Cancellation charges cannot be null.");
        }

        foreach (var fee in cancellationCharges)
        {
            if (fee.PercentageOrValueType == PercentageOrValueType.Percentage ||
                fee.PercentageOrValueType == PercentageOrValueType.Value)
            {
                CancellationFee cancellationFee;

                if (fee.PercentageOrValueType == PercentageOrValueType.Percentage)
                {
                    cancellationFee = CancellationFee.CreateByPercentage(Id, fee.FeeDescription, fee.ValuePercentage, fee.Currency);
                }
                else
                {
                    cancellationFee = CancellationFee.CreateByValue(Id, fee.FeeDescription, fee.ValueAmount, fee.Currency);
                }

                cancellationFee.SetEntityState(EntityState.Added);
                CancellationCharges.Add(cancellationFee);
            }
        }
    }

    public void ApplyCancellationRefund(Money refund)
    {
        if (refund is null)
        {
            throw new DomainException("Money refund can not be null");
        }

        if (refund.Amount <= 0)
        {
            throw new DomainException("Refund amount can not less than or equal with 0");
        }

        decimal totalCancellationFeeAmount = CancellationCharges?.Sum(x => x.FeeAmount.Amount) ?? 0;

        Money totalCancellationFees = new(totalCancellationFeeAmount, refund.Currency);

        CancellationRefund = refund - totalCancellationFees;

        if (CancellationRefund.Amount < 0)
        {
            throw new DomainException("Refund amount cannot be negative after applying charges.");
        }

        if (CancellationRefund.Amount == 0)
        {
            IsRefundPaid = true;
        }
    }
}
