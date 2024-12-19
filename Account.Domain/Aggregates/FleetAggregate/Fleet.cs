 
using Account.Domain.Enumerations; 
using Account.Domain.SeedWork;
using Core.Exceptions; 
namespace Account.Domain.Aggregates.FleetAggregate;


public class Fleet : Entity, IAggregateRoot
{
    public Guid? UserId { get; private set; } 

    public Guid? StaffId { get; private set; }
    public string ImageUrl { get; private set; }
    public string Brand { get; private set; }
    public string Model { get; private set; }
    public string? HashedRegistrationNumber { get; private set; }
    public string? EncryptedRegistrationNumber { get; private set; } 
    public VehicleType? VehicleType { get; private set; } 
    public int? YearOfManufacture { get; private set; }
    public string? HashedChassisNumber { get; private set; }
    public string? EncryptedChassisNumber { get; private set; }
    public string? HashedEngineNumber { get; private set; }
    public string? EncryptedEngineNumber { get; private set; }
    public string? HashedInsuranceNumber { get; private set; }
    public string? EncryptedInsuranceNumber { get; private set; } 
    public bool? IsInsuranceValid { get; private set; } 
    public DateTime? LastInspectionDateUtc { get; private set; } 
    public int? OdometerReading { get; private set; } 
    public FuelType? FuelType { get; private set; } 
    public Owner Owner { get; private set; } 
    public UsageStatus? UsageStatus { get; private set; } 
    public OwnershipStatus? OwnershipStatus { get; private set; } 
    public TransmissionType? TransmissionType { get; private set; } 
    public DateTime? RegistrationDateUtc { get; private init; }


    public Fleet()
    {
        Brand = string.Empty; 
        Model = string.Empty; 
        ImageUrl = string.Empty;
        Owner = null!;
    }


    public Fleet( 
        Guid userId,
        string? hashedRegistrationNumber,
        string? encryptedRegistrationNumber,
        VehicleType? vehicleType,
        string brand,
        string model,
        int? yearOfManufacture,
        string? hashedChassisNumber,
        string? encryptedChassisNumber,
        string? hashedEngineNumber,
        string? encryptedEngineNumber,
        string? hashedInsuranceNumber,
        string? encryptedInsuranceNumber,
        bool? isInsuranceValid,
        DateTime? lastInspectionDateUtc,
        int? odometerReading,
        FuelType? fuelType, 
        string? encryptedOwnerName,
        string? encryptedOwnerAddress, 
        UsageStatus? usageStatus,
        OwnershipStatus? ownershipStatus,
        TransmissionType? transmissionType,
        string imageUrl)
    { 
        UserId = userId;
        if (LastInspectionDateUtc > DateTime.UtcNow)
        {
            throw new DomainException("Last inspection date must be in the past");
        }
   
        Brand = brand ?? throw new ArgumentNullException(nameof(brand));
        Model = model ?? throw new ArgumentNullException(nameof(model));
        ImageUrl = imageUrl ?? throw new ArgumentNullException(nameof(ImageUrl));

        HashedRegistrationNumber = string.IsNullOrEmpty(hashedRegistrationNumber) ? null : hashedRegistrationNumber;
        EncryptedRegistrationNumber = string.IsNullOrEmpty(encryptedRegistrationNumber) ? null : encryptedRegistrationNumber;
        VehicleType = vehicleType == null ? null : vehicleType;
        YearOfManufacture = yearOfManufacture == null ? null : yearOfManufacture;
        HashedChassisNumber = string.IsNullOrEmpty(hashedChassisNumber) ? null : hashedChassisNumber;
        EncryptedChassisNumber = string.IsNullOrEmpty(encryptedChassisNumber) ? null : encryptedChassisNumber;
        HashedEngineNumber = string.IsNullOrEmpty(hashedEngineNumber) ? null : hashedEngineNumber;
        EncryptedEngineNumber = string.IsNullOrEmpty(encryptedEngineNumber) ? null : encryptedEngineNumber;
        HashedInsuranceNumber = string.IsNullOrEmpty(hashedInsuranceNumber) ? null : hashedInsuranceNumber;
        EncryptedInsuranceNumber = string.IsNullOrEmpty(encryptedInsuranceNumber) ? null : encryptedInsuranceNumber;
        IsInsuranceValid = isInsuranceValid == null ? null : isInsuranceValid;
        LastInspectionDateUtc = lastInspectionDateUtc == null ? null : lastInspectionDateUtc;
        OdometerReading = odometerReading == null ? null : odometerReading;
        FuelType = fuelType == null ? null : fuelType; 
        UsageStatus = usageStatus == null ? null : usageStatus; 
        OwnershipStatus = ownershipStatus == null ? null : ownershipStatus;
        TransmissionType = TransmissionType == null ? null : transmissionType;
        RegistrationDateUtc = DateTime.UtcNow;
        Owner = new Owner(encryptedOwnerName, encryptedOwnerAddress);
       
    }
     
