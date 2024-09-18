namespace Ordering.API.Applications.Dtos.Responses;

public class PreServiceInspectionResponseDto
{
    public string Description { get; private set; }
    public string Parameter { get; private set; }
    public int Rating { get; private set; }
    public IEnumerable<PreServiceInspectionResultResponseDto> PreServiceInspectionResults { get; private set; }
    public PreServiceInspectionResponseDto(
        string description,
        string parameter,
        int rating,
        IEnumerable<PreServiceInspectionResultResponseDto> preServiceInspectionResults)
    {
        Description = description;
        Parameter = parameter;
        Rating = rating;
        PreServiceInspectionResults = preServiceInspectionResults;
    }
}
