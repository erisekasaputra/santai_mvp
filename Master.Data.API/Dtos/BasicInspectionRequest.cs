using Master.Data.API.Models;

namespace Master.Data.API.Dtos;

public class BasicInspectionRequest
{
    public IEnumerable<BasicInspection> BasicInspections { get; set; }
    public BasicInspectionRequest(
        IEnumerable<BasicInspection> basicInspections)
    {
        BasicInspections = basicInspections;
    }
}
