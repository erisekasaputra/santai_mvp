using Account.API.Extensions; 
using Account.API.SeedWork;
using Account.API.Services;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using Account.Domain.ValueObjects;
using MediatR;

namespace Account.API.Applications.Commands.BusinessUserCommand.UpdateBusinessUserByUserId;

public class UpdateBusinessUserByUserIdCommandHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IKeyManagementService kmsClient) : IRequestHandler<UpdateBusinessUserByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;
    private readonly IKeyManagementService _kmsClient = kmsClient;
    public async Task<Result> Handle(UpdateBusinessUserByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.Users.GetBusinessUserByIdAsync(request.Id);
            if (user is null)
            {
                return Result.Failure($"Business '{request.Id}' not found", ResponseStatus.NotFound);
            }

            var encryptedAddressLine1 = await EncryptAsync(request.Address.AddressLine1);
            var encryptedAddressLine2 = await EncryptNullableAsync(request.Address.AddressLine2);
            var encryptedAddressLine3 = await EncryptNullableAsync(request.Address.AddressLine3);
            var encryptedContactPerson = await EncryptAsync(request.ContactPerson);
            var encryptedTaxId = await EncryptNullableAsync(request.TaxId);

            var address = new Address(
                    encryptedAddressLine1,
                    encryptedAddressLine2,
                    encryptedAddressLine3,
                    request.Address.City,
                    request.Address.State,
                    request.Address.PostalCode,
                    request.Address.Country
                );

            user.Update(
                request.BusinessName,
                encryptedContactPerson,
                encryptedTaxId,
                request.WebsiteUrl,
                request.Description,
                address,
                request.TimeZoneId);

            _unitOfWork.Users.Update(user);

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
}
