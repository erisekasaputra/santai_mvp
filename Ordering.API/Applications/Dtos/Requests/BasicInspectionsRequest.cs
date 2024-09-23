namespace Ordering.API.Applications.Dtos.Requests;

public class BasicInspectionsRequest
{
    public required IEnumerable<BasicInspectionRequest> BasicInspections { get; set; }
    public BasicInspectionsRequest(
        IEnumerable<BasicInspectionRequest> basicInspections)
    {
        BasicInspections = basicInspections;
    }
}
