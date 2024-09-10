using Account.API.Applications.Services; 
using Core.Results;
using Core.Messages;
using Account.Domain.Enumerations; 
using Account.Domain.SeedWork;
using MediatR;
using Core.Services.Interfaces;
using Core.Exceptions;

namespace Account.API.Applications.Commands.StaffCommand.UpdateStaffEmailByStaffId;

public class UpdateStaffEmailByStaffIdCommandHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IEncryptionService kmsClient,
    IHashService hashService) : IRequestHandler<UpdateStaffEmailByStaffIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;
    private readonly IEncryptionService _kmsClient = kmsClient;
    private readonly IHashService _hashClient = hashService;

    public async Task<Result> Handle(UpdateStaffEmailByStaffIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var staff = await _unitOfWork.Staffs.GetByIdAsync(request.StaffId);
             
            if (staff is null)
            {
                return Result.Failure($"Staff not found", ResponseStatus.NotFound)
                    .WithError(new("Staff.Id", "User not found"));
            }

            var hashedEmail = await _hashClient.Hash(request.Email);

            var encryptedEmail = await _kmsClient.EncryptAsync(request.Email);

            var conflict = await _unitOfWork.Staffs.GetAnyByIdentitiesExcludingIdsAsNoTrackingAsync((IdentityParameter.Email, [(staff.Id, hashedEmail)]));
            if (conflict)
            {
                return Result.Failure($"Email already registered", ResponseStatus.Conflict)
                    .WithError(new ErrorDetail("Staff.Email", "Email already registered"));
            }

            staff.UpdateEmail(hashedEmail, encryptedEmail);

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
