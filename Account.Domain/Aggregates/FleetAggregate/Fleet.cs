using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Enumerations;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;

namespace Account.Domain.Aggregates.FleetAggregate;


public class Fleet : Entity, IAggregateRoot
{
    public Guid? UserId { get; private set; }

    public BaseUser? BaseUser { get; private set; } 

    public Guid? StaffId { get; private set; }

    public Staff? Staff { get; private set; }

    public string HashedRegistrationNumber { get; private set; }

    public string EncryptedRegistrationNumber { get; private set; }

    public VehicleType VehicleType { get; private set; }

    public string Brand { get; private set; }

    public string Model { get; private set; }

    public int YearOfManufacture { get; private set; }

    public string HashedChassisNumber { get; private set; }

    public string EncryptedChassisNumber { get; private set; }

    public string HashedEngineNumber { get; private set; }

    public string EncryptedEngineNumber { get; private set; }

    public string HashedInsuranceNumber { get; private set; }

    public string EncryptedInsuranceNumber { get; private set; }

    public bool IsInsuranceValid { get; private set; }

    public DateTime LastInspectionDateUtc { get; private set; }

    public int OdometerReading { get; private set; }

    public FuelType FuelType { get; private set; } 

    public Owner Owner { get; private set; }

    public UsageStatus UsageStatus { get; private set; }

    public OwnershipStatus OwnershipStatus { get; private set; }

    public TransmissionType TransmissionType { get; private set; }

    public DateTime RegistrationDateUtc { get; private init; }

    public string? ImageUrl { get; private set; }

    public Fleet()
    {
        HashedRegistrationNumber = null!;
        EncryptedRegistrationNumber = null!;
        Brand = null!;
        Model = null!;
        HashedChassisNumber = null!;
        EncryptedChassisNumber = null!;
        HashedEngineNumber = null!;
        EncryptedEngineNumber = null!;
        HashedInsuranceNumber = null!;
        EncryptedInsuranceNumber = null!; 
        Owner = null!;
    }


    public Fleet( 
        Guid userId,
        string hashedRegistrationNumber,
        string encryptedRegistrationNumber,
        VehicleType vehicleType,
        string brand,
        string model,
        int yearOfManufacture,
        string hashedChassisNumber,
        string encryptedChassisNumber,
        string hashedEngineNumber,
        string encryptedEngineNumber,
        string hashedInsuranceNumber,
        string encryptedInsuranceNumber,
        bool isInsuranceValid,
        DateTime lastInspectionDateUtc,
        int odometerReading,
        FuelType fuelType, 
        string encryptedOwnerName,
        string encryptedOwnerAddress, 
        UsageStatus usageStatus,
        OwnershipStatus ownershipStatus,
        TransmissionType transmissionType,
        string? imageUrl)
    { 
        UserId = userId;
        if (LastInspectionDateUtc > DateTime.UtcNow)
        {
            throw new DomainException("Last inspection date must be in the past");
        }

        HashedRegistrationNumber = hashedRegistrationNumber ?? throw new ArgumentNullException(nameof(hashedRegistrationNumber));
        EncryptedRegistrationNumber = encryptedRegistrationNumber ?? throw new ArgumentNullException(nameof(encryptedRegistrationNumber));
        VehicleType = vehicleType;
        Brand = brand ?? throw new ArgumentNullException(nameof(brand));
        Model = model ?? throw new ArgumentNullException(nameof(model));
        YearOfManufacture = yearOfManufacture;
        HashedChassisNumber = hashedChassisNumber ?? throw new ArgumentNullException(nameof(hashedChassisNumber));
        EncryptedChassisNumber = encryptedChassisNumber ?? throw new ArgumentNullException(nameof(encryptedChassisNumber));
        HashedEngineNumber = hashedEngineNumber ?? throw new ArgumentNullException(nameof(hashedEngineNumber));
        EncryptedEngineNumber = encryptedEngineNumber ?? throw new ArgumentNullException(nameof(encryptedEngineNumber));
        HashedInsuranceNumber = hashedInsuranceNumber ?? throw new ArgumentNullException(nameof(hashedInsuranceNumber));
        EncryptedInsuranceNumber = encryptedInsuranceNumber ?? throw new ArgumentNullException(nameof(encryptedInsuranceNumber));
        IsInsuranceValid = isInsuranceValid;
        LastInspectionDateUtc = lastInspectionDateUtc;
        OdometerReading = odometerReading;
        FuelType = fuelType; 
        UsageStatus = usageStatus;
        OwnershipStatus = ownershipStatus;
        TransmissionType = transmissionType;
        RegistrationDateUtc = DateTime.UtcNow;
        Owner = new Owner(encryptedOwnerName, encryptedOwnerAddress);
        ImageUrl = imageUrl;
    }
     
