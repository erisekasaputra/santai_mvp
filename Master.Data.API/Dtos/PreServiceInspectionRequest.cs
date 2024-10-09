using Master.Data.API.Models;

namespace Master.Data.API.Dtos;

public class PreServiceInspectionRequest
{
    public IEnumerable<PreServiceInspection> PreServiceInspections { get; set; }
    public PreServiceInspectionRequest(
        IEnumerable<PreServiceInspection> preServiceInspections)
    {
        PreServiceInspections = preServiceInspections;
    }
}
