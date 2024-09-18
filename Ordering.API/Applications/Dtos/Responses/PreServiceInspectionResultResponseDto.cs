namespace Ordering.API.Applications.Dtos.Responses;

public class PreServiceInspectionResultResponseDto
{
    public string Description { get; set; }
    public string Parameter { get; set; }
    public bool IsWorking { get; set; }
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
