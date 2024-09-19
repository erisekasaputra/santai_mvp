using Ordering.API.Applications.Dtos.Responses;

namespace Ordering.API.Applications.Dtos.Requests;

public class PreServiceInspectionRequest
{
    public string Description { get; set; }
    public string Parameter { get; set; }
    public int Rating { get; set; }
    public IEnumerable<PreServiceInspectionResultRequest> PreServiceInspectionResults { get; set; }
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
