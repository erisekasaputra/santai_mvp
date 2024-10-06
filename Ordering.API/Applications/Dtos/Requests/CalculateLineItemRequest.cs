using Core.Enumerations;

namespace Ordering.API.Applications.Dtos.Requests;

public class CalculateLineItemRequest
{
    public required Guid Id { get; set; }
    public required int Quantity { get; set; }
    public required decimal Price { get; set; }
    public Currency Currency { get; set; }

    public CalculateLineItemRequest(
        Guid id, 
        int quantity,
        decimal price,
        Currency currency)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(id), "ID can not be empty");
        }

        if (quantity < 1)
        {
            throw new ArgumentException("Minimum quantity is 1");
        }

        Id = id;
        Quantity = quantity;
        Price = price;
        Currency = currency;
    }
}
