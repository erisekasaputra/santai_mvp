namespace Ordering.API.Applications.Dtos.Requests;

public class BasicInspectionRequest
{
    public required string Description { get; set; }
    public required string Parameter { get; set; }
    public required int Value { get; set; }

    public BasicInspectionRequest(
        string description,
        string parameter,
        int value)
    {
        Description = description;
        Parameter = parameter;
        Value = value;
    }
}
