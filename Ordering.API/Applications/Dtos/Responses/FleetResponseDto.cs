namespace Ordering.API.Applications.Dtos.Responses;

public class FleetResponseDto
{ 
    public Guid FleetId { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public string? RegistrationNumber { get; set; }
    public string ImageUrl { get; set; }
    public IEnumerable<BasicInspectionResponseDto> BasicInspections { get; set; }
    public IEnumerable<PreServiceInspectionResponseDto> PreServiceInspections { get; set; }
    public IEnumerable<JobChecklistResponseDto> JobChecklists { get; set; }
    public string Comment { get; set; }
    public FleetResponseDto(
        Guid fleetId,
        string brand,
        string model,
        string? registrationNumber,
        string imageUrl,
        IEnumerable<BasicInspectionResponseDto> basicInspections,
        IEnumerable<PreServiceInspectionResponseDto> preServiceInspections,
        IEnumerable<JobChecklistResponseDto> jobChecklists,
        string comment)
    {
        FleetId = fleetId;
        Brand = brand;
        Model = model;
        RegistrationNumber = registrationNumber;
        ImageUrl = imageUrl;
        BasicInspections = basicInspections;
        PreServiceInspections = preServiceInspections;
        JobChecklists = jobChecklists;
        Comment = comment;
    }
}
