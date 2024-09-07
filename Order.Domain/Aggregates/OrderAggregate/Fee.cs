using Order.Domain.Enumerations;
using Order.Domain.Exceptions;
using Order.Domain.SeedWork;
using Order.Domain.ValueObjects;

namespace Order.Domain.Aggregates.OrderAggregate;

public class Fee : Entity
{ 
    public PercentageOrValueType PercentageOrValueType { get; private set; }
    public FeeDescription FeeDescription { get; private set; }
    public decimal? Percentage { get; private set; }
    public Money? Amount { get; private set; } 
    public Money FeeAmount { get; private set; }

    private Fee(FeeDescription feeDescription, PercentageOrValueType percentageOrValueType, decimal amount, Currency currency)
    {
        if (amount <= 0) 
        {
            throw new DomainException("Amount can not less than or equal with 0");
        }

        FeeAmount = new Money(0, currency);
        PercentageOrValueType = percentageOrValueType;
        FeeDescription = feeDescription;
         
        if (percentageOrValueType == PercentageOrValueType.Percentage)
        {
            if (amount < 1 && amount > 100)
            {
                throw new DomainException("Percentage must be between 1 and 100");
            } 
            Percentage = amount;
            return;
        }

        if (percentageOrValueType == PercentageOrValueType.Value)
        {
            if (amount < 1)
            {
                throw new DomainException("Amount can not less than or equal with 0");
            }

            Amount = new Money(amount, currency);
            return;
        }    
    } 

    public static Fee CreateByValue(FeeDescription feeDescription, decimal amount, Currency currency)
    { 
        return new Fee(feeDescription, PercentageOrValueType.Value, amount, currency);
    }

    public static Fee CreateByPercentage(FeeDescription feeDescription, decimal percentage, Currency currency) 
    {
        return new Fee(feeDescription, PercentageOrValueType.Percentage, percentage, currency);
    }

    public Money Apply(Money orderAmount)
    {
        Money feeAmount;

        if (PercentageOrValueType == PercentageOrValueType.Value && Amount is not null) 
        { 
            if (Amount.Currency != orderAmount.Currency)
            {
                throw new DomainException($"Fee currency ({Amount.Currency}) does not match order currency ({orderAmount.Currency}).");
            }

            feeAmount = Amount; 

            if (orderAmount.Amount < feeAmount.Amount)
            {
                throw new DomainException("Fee amount can not greather than order amount");
            }
        } 
        else if (PercentageOrValueType == PercentageOrValueType.Percentage && Percentage.HasValue && Percentage.Value > 0)
        {
            feeAmount = new Money(orderAmount.Amount * (Percentage.Value / 100), orderAmount.Currency);

            if (orderAmount.Amount < feeAmount.Amount)
            {
                throw new DomainException("Fee amount can not greather than order amount");
            }
        } 
        else
        {
            feeAmount = new Money(0, orderAmount.Currency);
        } 

        FeeAmount = feeAmount;

        return feeAmount;
    }
}
