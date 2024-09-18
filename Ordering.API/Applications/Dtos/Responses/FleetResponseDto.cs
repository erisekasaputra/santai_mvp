namespace Ordering.API.Applications.Dtos.Responses;

public class FleetResponseDto
{ 
    public string Brand { get; set; }
    public string Model { get; set; }
    public string RegistrationNumber { get; set; }
    public string? ImageUrl { get; set; }
    public FleetResponseDto(
        string brand,
        string model,
        string registrationNumber,
        string? imageUrl)
    {
        Brand = brand;
        Model = model;
        RegistrationNumber = registrationNumber;
        ImageUrl = imageUrl;
    }
}
