namespace Ordering.API.Applications.Dtos.Requests;

public class CalculateFleetRequest
{
    public Guid Id { get; private set; } 
    public CalculateFleetRequest(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(id), "ID can not be empty");
        }

        Id = id;
    }
}
