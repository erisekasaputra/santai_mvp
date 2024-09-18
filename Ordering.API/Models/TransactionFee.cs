using Core.Enumerations;
using Ordering.Domain.Enumerations;

namespace Ordering.API.Models;

public class TransactionFee
{
    public string FeeName { get; set; }
    public string FeeDecriptionDetail { get; set; }
    public string FeeDescription { get; set; }
    public PercentageOrValueType PercentageOrValueType { get; set; }
    public Currency Currency { get; set; }
    public decimal ValuePercentage { get; set; }
    public decimal ValueAmount { get; set; }
    public bool IsChargedForBuy { get; set; }
    public TransactionFee(
        string feeName,
        string feeDescriptionDetail,
        string feeDescription,
        bool isChargesForBuy)
    {
        FeeName = feeName;
        FeeDecriptionDetail = feeDescriptionDetail;
        FeeDescription = feeDescription;
        IsChargedForBuy = isChargesForBuy;
    }
}

