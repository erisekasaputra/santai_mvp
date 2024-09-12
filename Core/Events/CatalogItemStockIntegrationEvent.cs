namespace Core.Events;

public class CatalogItemStockIntegrationEvent
{
    public Guid Id { get; set; } 
    public int Quantity { get; set; }
    public CatalogItemStockIntegrationEvent(Guid id, int quantity)
    {
        Id = id;
        Quantity = quantity;
    }
}
