namespace Ordering.API.Applications.Dtos.Responses;

public class MechanicResponseDto
{ 
    public Guid MechanicId { get; set; }
    public string Name { get; set; }
    public RatingResponseDto? Rating { get; set; }
    public decimal Performance { get; set; }
    public bool IsRated { get; set; }

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
