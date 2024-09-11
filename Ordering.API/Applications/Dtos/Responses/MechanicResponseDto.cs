namespace Ordering.API.Applications.Dtos.Responses;

public class MechanicResponseDto
{ 
    public Guid MechanicId { get; private set; }
    public string Name { get; private set; }
    public RatingResponseDto? Rating { get; private set; }
    public decimal Performance { get; private set; }
    public bool IsRated { get; private set; }

    public MechanicResponseDto(
        Guid mechanicId,
        string name, 
        RatingResponseDto? rating,
        decimal performance,
        bool isRated)
    {
        MechanicId = mechanicId;
        Name = name;
        Rating = rating;
        Performance = performance;
        IsRated = isRated;
    }
}
