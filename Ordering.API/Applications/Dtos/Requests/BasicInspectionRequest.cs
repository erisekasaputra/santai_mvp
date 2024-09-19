namespace Ordering.API.Applications.Dtos.Requests;

public class BasicInspectionRequest
{
    public string Description { get; set; }
    public string Parameter { get; set; }
    public int Value { get; set; }

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
