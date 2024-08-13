using Account.API.Extensions;
using Account.API.Mapper;
using Account.API.Options;
using Account.API.SeedWork;
using Account.API.Services;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using Account.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Options;

namespace Account.API.Applications.Commands.MechanicUserCommand.UpdateMechanicUserByUserId;

public class UpdateMechanicUserByUserIdCommandHandler : IRequestHandler<UpdateMechanicUserByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOptionsMonitor<ReferralProgramOption> _referralOptions;
    private readonly ApplicationService _service;
    private readonly IKeyManagementService _kmsClient;
    private readonly IHashService _hashService;

    public UpdateMechanicUserByUserIdCommandHandler(
        IUnitOfWork unitOfWork,
        IOptionsMonitor<ReferralProgramOption> referralOptions,
        ApplicationService service,
        IKeyManagementService kmsClient,
        IHashService hashService)
    {
        _unitOfWork = unitOfWork;
        _referralOptions = referralOptions;
        _service = service;
        _kmsClient = kmsClient;
        _hashService = hashService;
    }

    public async Task<Result> Handle(UpdateMechanicUserByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var mechanicUser = await _unitOfWork.Users.GetMechanicUserByIdAsync(request.UserId);

            if (mechanicUser is null)
            {
                return Result.Failure($"Mechanic user '{request.UserId}' not found", ResponseStatus.NotFound);
            }  

            var address = new Address(
                    await EncryptAsync(request.Address.AddressLine1),
                    await EncryptNullableAsync(request.Address.AddressLine2),
                    await EncryptNullableAsync(request.Address.AddressLine3),
                    request.Address.City,
                    request.Address.State,
                    request.Address.PostalCode,
                    request.Address.Country
                );

            mechanicUser.Update(
                request.personalInfo.ToPersonalInfo(mechanicUser.TimeZoneId),
                address,
                request.TimeZoneId);

            _unitOfWork.Users.Update(mechanicUser);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(null, ResponseStatus.NoContent);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (Exception ex)
        {
            _service.Logger.LogError(ex.Message, ex.InnerException?.Message);
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
