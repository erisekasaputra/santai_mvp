using Account.API.Applications.Services; 
using Core.Results;
using Core.Messages;
using Account.Domain.Aggregates.DrivingLicenseAggregate;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;
using Core.Services.Interfaces;

namespace Account.API.Applications.Commands.MechanicUserCommand.SetDrivingLicenseByUserId;

public class SetDrivingLicenseByUserIdCommandHandler : IRequestHandler<SetDrivingLicenseByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork; 
    private readonly ApplicationService _service;
    private readonly IEncryptionService _kmsClient;
    private readonly IHashService _hashService;

    public SetDrivingLicenseByUserIdCommandHandler(
        IUnitOfWork unitOfWork, 
        ApplicationService service,
        IEncryptionService kmsClient,
        IHashService hashService)
    {
        _unitOfWork = unitOfWork; 
        _service = service;
        _kmsClient = kmsClient;
        _hashService = hashService;
    }

    public async Task<Result> Handle(SetDrivingLicenseByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        { 
            var mechanicUser = await _unitOfWork.BaseUsers.GetMechanicUserByIdAsync(request.UserId);

            if (mechanicUser is null)
            {
                return Result.Failure($"Mechanic user not found", ResponseStatus.NotFound)
                     .WithError(new("MechanicUser.Id", "Mechanic user not found"));
            }  

            var accepted = await _unitOfWork.DrivingLicenses.GetAcceptedByUserIdAsync(request.UserId);

            if (accepted is not null && accepted.UserId == request.UserId)
            {
                return Result.Failure($"Driving license already accepted", ResponseStatus.Conflict)
                     .WithError(new("DrivingLicense.Id", "Conflict driving license"));
            }

            if (accepted is not null)
            {
                return Result.Failure($"Can only have one 'Accepted' driving license for a user", ResponseStatus.Conflict)
                     .WithError(new("DrivingLicense.Id", "Conflict driving license"));
            }

            var hashedLicenseNumber = await HashAsync(request.LicenseNumber);
            var encryptedLicenseNumber = await EncryptAsync(request.LicenseNumber); 

            var registeredToOtherUser = await _unitOfWork.DrivingLicenses.GetAnyByLicenseNumberExcludingUserIdAsync(request.UserId, hashedLicenseNumber);
            
            if (registeredToOtherUser)
            { 
                return Result.Failure($"Driving license number already used by other user", ResponseStatus.Conflict)
                     .WithError(new("DrivingLicense.Id", "Driving license conflict"));
            }

            var drivingLicense = new DrivingLicense(
                request.UserId,
                hashedLicenseNumber,
                encryptedLicenseNumber,
                request.FrontSideImageUrl,
                request.BackSideImageUrl);

            await _unitOfWork.DrivingLicenses.CreateAsync(drivingLicense);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(null, ResponseStatus.NoContent);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (Exception ex)
        {
            _service.Logger.LogError(ex, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }

    private async Task<string?> EncryptNullableAsync(string? plaintext)
    {
        if (string.IsNullOrEmpty(plaintext))
            return null;

        return await _kmsClient.EncryptAsync(plaintext);
    }

    private async Task<string> EncryptAsync(string plaintext)
    {
        return await _kmsClient.EncryptAsync(plaintext);
    }

    private async Task<string> HashAsync(string plainText)
    {
        return await _hashService.Hash(plainText);
    }
}
