using Core.Enumerations;
using Core.ValueObjects;
using Ordering.Domain.Enumerations;
using Ordering.Domain.SeedWork;

namespace Ordering.Domain.Aggregates.TransactionFeeAggregate;

public class TransactionFee : Entity
{
    public string FeeName { get; set; }
    public string FeeDecriptionDetail { get; set; }
    public FeeDescription FeeDescription { get; set; } 
    public PercentageOrValueType PercentageOrValueType { get; private set; } 
    public Currency Currency { get; private set; }
    public decimal ValuePercentage { get; private set; }
    public decimal ValueAmount { get; private set; }
    public Money FeeAmount { get; private set; }  
    public bool IsChargedForBuy { get; set; } 
    public TransactionFee(
        string feeName,
        string feeDescriptionDetail,
        FeeDescription feeDescription,
        bool isChargesForBuy)
    {
        FeeName = feeName;
        FeeDecriptionDetail = feeDescriptionDetail;
        FeeDescription = feeDescription;
        IsChargedForBuy = isChargesForBuy;
    }
}
