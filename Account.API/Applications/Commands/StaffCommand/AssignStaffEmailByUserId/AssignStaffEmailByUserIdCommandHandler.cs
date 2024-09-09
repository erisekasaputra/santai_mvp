using Account.API.Applications.Services; 
using Core.Results;
using Core.Messages;
using Account.Domain.Enumerations;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;
using Core.Services.Interfaces;

namespace Account.API.Applications.Commands.StaffCommand.AssignStaffEmailByUserId;

public class AssignStaffEmailByUserIdCommandHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IEncryptionService kmsClient,
    IHashService hashService) : IRequestHandler<AssignStaffEmailByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;
    private readonly IEncryptionService _kmsClient = kmsClient;
    private readonly IHashService _hashService = hashService;

    public async Task<Result> Handle(AssignStaffEmailByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var staff = await _unitOfWork.Staffs.GetByIdAsync(request.UserId);

            if (staff is null)
            {
                return Result.Failure($"Staff not found", ResponseStatus.NotFound)
                    .WithError(new("Staff.Id", "User not found"));
            }

            var hashedEmail = await HashAsync(request.Email); 
            var encryptedEmail = await EncryptAsync(request.Email);

            var conflict = await _unitOfWork.Staffs.GetAnyByIdentitiesExcludingIdsAsNoTrackingAsync((IdentityParameter.Email, [(staff.Id, hashedEmail)]));
            if (conflict)
            {
                return Result.Failure($"Email already registered", ResponseStatus.Conflict)
                    .WithError(new ErrorDetail("Staff.Email", "Email already registered"));
            }

            staff.UpdateEmail(hashedEmail, encryptedEmail);

            staff.VerifyEmail();

            _unitOfWork.Staffs.Update(staff);

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

    private async Task<string?> HashNullableAsync(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return await _hashService.Hash(value);
    }
}
