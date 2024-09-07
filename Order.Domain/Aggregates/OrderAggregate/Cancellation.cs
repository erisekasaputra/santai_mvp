using Order.Domain.Exceptions;
using Order.Domain.SeedWork;
using Order.Domain.ValueObjects;

namespace Order.Domain.Aggregates.OrderAggregate;

public class Cancellation : Entity
{
    public ICollection<Fee>? CancellationCharges { get; private set; }
    public Money? CancellationRefund { get; private set; }

    public Cancellation()
    { 
    }

    public void ApplyCancellationCharge(IEnumerable<Fee> cancellationCharges)
    {
        if (cancellationCharges == null || cancellationCharges.ToList().Count <= 0)
        {
            throw new DomainException("Cancellation charges cannot be null.");
        }

        CancellationCharges = cancellationCharges.ToList();
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
