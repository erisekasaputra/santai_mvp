namespace Ordering.API.Applications.Dtos.Requests;

public class PreServiceInspectionResultRequest
{
    public string Description { get; set; }
    public string Parameter { get; set; }
    public bool IsWorking { get; set; }
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
