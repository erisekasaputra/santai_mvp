namespace Account.API.Applications.Dtos.RequestDtos;

public class LocationRequestDto(double latitude, double longitude)
{
    public double Latitude { get; set; } = latitude;
    public double Longitude { get; set; } = longitude;
}
