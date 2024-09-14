using Core.Enumerations; 

namespace Core.Events.Models;

public class OrderLineItemEventModel
{
    public Guid Id { get; set; }
    public Guid LineItemId { get; set; }
    public string Name { get; set; }
    public string? Sku { get; set; }
    public decimal Price { get; set; } 
    public Currency Currency { get; set; }
    public int Quantity { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Subtotal { get; set; }

    public OrderLineItemEventModel(
        Guid id,
        Guid lineItemId,
        string name,
        string? sku,
        decimal price,
        Currency currency,
        int quantity,
        decimal taxAmount,
        decimal subtotal)
    {
        Id = id;
        LineItemId = lineItemId;
        Name = name;
        Sku = sku;
        Price = price;
        Currency = currency;
        Quantity = quantity;
        TaxAmount = taxAmount;
        Subtotal = subtotal;
    }
}
