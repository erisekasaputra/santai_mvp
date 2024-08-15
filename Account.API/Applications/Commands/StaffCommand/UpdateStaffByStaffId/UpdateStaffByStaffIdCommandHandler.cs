using Account.API.Mapper;
using Account.API.SeedWork;
using Account.API.Services;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using Account.Domain.ValueObjects;
using MediatR;

namespace Account.API.Applications.Commands.StaffCommand.UpdateStaffByStaffId;

public class UpdateStaffByStaffIdCommandHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IKeyManagementService kmsClient) : IRequestHandler<UpdateStaffByStaffIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;
    private readonly IKeyManagementService _kmsClient = kmsClient;
    public async Task<Result> Handle(UpdateStaffByStaffIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var staff = await _unitOfWork.Staffs.GetByBusinessUserIdAndStaffIdAsync(request.UserId, request.StaffId);
            if (staff is null)
            {
                return Result.Failure($"Staff not found", ResponseStatus.NotFound)
                    .WithError(new("Staff.Id", "User not found"));
            }


            var encryptedAddressLine1 = await EncryptAsync(request.Address.AddressLine1);
            var encryptedAddressLine2 = await EncryptNullableAsync(request.Address.AddressLine2);
            var encryptedAddressLine3 = await EncryptNullableAsync(request.Address.AddressLine3);

            var address = new Address(
                    encryptedAddressLine1,
                    encryptedAddressLine2,
                    encryptedAddressLine3,
                    request.Address.City,
                    request.Address.State,
                    request.Address.PostalCode,
                    request.Address.Country
                );


            staff.Update(request.Name, address, request.TimeZoneId);

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
