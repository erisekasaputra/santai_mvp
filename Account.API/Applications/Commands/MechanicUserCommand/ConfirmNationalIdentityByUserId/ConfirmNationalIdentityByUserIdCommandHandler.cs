using Account.API.Options;
using Account.API.SeedWork;
using Account.API.Services;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;
using Microsoft.Extensions.Options;

namespace Account.API.Applications.Commands.MechanicUserCommand.ConfirmNationalIdentityByUserId;

public class ConfirmNationalIdentityByUserIdCommandHandler : IRequestHandler<ConfirmNationalIdentityByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOptionsMonitor<ReferralProgramOption> _referralOptions;
    private readonly ApplicationService _service;
    private readonly IKeyManagementService _kmsClient;
    private readonly IHashService _hashService;

    public ConfirmNationalIdentityByUserIdCommandHandler(
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

    public async Task<Result> Handle(ConfirmNationalIdentityByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var license = await _unitOfWork.NationalIdentities.GetByUserIdAndIdAsync(request.UserId, request.NationalIdentityId);

            if (license is null)
            {
                return Result.Failure($"National identity not found", ResponseStatus.NotFound)
                    .WithError(new("NationalIdentity.Id", "National identity not found"));
            }

            var accepted = await _unitOfWork.NationalIdentities.GetAcceptedByUserIdAsync(request.UserId);

            if (accepted is not null && accepted.Id == request.NationalIdentityId)
            {
                return Result.Failure($"National identity already accepted", ResponseStatus.Conflict)
                    .WithError(new("NationalIdentity.Id", "Conflict national identity")); ;
            }

            if (accepted is not null)
            {
                return Result.Failure("Can only have one 'Accepted' national identity for a user", ResponseStatus.Conflict)
                    .WithError(new("NationalIdentity.Id", "Conflict national identity"));
            }

            license.VerifyDocument();

            _unitOfWork.NationalIdentities.Update(license);

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