    public void Update(
        string? hashedRegistrationNumber,
        string? encryptedRegistrationNumber,
        VehicleType? vehicleType,
        string brand,
        string model,
        int? yearOfManufacture,
        string? hashedChassisNumber,
        string? encryptedChassisNumber,
        string? hashedEngineNumber,
        string? encryptedEngineNumber,
        string? hashedInsuranceNumber,
        string? encryptedInsuranceNumber,
        bool? isInsuranceValid,
        DateTime? lastInspectionDateUtc,
        int? odometerReading,
        FuelType? fuelType, 
        string? encryptedOwnerName,
        string? encryptedOwnerAddress,
        UsageStatus? usageStatus,
        OwnershipStatus? ownershipStatus,
        TransmissionType? transmissionType,
        string imageUrl)
    {
        if (LastInspectionDateUtc > DateTime.UtcNow)
        {
            throw new DomainException("Last inspection date must be in the past");
        }

        Brand = brand ?? throw new ArgumentNullException(nameof(brand));
        Model = model ?? throw new ArgumentNullException(nameof(model));
        ImageUrl = imageUrl ?? throw new ArgumentNullException(nameof(imageUrl));  

        HashedRegistrationNumber = string.IsNullOrEmpty(hashedRegistrationNumber) ? null : hashedRegistrationNumber;
        EncryptedRegistrationNumber = string.IsNullOrEmpty(encryptedRegistrationNumber) ? null : encryptedRegistrationNumber;
        VehicleType = vehicleType == null ? null : vehicleType;
        YearOfManufacture = yearOfManufacture == null ? null : yearOfManufacture;
        HashedChassisNumber = string.IsNullOrEmpty(hashedChassisNumber) ? null : hashedChassisNumber;
        EncryptedChassisNumber = string.IsNullOrEmpty(encryptedChassisNumber) ? null : encryptedChassisNumber;
        HashedEngineNumber = string.IsNullOrEmpty(hashedEngineNumber) ? null : hashedEngineNumber;
        EncryptedEngineNumber = string.IsNullOrEmpty(encryptedEngineNumber) ? null : encryptedEngineNumber;
        HashedInsuranceNumber = string.IsNullOrEmpty(hashedInsuranceNumber) ? null : hashedInsuranceNumber;
        EncryptedInsuranceNumber = string.IsNullOrEmpty(encryptedInsuranceNumber) ? null : encryptedInsuranceNumber;
        IsInsuranceValid = isInsuranceValid == null ? null : isInsuranceValid;
        LastInspectionDateUtc = lastInspectionDateUtc == null ? null : lastInspectionDateUtc;
        OdometerReading = odometerReading == null ? null : odometerReading;
        FuelType = fuelType == null ? null : fuelType;
        UsageStatus = usageStatus == null ? null : usageStatus;
        OwnershipStatus = ownershipStatus == null ? null : ownershipStatus;
        TransmissionType = TransmissionType == null ? null : transmissionType;
        Owner.Update(encryptedOwnerName, encryptedOwnerAddress);
    }

    public void AssignStaff(Guid businessUserId, Guid staffId)
    { 
        if (UserId is null)
        {
            throw new DomainException("Business user is not set for this fleet");
        }

        if (UserId != businessUserId)
        {
            throw new DomainException("Fleet can not assign to staff if the staff not belong to same business account");
        }

        StaffId = staffId;
    } 

    public void RemoveStaff()
    {
        StaffId = null;
    }
}
