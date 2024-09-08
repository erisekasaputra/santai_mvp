using Account.API.Extensions;
using Account.Domain.Enumerations;
using Core.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;

public class CreateFleetRequestDto(
    string registrationNumber,
    VehicleType vehicleType,
    string brand,
    string model,
    int yearOfManufacture,
    string chassisNumber,
    string engineNumber,
    string insuranceNumber,
    bool isInsuranceValid,
    DateTime lastInspectionDateLocal,
    int odometerReading,
    FuelType fuelType, 
    string ownerName,
    string ownerAddress,
    UsageStatus usageStatus,
    OwnershipStatus ownershipStatus,
    TransmissionType transmissionType,
    string? imageUrl)
{
    public string RegistrationNumber { get; set; } = registrationNumber.Clean();
    public VehicleType VehicleType { get; set; } = vehicleType;
    public string Brand { get; set; } = brand.Clean();
    public string Model { get; set; } = model.Clean();
    public int YearOfManufacture { get; set; } = yearOfManufacture;
    public string ChassisNumber { get; set; } = chassisNumber.Clean();
    public string EngineNumber { get; set; } = engineNumber.Clean();
    public string InsuranceNumber { get; set; } = insuranceNumber.Clean();
    public bool IsInsuranceValid { get; set; } = isInsuranceValid;
    public DateTime LastInspectionDateLocal { get; set; } = lastInspectionDateLocal;
    public int OdometerReading { get; set; } = odometerReading;
    public FuelType FuelType { get; set; } = fuelType; 
    public string OwnerName { get; set; } = ownerName.Clean();
    public string OwnerAddress { get; set; } = ownerAddress.Clean();
    public UsageStatus UsageStatus { get; set; } = usageStatus;
    public OwnershipStatus OwnershipStatus { get; set; } = ownershipStatus;
    public TransmissionType TransmissionType { get; set; } = transmissionType;
    public string? ImageUrl { get; set; } = imageUrl?.Clean();
}