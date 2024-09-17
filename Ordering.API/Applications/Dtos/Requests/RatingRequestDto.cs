namespace Ordering.API.Applications.Dtos.Requests;

public class RatingRequestDto
{
    public decimal Value { get; private set; }
    public string Comment { get; private set; }
    public IEnumerable<string>? Images { get; private set; }
    public RatingRequestDto(decimal value, string comment, IEnumerable<string>? images)
    {
        Value = value;
        Comment = comment;
        Images = images;
    }
}
