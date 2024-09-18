namespace Ordering.API.Applications.Dtos.Responses;

public class BasicInspectionResponseDto
{
    public string Description { get; set; }
    public string Parameter { get; set; }
    public int Value { get; set; }

    public BasicInspectionResponseDto(
        string description,
        string parameter,
        int value)
    {
        Description = description;
        Parameter = parameter;
        Value = value;
    }
}
