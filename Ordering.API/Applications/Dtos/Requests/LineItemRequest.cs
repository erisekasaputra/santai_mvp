namespace Ordering.API.Applications.Dtos.Requests;

public class LineItemRequest
{
    public required Guid Id { get; private set; }
    public required int Quantity { get; private set; }

    public LineItemRequest(Guid id, int quantity)
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
    }
}
