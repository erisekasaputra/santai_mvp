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
    public required string RegistrationNumber { get; set; } = registrationNumber.Clean();
    public required VehicleType VehicleType { get; set; } = vehicleType;
    public required string Brand { get; set; } = brand.Clean();
    public required string Model { get; set; } = model.Clean();
    public required int YearOfManufacture { get; set; } = yearOfManufacture;
    public required string ChassisNumber { get; set; } = chassisNumber.Clean();
    public required string EngineNumber { get; set; } = engineNumber.Clean();
    public required string InsuranceNumber { get; set; } = insuranceNumber.Clean();
    public required bool IsInsuranceValid { get; set; } = isInsuranceValid;
    public required DateTime LastInspectionDateLocal { get; set; } = lastInspectionDateLocal;
    public required int OdometerReading { get; set; } = odometerReading;
    public required FuelType FuelType { get; set; } = fuelType; 
    public required string OwnerName { get; set; } = ownerName.Clean();
    public required string OwnerAddress { get; set; } = ownerAddress.Clean();
    public required UsageStatus UsageStatus { get; set; } = usageStatus;
    public required OwnershipStatus OwnershipStatus { get; set; } = ownershipStatus;
    public required TransmissionType TransmissionType { get; set; } = transmissionType;
    public string? ImageUrl { get; set; } = imageUrl?.Clean();
}