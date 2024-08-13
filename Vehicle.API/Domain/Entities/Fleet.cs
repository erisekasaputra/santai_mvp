using Vehicle.API.Domain.Enumerations;

namespace Vehicle.API.Domain.Entities;

public class Fleet
{ 
    public int Id { get; private set; }
     
    public string HashedRegistrationNumber { get; private set; }  

    public string EncryptedRegistrationNumber { get; private set; }  
     
    public VehicleType VehicleType { get; private set; } 
     
    public string Make { get; private set; }  
     
    public string Model { get; private set; }  
     
    public int YearOfManufacture { get; private set; } 
     
    public string HashedChassisNumber { get; private set; }  
    public string EncryptedChassisNumber { get; private set; }  

    public string HashedEngineNumber { get; private set; }
    public string EncryptedEngineNumber { get; private set; }
     
    public bool IsRoadTaxValid { get; private set; }  
     
    public bool IsInsuranceValid { get; private set; } 

    public DateTime LastInspectionDateUtc { get; private set; }
     
    public int OdometerReading { get; private set; } 
     
    public FuelType FuelType { get; private set; }  
     
    public string Color { get; private set; } 
     
    public Owner Owner { get; private set; }
     
    public UsageStatus UsageStatus { get; private set; }  
     
    public OwnershipStatus OwnershipStatus { get; private set; } 
     
    public TransmissionType TransmissionType { get; private set; }   
     
    public DateTime RegistrationDateUtc { get; private set; }  

    public Fleet(
        string hashedRegistrationNumber,
        string encryptedRegistrationNumber,
        VehicleType vehicleType,
        string make,
        string model,
        int yearOfManufacture,
        string hashedChassisNumber,
        string encryptedChassisNumber,
        string hashedEngineNumber,
        string encryptedEngineNumber,
        bool isRoadTaxValid,
        bool isInsuranceValid,
        DateTime lastInspectionDateUtc,
        int odometerReading,
        FuelType fuelType,
        string color,
        string encryptedOwnerName,
        string encryptedOwnerAddress,
        string encryptedOwnerPhoneNumber,
        UsageStatus usageStatus,
        OwnershipStatus ownershipStatus,
        TransmissionType transmissionType,
        DateTime registrationDateUtc)
    {
        HashedRegistrationNumber = hashedRegistrationNumber;
        EncryptedRegistrationNumber = encryptedRegistrationNumber;
        VehicleType = vehicleType;
        Make = make;
        Model = model;
        YearOfManufacture = yearOfManufacture;
        HashedChassisNumber = hashedChassisNumber;
        EncryptedChassisNumber = encryptedChassisNumber;
        HashedEngineNumber = hashedEngineNumber;
        EncryptedEngineNumber = encryptedEngineNumber;
        IsRoadTaxValid = isRoadTaxValid;
        IsInsuranceValid = isInsuranceValid;
        LastInspectionDateUtc = lastInspectionDateUtc;
        OdometerReading = odometerReading;
        FuelType = fuelType;
        Color = color; 
        UsageStatus = usageStatus;
        OwnershipStatus = ownershipStatus;
        TransmissionType = transmissionType;
        RegistrationDateUtc = registrationDateUtc;
        Owner = new Owner(encryptedOwnerName, encryptedOwnerAddress, encryptedOwnerPhoneNumber); 
    }
}
