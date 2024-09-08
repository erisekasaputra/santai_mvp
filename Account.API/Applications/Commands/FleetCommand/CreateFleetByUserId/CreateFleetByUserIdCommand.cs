using Core.Results;
using Account.Domain.Enumerations;
using MediatR;

namespace Account.API.Applications.Commands.FleetCommand.CreateFleetByUserId;

public record CreateFleetByUserIdCommand(
    Guid UserId, 
    string RegistrationNumber,
    VehicleType VehicleType,
    string Brand,
    string Model,
    int YearOfManufacture,
    string ChassisNumber,
    string EngineNumber,
    string InsuranceNumber,
    bool IsInsuranceValid,
    DateTime LastInspectionDateLocal,
    int OdometerReading,
    FuelType FuelType, 
    string OwnerName,
    string OwnerAddress,
    UsageStatus UsageStatus,
    OwnershipStatus OwnershipStatus,
    TransmissionType TransmissionType,
    string? ImageUrl) : IRequest<Result>;