namespace Ordering.API.Applications.Dtos.Responses;

public class FleetResponseDto
{ 
    public string Brand { get; private set; }
    public string Model { get; private set; }
    public string RegistrationNumber { get; private set; }
    public string? ImageUrl { get; private set; }
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
