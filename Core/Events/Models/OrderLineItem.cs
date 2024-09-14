using Core.Enumerations; 

namespace Core.Events.Models;

public class OrderLineItem
{
    public Guid Id { get; set; }
    public Guid LineItemId { get; set; }
    public string Name { get; set; }
    public string Sku { get; set; }
    public decimal Price { get; set; } 
    public Currency Currency { get; set; }
    public int Quantity { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Subtotal { get; set; }

    public OrderLineItem()
    {
        
    }
}