    public void Update(
        string hashedRegistrationNumber,
        string encryptedRegistrationNumber,
        VehicleType vehicleType,
        string brand,
        string model,
        int yearOfManufacture,
        string hashedChassisNumber,
        string encryptedChassisNumber,
        string hashedEngineNumber,
        string encryptedEngineNumber,
        string hashedInsuranceNumber,
        string encryptedInsuranceNumber,
        bool isInsuranceValid,
        DateTime lastInspectionDateUtc,
        int odometerReading,
        FuelType fuelType, 
        string encryptedOwnerName,
        string encryptedOwnerAddress,
        UsageStatus usageStatus,
        OwnershipStatus ownershipStatus,
        TransmissionType transmissionType,
        string? imageUrl)
    {
        if (LastInspectionDateUtc > DateTime.UtcNow)
        {
            throw new DomainException("Last inspection date must be in the past");
        }

        HashedRegistrationNumber = hashedRegistrationNumber ?? throw new ArgumentNullException(nameof(hashedRegistrationNumber));
        EncryptedRegistrationNumber = encryptedRegistrationNumber ?? throw new ArgumentNullException(nameof(encryptedRegistrationNumber));
        VehicleType = vehicleType;
        Brand = brand ?? throw new ArgumentNullException(nameof(brand));
        Model = model ?? throw new ArgumentNullException(nameof(model));
        YearOfManufacture = yearOfManufacture;
        HashedChassisNumber = hashedChassisNumber ?? throw new ArgumentNullException(nameof(hashedChassisNumber));
        EncryptedChassisNumber = encryptedChassisNumber ?? throw new ArgumentNullException(nameof(encryptedChassisNumber));
        HashedEngineNumber = hashedEngineNumber ?? throw new ArgumentNullException(nameof(hashedEngineNumber));
        EncryptedEngineNumber = encryptedEngineNumber ?? throw new ArgumentNullException(nameof(encryptedEngineNumber));
        HashedInsuranceNumber = hashedInsuranceNumber ?? throw new ArgumentNullException(nameof(hashedInsuranceNumber));
        EncryptedInsuranceNumber = encryptedInsuranceNumber ?? throw new ArgumentNullException(nameof(encryptedInsuranceNumber));
        IsInsuranceValid = isInsuranceValid;
        LastInspectionDateUtc = lastInspectionDateUtc;
        OdometerReading = odometerReading;
        FuelType = fuelType; 
        UsageStatus = usageStatus;
        OwnershipStatus = ownershipStatus;
        TransmissionType = transmissionType; 
        Owner = new Owner(encryptedOwnerName 
            ?? throw new ArgumentNullException(nameof(encryptedOwnerName)), encryptedOwnerAddress 
            ?? throw new ArgumentNullException(nameof(encryptedOwnerAddress)));
        ImageUrl = imageUrl;
    }

    public void AssignStaff(Staff staff)
    {
        if (staff is null)
        {
            throw new DomainException("Staff must not be empty");
        }

        if (staff.BusinessUserId != UserId)
        {
            throw new DomainException("The staff member must belong to the same business user as the origin");
        } 

        Staff = staff;
        StaffId = staff.Id;
    } 
}
