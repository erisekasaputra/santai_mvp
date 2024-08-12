using Account.API.Extensions;
using Account.API.SeedWork;
using Account.API.Services;
using Account.Domain.Enumerations;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.StaffCommand.UpdateStaffEmailByStaffId;

public class UpdateStaffEmailByStaffIdCommandHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IKeyManagementService kmsClient,
    IHashService hashService) : IRequestHandler<UpdateStaffEmailByStaffIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;
    private readonly IKeyManagementService _kmsClient = kmsClient;
    private readonly IHashService _hashClient = hashService;

    public async Task<Result> Handle(UpdateStaffEmailByStaffIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var staff = await _unitOfWork.Staffs.GetByBusinessUserIdAndStaffIdAsync(request.BusinessUserId, request.StaffId);
             
            if (staff is null)
            {
                return Result.Failure($"User '{request.StaffId}' not found", ResponseStatus.NotFound);
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
            _service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }
}
