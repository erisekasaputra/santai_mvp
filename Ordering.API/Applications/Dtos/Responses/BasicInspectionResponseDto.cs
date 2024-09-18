namespace Ordering.API.Applications.Dtos.Responses;

public class BasicInspectionResponseDto
{
    public string Description { get; private set; }
    public string Parameter { get; private set; }
    public int Value { get; private set; }

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
