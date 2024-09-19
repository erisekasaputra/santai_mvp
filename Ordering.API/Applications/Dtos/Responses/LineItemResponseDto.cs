using Core.Enumerations;

namespace Ordering.API.Applications.Dtos.Responses;

public class LineItemResponseDto
{
    public Guid LineItemId { get; set; }
    public string Name { get; set; }
    public string Sku { get; set; }
    public decimal UnitPrice { get; set; }
    public Currency Currency { get; set; }
    public int Quantity { get; set; }
    public decimal? TaxPercentage { get; set; }
    public decimal? TaxValue { get; set; }
    public decimal SubTotal { get; set; }

    public LineItemResponseDto(
        Guid lineItemId,
        string name,
        string sku,
        decimal unitPrice,
        Currency currency,
        decimal? taxPercentage,
        decimal? taxValue,
        decimal subTotal)
    {
        LineItemId = lineItemId;
        Name = name; 
        Sku = sku;
        UnitPrice = unitPrice;
        Currency = currency;
        TaxPercentage = taxPercentage;
        TaxValue = taxValue;
        SubTotal = subTotal; 
    }
}
