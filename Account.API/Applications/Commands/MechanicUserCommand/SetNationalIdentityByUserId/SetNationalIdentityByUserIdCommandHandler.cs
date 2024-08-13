using Account.API.Extensions;
using Account.API.Options;
using Account.API.SeedWork;
using Account.API.Services;
using Account.Domain.Aggregates.NationalIdentityAggregate;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;
using Microsoft.Extensions.Options;

namespace Account.API.Applications.Commands.MechanicUserCommand.SetNationalIdentityByUserId;

public class SetNationalIdentityByUserIdCommandHandler : IRequestHandler<SetNationalIdentityByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOptionsMonitor<ReferralProgramOption> _referralOptions;
    private readonly ApplicationService _service;
    private readonly IKeyManagementService _kmsClient;
    private readonly IHashService _hashService;

    public SetNationalIdentityByUserIdCommandHandler(
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

    public async Task<Result> Handle(SetNationalIdentityByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var mechanicUser = await _unitOfWork.Users.GetMechanicUserByIdAsync(request.UserId);

            if (mechanicUser is null)
            {
                return Result.Failure($"Mechanic user '{request.UserId}' not found", ResponseStatus.NotFound);
            }


            var accepted = await _unitOfWork.NationalIdentities.GetAcceptedByUserIdAsync(request.UserId);

            if (accepted is not null && accepted.UserId == request.UserId)
            {
                return Result.Failure($"National identity '{accepted.Id}' already accepted", ResponseStatus.Conflict);
            }

            if (accepted is not null)
            {
                return Result.Failure($"Can only have one 'Accepted' national identity for a user", ResponseStatus.Conflict);
            }


            var hashedIdentityNumber = await HashAsync(request.IdentityNumber);
            var encryptedIdentityNumber = await EncryptAsync(request.IdentityNumber);
             

            var registeredToOtherUser = await _unitOfWork.NationalIdentities.GetAnyByIdentityNumberExcludingUserIdAsync(request.UserId, hashedIdentityNumber);

            if (registeredToOtherUser)
            {
                return Result.Failure($"National identity number already used by other user", ResponseStatus.Conflict);
            } 

            var nationalIdentity = new NationalIdentity(
                mechanicUser.Id,
                hashedIdentityNumber,
                encryptedIdentityNumber,
                request.FrontSideImageUrl,
                request.BackSideImageUrl);

            await _unitOfWork.NationalIdentities.CreateAsync(nationalIdentity);

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
