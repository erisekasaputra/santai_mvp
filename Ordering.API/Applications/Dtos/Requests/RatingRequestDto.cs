namespace Ordering.API.Applications.Dtos.Requests;

public class RatingRequestDto
{
    public required decimal Value { get; set; }
    public required string Comment { get; set; }
    public IEnumerable<string>? Images { get; private set; }
    public RatingRequestDto(decimal value, string comment, IEnumerable<string>? images)
    {
        Value = value;
        Comment = comment;
        Images = images;
    }
}
