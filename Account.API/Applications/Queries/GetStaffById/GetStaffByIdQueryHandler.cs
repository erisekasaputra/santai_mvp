using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Applications.Services;
using Core.Results;
using Account.Domain.SeedWork;
using MediatR;
using Core.Services.Interfaces;
using Core.CustomMessages;

namespace Account.API.Applications.Queries.GetStaffById;

public class GetStaffByIdQueryHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IEncryptionService kmsClient) : IRequestHandler<GetStaffByIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;
    private readonly IEncryptionService _kmsClient = kmsClient; 
    public async Task<Result> Handle(GetStaffByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var fleetIds = request.FleetsRequest?.Fleets;

            var staff = await _unitOfWork.Staffs.GetByIdAsync(request.StaffId);

            if (staff is null)
            {
                return Result.Failure($"User '{request.StaffId}' not found", ResponseStatus.NotFound);
            }

            var decryptedEmail = await DecryptNullableAsync(staff.EncryptedEmail);
            var decryptedPhoneNumber = await DecryptNullableAsync(staff.EncryptedPhoneNumber);

            var decryptedAddressLine1 = await DecryptAsync(staff.Address.EncryptedAddressLine1);
            var decryptedAddressLine2 = await DecryptNullableAsync(staff.Address.EncryptedAddressLine2);
            var decryptedAddressLine3 = await DecryptNullableAsync(staff.Address.EncryptedAddressLine3);

            List<FleetResponseDto> fleets = [];

            if (staff.Fleets is not null && staff.Fleets.Count > 0)
            {
                foreach (var fleet in staff.Fleets)
                {
                    if (fleetIds is not null && fleetIds.Any() && !fleetIds.Contains(fleet.Id))
                    {
                        continue;
                    }

                    var registrationNumber = await DecryptAsync(fleet.EncryptedRegistrationNumber);
                    var chassisNumber = await DecryptAsync(fleet.EncryptedChassisNumber);
                    var engineNumber = await DecryptAsync(fleet.EncryptedEngineNumber);
                    var insuranceNumber = await DecryptAsync(fleet.EncryptedInsuranceNumber);
                    var ownerName = await DecryptAsync(fleet.Owner.EncryptedOwnerName);
                    var ownerAddress = await DecryptAsync(fleet.Owner.EncryptedOwnerAddress);

                    fleets.Add(new FleetResponseDto(
                        fleet.Id,
                        registrationNumber,
                        fleet.VehicleType,
                        fleet.Brand,
                        fleet.Model,
                        fleet.YearOfManufacture,
                        chassisNumber,
                        engineNumber,
                        insuranceNumber,
                        fleet.IsInsuranceValid,
                        fleet.LastInspectionDateUtc,
                        fleet.OdometerReading,
                        fleet.FuelType,
                        ownerName,
                        ownerAddress,
                        fleet.UsageStatus,
                        fleet.OwnershipStatus,
                        fleet.TransmissionType,
                        fleet.ImageUrl));
                }
            }

            var addressDto = new AddressResponseDto(
                decryptedAddressLine1,
                decryptedAddressLine2,
                decryptedAddressLine3,
                staff.Address.City,
                staff.Address.State,
                staff.Address.PostalCode,
                staff.Address.Country);

            var staffDto = new StaffResponseDto(
                    staff.Id,
                    decryptedEmail,
                    decryptedPhoneNumber,
                    staff.Name,
                    addressDto,
                    staff.TimeZoneId,
                    fleets);

            return Result.Success(staffDto, ResponseStatus.Ok);
        }
        catch (Exception ex)
        {
            _service.Logger.LogError(ex.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }

    private async Task<string> DecryptAsync(string cipherText)
    {
        return await _kmsClient.DecryptAsync(cipherText);
    }

    private async Task<string?> DecryptNullableAsync(string? cipherText)
    {
        if (string.IsNullOrWhiteSpace(cipherText)) { return null; }

        return await _kmsClient.DecryptAsync(cipherText);
    }
}
