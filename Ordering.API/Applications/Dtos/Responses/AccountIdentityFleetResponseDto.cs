
namespace Ordering.API.Applications.Dtos.Responses;

public class AccountIdentityFleetResponseDto
{
    public Guid Id { get; set; }
    public string RegistrationNumber { get; set; } 
    public string Brand { get; set; }
    public string Model { get; set; }
    public int YearOfManufacture { get; set; }
    public string ChassisNumber { get; set; }
    public string EngineNumber { get; set; }
    public string InsuranceNumber { get; set; }
    public bool IsInsuranceValid { get; set; }
    public DateTime LastInspectionDateLocal { get; set; }
    public int OdometerReading { get; set; } 
    public string OwnerName { get; set; }
    public string OwnerAddress { get; set; }
    public string? ImageUrl { get; set; }
    public AccountIdentityFleetResponseDto( 
        Guid id,
        string registrationNumber,
        string brand,
        string model,
        int yearOfManufacture,
        string chassisNumber,
        string engineNumber,
        string insuranceNumber,
        bool isInsuranceValid,
        DateTime lastInspectionDateLocal,
        int odometerReading,
        string ownerName,
        string ownerAddress,
        string? imageUrl)
    {
        Id = id;
        RegistrationNumber = registrationNumber;
        Brand = brand;
        Model = model;
        YearOfManufacture = yearOfManufacture;
        ChassisNumber = chassisNumber;
        EngineNumber = engineNumber;
        InsuranceNumber = insuranceNumber;
        IsInsuranceValid = isInsuranceValid;
        LastInspectionDateLocal = lastInspectionDateLocal;
        OdometerReading = odometerReading;
        OwnerName = ownerName;
        OwnerAddress = ownerAddress;
        ImageUrl = imageUrl;
    }
}
