using Order.Domain.Enumerations;
using Order.Domain.Exceptions;
using Order.Domain.SeedWork;
using Order.Domain.ValueObjects;

namespace Order.Domain.Aggregates.OrderAggregate;

public class Fee : Entity
{ 
    public PercentageOrValueType PercentageOrValueType { get; private set; }
    public FeeDescription FeeDescription { get; private set; }
    public Money Amount { get; set; } 

    private Fee(FeeDescription feeDescription, PercentageOrValueType percentageOrValueType, decimal amount, Currency currency, Money? reference = null)
    {
        if (amount <= 0) 
        {
            throw new DomainException("Amount can not less than or equal with 0");
        }

        PercentageOrValueType = percentageOrValueType;
        FeeDescription = feeDescription;
         
        if (percentageOrValueType == PercentageOrValueType.Percentage)
        {
            if (amount < 1 && amount > 100)
            {
                throw new DomainException("Percentage must be between 1 and 100");
            }

            if (reference is null)
            {
                throw new DomainException("Amount reference can not be empty");
            }  

            Amount = new Money(reference.Amount * (amount / 100), currency);
            return;
        }

        if (percentageOrValueType == PercentageOrValueType.Value)
        {
            Amount = new Money(amount, currency);
            return;
        } 

        Amount = new Money(0, currency);
        throw new DomainException("Invalid state is occured, Percentage or Value type may invalid");
    } 

    public static Fee CreateByValue(FeeDescription feeDescription, decimal amount, Currency currency)
    {
        return new Fee(feeDescription, PercentageOrValueType.Value, amount, currency);
    }

    public static Fee CreateByPercentage(FeeDescription feeDescription, decimal percentage, Money reference) 
    {
        return new Fee(feeDescription, PercentageOrValueType.Percentage, percentage, reference.Currency, reference);
    }
}
