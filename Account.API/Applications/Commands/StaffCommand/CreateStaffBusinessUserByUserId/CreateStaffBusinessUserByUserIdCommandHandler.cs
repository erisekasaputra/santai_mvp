using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Applications.Services;
using Core.Results;
using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Enumerations;
using Account.Domain.SeedWork;
using Account.Domain.ValueObjects;
using MediatR;
using Core.Services.Interfaces;
using Core.Exceptions;
using Core.CustomMessages;

namespace Account.API.Applications.Commands.StaffCommand.CreateStaffBusinessUserByUserId;

public class CreateStaffBusinessUserByUserIdCommandHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IEncryptionService kmsClient,
    IHashService hashService) : IRequestHandler<CreateStaffBusinessUserByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _appService = service;
    private readonly IEncryptionService _kmsClient = kmsClient;
    private readonly IHashService _hashClient = hashService;

    public async Task<Result> Handle(CreateStaffBusinessUserByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _unitOfWork.BaseUsers.GetBusinessUserByIdAsync(request.Id);
            if (entity is null)
            {
                return Result.Failure($"Business user not found", ResponseStatus.NotFound)
                    .WithError(new("BusinessUser.Id", "User not found"));
            } 

            var addressRequest = request.Address;
             
            var hashedPhoneNumber = await HashAsync(request.PhoneNumber);
             
            var encryptedPhoneNumber = await EncryptAsync(request.PhoneNumber);

            var encryptedAddressLine1 = await EncryptAsync(addressRequest.AddressLine1);
            var encryptedAddressLine2 = await EncryptNullableAsync(addressRequest.AddressLine2);
            var encryptedAddressLine3 = await EncryptNullableAsync(addressRequest.AddressLine3);

            var address = new Address(
                encryptedAddressLine1,
                encryptedAddressLine2,
                encryptedAddressLine3,
                addressRequest.City,
                addressRequest.State,
                addressRequest.PostalCode,
                addressRequest.Country);

            var conflicts = await _unitOfWork.Staffs.GetByIdentitiesAsNoTrackingAsync( 
                    (IdentityParameter.PhoneNumber, [hashedPhoneNumber]));

            if (conflicts is not null && conflicts.Any())
            {
                var errorDetails = new List<ErrorDetail>();
                var conflict = conflicts.First();  

                if (conflict.HashedPhoneNumber == hashedPhoneNumber || conflict.NewHashedPhoneNumber == hashedPhoneNumber)
                {
                    errorDetails.Add(new ("Staff.PhoneNumber", "Phone number already registered"));
                }

                var message = errorDetails.Count switch
                {
                    <= 1 => "There is a conflict",
                    _ => $"There are {errorDetails.Count} conflicts"
                };

                return Result.Failure(message, ResponseStatus.BadRequest).WithErrors(errorDetails);
            }


            var newStaff = new Staff(
                entity.Id,
                entity.Code, 
                hashedPhoneNumber,
                encryptedPhoneNumber,
                request.Name,
                address,
                request.TimeZoneId,
                request.Password,
                raiseCreatedEvent: true);

            await _unitOfWork.Staffs.CreateAsync(newStaff);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
         
            return Result.Success(ToStaffResponseDto(newStaff, request), ResponseStatus.Created);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);

        }
        catch (Exception ex)
        {
            _appService.Logger.LogError(ex, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }

    private static StaffResponseDto ToStaffResponseDto(
        Staff staff,
        CreateStaffBusinessUserByUserIdCommand request)
    {
        var addressResponseDto = new AddressResponseDto(
            request.Address.AddressLine1,
            request.Address.AddressLine2,
            request.Address.AddressLine3,
            request.Address.City,
            request.Address.State,
            request.Address.PostalCode,
            request.Address.Country); 

        var staffResponseDto = new StaffResponseDto(
                staff.Id,   
                null,
                request.PhoneNumber,
                request.Name,
                addressResponseDto,
                request.TimeZoneId,
                []
            );

        return staffResponseDto;
    }

    private async Task<string?> EncryptNullableAsync(string? value)
    {
        if (value == null) return null;

        return await _kmsClient.EncryptAsync(value);
    }

    private async Task<string> EncryptAsync(string value)
    {
        return await _kmsClient.EncryptAsync(value);
    }

    private async Task<string> HashAsync(string value)
    {
        return await _hashClient.Hash(value);
    }

    private async Task<string?> HashNullableAsync(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return await _hashClient.Hash(value);
    }
}
