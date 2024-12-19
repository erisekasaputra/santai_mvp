using Core.Results;
using Account.Domain.Enumerations;
using MediatR;

namespace Account.API.Applications.Commands.FleetCommand.UpdateFleetByUserId;

public record UpdateFleetByUserIdCommand(
    Guid UserId, 
    Guid FleetId,
    string? RegistrationNumber,
    VehicleType? VehicleType,
    string Brand,
    string Model,
    int? YearOfManufacture,
    string? ChassisNumber,
    string? EngineNumber, 
    string? InsuranceNumber,
    bool? IsInsuranceValid,
    DateTime? LastInspectionDateLocal,
    int? OdometerReading,
    FuelType? FuelType, 
    string? OwnerName,
    string? OwnerAddress,
    UsageStatus? UsageStatus,
    OwnershipStatus? OwnershipStatus,
    TransmissionType? TransmissionType,
    string ImageUrl) : IRequest<Result>;
