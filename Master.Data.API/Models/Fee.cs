using Core.Enumerations;

namespace Master.Data.API.Models;

public class Fee
{
    public string Parameter { get; set; }
    public string FeeDescriptionDetail { get; set; }
    public string FeeDescription { get; set; }
    public Currency Currency { get; set; }
    public decimal ValuePercentage { get; set; }
    public decimal ValueAmount { get; set; }
    public decimal FeeAmount { get; set; }

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
        FeeDescriptionDetail = feeDescriptionDetail;
        FeeDescription = feeDescription;
        Currency = currency;
        ValuePercentage = valuePercentage;
        ValueAmount = valueAmount;
        FeeAmount = feeAmount;
    }
}
