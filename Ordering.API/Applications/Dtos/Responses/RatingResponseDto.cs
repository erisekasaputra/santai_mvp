namespace Ordering.API.Applications.Dtos.Responses;

public class RatingResponseDto
{
    public decimal Value { get; private set; }
    public string? Comment { get; private set; }
    public RatingResponseDto(
        decimal value, 
        string? comment)
    {
        Value = value;
        Comment = comment;
    }
}
