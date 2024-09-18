namespace Ordering.API.Applications.Dtos.Responses;

public class PreServiceInspectionResponseDto
{
    public string Description { get; set; }
    public string Parameter { get; set; }
    public int Rating { get; set; }
    public IEnumerable<PreServiceInspectionResultResponseDto> PreServiceInspectionResults { get; set; }
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
