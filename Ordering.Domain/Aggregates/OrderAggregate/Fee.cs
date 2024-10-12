using Core.Enumerations;
using Core.Exceptions;
using Core.ValueObjects;
using Ordering.Domain.Enumerations;
using Ordering.Domain.SeedWork;

namespace Ordering.Domain.Aggregates.OrderAggregate;

public class Fee : Entity
{
    public Guid OrderId { get; private set; }
    public PercentageOrValueType PercentageOrValueType { get; private set; }
    public string FeeDescription { get; private set; }
    public Currency Currency { get; private set; }
    public decimal ValuePercentage { get; private set; }
    public decimal ValueAmount { get; private set; }
    public Money FeeAmount { get; private set; }
    public Fee()
    {
        FeeDescription = string.Empty;
        FeeAmount = null!;
    }
    private Fee(
        Guid orderId,
        string feeDescription,
        PercentageOrValueType percentageOrValueType,
        decimal amount,
        Currency currency)
    {
        if (amount <= 0)
        {
            throw new DomainException("Amount can not less than or equal with 0");
        }

        OrderId = orderId;
        FeeAmount = new Money(0, currency);
        PercentageOrValueType = percentageOrValueType;
        FeeDescription = feeDescription;
        Currency = currency;

        if (percentageOrValueType == PercentageOrValueType.Percentage)
        {
            if (amount < 1 && amount > 100)
            {
                throw new DomainException("Percentage must be between 1 and 100");
            }
            ValuePercentage = amount;
            return;
        }

        if (percentageOrValueType == PercentageOrValueType.Value)
        {
            if (amount < 1)
            {
                throw new DomainException("Amount can not less than or equal with 0");
            }

            ValueAmount = amount;
            return;
        }
    }

    public static Fee CreateByValue(Guid orderingId, string feeDescription, decimal amount, Currency currency)
    {
        return new Fee(orderingId, feeDescription, PercentageOrValueType.Value, amount, currency);
    }

    public static Fee CreateByPercentage(Guid orderingId, string feeDescription, decimal percentage, Currency currency)
    {
        return new Fee(orderingId, feeDescription, PercentageOrValueType.Percentage, percentage, currency);
    }

    public Money Apply(decimal orderAmount, Currency orderCurrency)
    {
        Money feeAmount;

        if (PercentageOrValueType == PercentageOrValueType.Value && ValueAmount > 0)
        {
            if (Currency != orderCurrency)
            {
                throw new DomainException($"Fee currency ({Currency}) does not match order currency ({orderCurrency}).");
            }

            feeAmount = new Money(ValueAmount, Currency); 
        }
        else if (PercentageOrValueType == PercentageOrValueType.Percentage && ValuePercentage > 0)
        {
            feeAmount = new Money(orderAmount * (ValuePercentage / 100), orderCurrency); 
        }
        else
        {
            feeAmount = new Money(0, orderCurrency);
        }

        FeeAmount = feeAmount;

        return feeAmount;
    }
}
