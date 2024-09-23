namespace Ordering.API.Applications.Dtos.Requests;

public class PreServiceInspectionResultRequest
{
    public required string Description { get; set; }
    public required string Parameter { get; set; }
    public required bool IsWorking { get; set; }
    public PreServiceInspectionResultRequest(
        string description,
        string parameter,
        bool isWorking)
    {
        Description = description;
        Parameter = parameter;
        IsWorking = isWorking;
    }
}
