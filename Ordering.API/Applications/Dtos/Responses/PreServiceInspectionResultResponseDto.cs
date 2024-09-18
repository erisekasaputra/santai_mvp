namespace Ordering.API.Applications.Dtos.Responses;

public class PreServiceInspectionResultResponseDto
{
    public string Description { get; private set; }
    public string Parameter { get; private set; }
    public bool IsWorking { get; private set; }
    public PreServiceInspectionResultResponseDto(
        string description,
        string parameter,
        bool isWorking)
    {
        Description = description;
        Parameter = parameter;
        IsWorking = isWorking;
    }
}
