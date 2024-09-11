using Core.Enumerations;

namespace Ordering.API.Applications.Dtos.Responses;

public class LineItemResponseDto
{
    public string Name { get; private set; }
    public string Sku { get; private set; }
    public decimal UnitPrice { get; private set; }
    public Currency Currency { get; private set; }
    public int Quantity { get; private set; }
    public decimal? TaxPercentage { get; private set; }
    public decimal? TaxValue { get; private set; }
    public decimal SubTotal { get; private set; }

    public LineItemResponseDto(
        string name,
        string sku,
        decimal unitPrice,
        Currency currency,
        decimal? taxPercentage,
        decimal? taxValue,
        decimal subTotal)
    {
        Name = name; 
        Sku = sku;
        UnitPrice = unitPrice;
        Currency = currency;
        TaxPercentage = taxPercentage;
        TaxValue = taxValue;
        SubTotal = subTotal; 
    }
}
