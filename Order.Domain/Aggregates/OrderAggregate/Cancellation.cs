using Core.Exceptions;
using Core.Enumerations;
using Core.ValueObjects;
using Order.Domain.Enumerations; 
using Order.Domain.SeedWork; 

namespace Order.Domain.Aggregates.OrderAggregate;

public class Cancellation : Entity
{
    public Guid OrderingId { get; private set; }
    public ICollection<CancellationFee> CancellationCharges { get; private set; }
    public Money? CancellationRefund { get; private set; }

    public Cancellation()
    {
        CancellationCharges = [];
    }

    public Cancellation(Guid orderingId)
    {
        OrderingId = orderingId;
        CancellationCharges = [];
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
                 
                cancellationFee.SetEntityState(EntityStateAction.Added);  
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

        Money totalCancellationFees = new (totalCancellationFeeAmount, refund.Currency);

        CancellationRefund = refund - totalCancellationFees;

        if (CancellationRefund.Amount < 0)
        {
            throw new DomainException("Refund amount cannot be negative after applying charges.");
        }
    }
}
