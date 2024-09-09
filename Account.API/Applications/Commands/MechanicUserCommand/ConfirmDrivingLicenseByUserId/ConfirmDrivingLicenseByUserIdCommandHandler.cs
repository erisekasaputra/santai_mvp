using Account.API.Applications.Services; 
using Core.Results;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;
using Microsoft.Extensions.Options;
using Core.Messages;
using Core.Services.Interfaces;

namespace Account.API.Applications.Commands.MechanicUserCommand.ConfirmDrivingLicenseByUserId;

public class ConfirmDrivingLicenseByUserIdCommandHandler : IRequestHandler<ConfirmDrivingLicenseByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork; 
    private readonly ApplicationService _service;
    private readonly IEncryptionService _kmsClient;
    private readonly IHashService _hashService;

    public ConfirmDrivingLicenseByUserIdCommandHandler(
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

    public async Task<Result> Handle(ConfirmDrivingLicenseByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        { 
            var license = await _unitOfWork.DrivingLicenses.GetByUserIdAndIdAsync(request.UserId, request.DrivingLicenseId);

            if (license is null)
            {
                return Result.Failure($"Driving license not found", ResponseStatus.NotFound)
                    .WithError(new("DrivingLicense.Id", "Driving license not found"));
            }
             
            var accepted = await _unitOfWork.DrivingLicenses.GetAcceptedByUserIdAsync(request.UserId);

            if (accepted is not null && accepted.Id == request.DrivingLicenseId)
            {
                return Result.Failure($"Driving license already accepted", ResponseStatus.Conflict)
                    .WithError(new("DrivingLicense.Id", "Conflict driving license"));
            }

            if (accepted is not null)
            {
                return Result.Failure($"Can only have one 'Accepted' driving license for a user", ResponseStatus.Conflict)
                    .WithError(new("DrivingLicense.Id", "Conflict driving license"));
            }

            license.VerifyDocument();

            _unitOfWork.DrivingLicenses.Update(license);

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
