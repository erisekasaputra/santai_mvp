namespace Ordering.API.Applications.Dtos.Requests;

public class PreServiceInspectionsRequest
{
    public required IEnumerable<PreServiceInspectionRequest> PreServiceInspections { get; set; }
    public PreServiceInspectionsRequest(
        IEnumerable<PreServiceInspectionRequest> preServiceInspections)
    {
        PreServiceInspections = preServiceInspections;
    }
}
