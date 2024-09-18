using Core.Enumerations;

namespace Master.Data.API.Models;

public class Fee
{
    public string Parameter { get; private set; }
    public string FeeDecriptionDetail { get; private set; }
    public string FeeDescription { get; private set; }
    public Currency Currency { get; private set; }
    public decimal ValuePercentage { get; private set; }
    public decimal ValueAmount { get; private set; }
    public decimal FeeAmount { get; private set; }

    public Fee(
        string parameter,
        string feeDescriptionDetail,
        string feeDescription,
        Currency currency,
        decimal valuePercentage,
        decimal valueAmount,
        decimal feeAmount)
    {
        Parameter = parameter;
        FeeDecriptionDetail = feeDescriptionDetail;
        FeeDescription = feeDescription;
        Currency = currency;
        ValuePercentage = valuePercentage;
        ValueAmount = valueAmount;
        FeeAmount = feeAmount;
    }
}
