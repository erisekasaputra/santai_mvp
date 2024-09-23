using Ordering.API.Applications.Dtos.Responses;

namespace Ordering.API.Applications.Dtos.Requests;

public class PreServiceInspectionRequest
{
    public required string Description { get; set; }
    public required string Parameter { get; set; }
    public required int Rating { get; set; }
    public required IEnumerable<PreServiceInspectionResultRequest> PreServiceInspectionResults { get; set; }
    public PreServiceInspectionRequest(
        string description,
        string parameter,
        int rating,
        IEnumerable<PreServiceInspectionResultRequest> preServiceInspectionResults)
    {
        Description = description;
        Parameter = parameter;
        Rating = rating;
        PreServiceInspectionResults = preServiceInspectionResults;
    }
}
