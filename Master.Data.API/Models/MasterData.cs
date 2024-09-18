namespace Master.Data.API.Models; 

public class MasterData
{
    public IEnumerable<Fee> Fees { get; set; }
    public IEnumerable<BasicInspection> BasicInspections { get; set; }
    public IEnumerable<PreServiceInspection> PreServiceInspections { get; set; }
    public MasterData(
        IEnumerable<Fee> fee,
        IEnumerable<BasicInspection> basicInspections,
        IEnumerable<PreServiceInspection> preServicesInspections)
    {
        Fees = fee;
        BasicInspections = basicInspections;
        PreServiceInspections = preServicesInspections;
    }
}
