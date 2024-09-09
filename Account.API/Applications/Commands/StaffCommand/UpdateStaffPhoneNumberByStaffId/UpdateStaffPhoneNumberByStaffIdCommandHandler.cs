using Account.API.Applications.Services; 
using Core.Results;
using Core.Messages;
using Account.Domain.Enumerations;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;
using Core.Services.Interfaces;

namespace Account.API.Applications.Commands.StaffCommand.UpdateStaffPhoneNumberByStaffId;

public class UpdateStaffPhoneNumberByStaffIdCommandHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IEncryptionService kmsClient,
    IHashService hashService) : IRequestHandler<UpdateStaffPhoneNumberByStaffIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;
    private readonly IEncryptionService _kmsClient = kmsClient;
    private readonly IHashService _hashClient = hashService;

    public async Task<Result> Handle(UpdateStaffPhoneNumberByStaffIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var staff = await _unitOfWork.Staffs.GetByIdAsync(request.StaffId);
            
            if (staff is null)
            {
                return Result.Failure($"Staff not found", ResponseStatus.NotFound)
                    .WithError(new("Staff.Id", "User not found"));
            }

            var hashedPhoneNumber = await _hashClient.Hash(request.PhoneNumber);
            var encryptedPhoneNumber = await _kmsClient.EncryptAsync(request.PhoneNumber);

            var conflict = await _unitOfWork.Staffs
                .GetAnyByIdentitiesExcludingIdsAsNoTrackingAsync((IdentityParameter.PhoneNumber, [(staff.Id, hashedPhoneNumber)]));

            if (conflict)
            {
                return Result.Failure($"Phone number already registered", ResponseStatus.Conflict)
                    .WithError(new ErrorDetail("Staff.PhoneNumber", "Phone number already registered"));
            }

            staff.UpdatePhoneNumber(hashedPhoneNumber, encryptedPhoneNumber);

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
}
