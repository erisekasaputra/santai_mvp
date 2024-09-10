using Core.Exceptions;
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

        if (cancellationCharges == null || cancellationCharges.ToList().Count <= 0)
        {
            throw new DomainException("Cancellation charges cannot be null.");
        }

        CancellationCharges = cancellationCharges
            .Where(x => x.PercentageOrValueType == PercentageOrValueType.Percentage ||
                        x.PercentageOrValueType == PercentageOrValueType.Value)
            .Select(x =>
            {
                if (x.PercentageOrValueType == PercentageOrValueType.Percentage)
                {
                    return CancellationFee.CreateByPercentage(Id, x.FeeDescription, x.ValuePercentage, x.Currency);
                }
                 
                return CancellationFee.CreateByValue(Id, x.FeeDescription, x.ValueAmount, x.Currency);
            })
            .ToList();
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
