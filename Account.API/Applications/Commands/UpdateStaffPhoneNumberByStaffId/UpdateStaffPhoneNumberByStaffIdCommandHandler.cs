using Account.API.Applications.Services;
using Account.API.Extensions;
using Account.API.SeedWork;
using Account.Domain.Enumerations;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.UpdateStaffPhoneNumberByStaffId;

public class UpdateStaffPhoneNumberByStaffIdCommandHandler(IUnitOfWork unitOfWork, AppService service) : IRequestHandler<UpdateStaffPhoneNumberByStaffIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly AppService _service = service;
    public async Task<Result> Handle(UpdateStaffPhoneNumberByStaffIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var staff = await _unitOfWork.Staffs.GetByBusinessUserIdAndStaffIdAsync(request.BusinessUserId, request.StaffId);
            if (staff is null)
            {
                return Result.Failure($"User '{request.StaffId}' not found", ResponseStatus.NotFound);
            }

            var conflict = await _unitOfWork.Staffs.GetAnyByIdentitiesExcludingIdsAsNoTrackingAsync((IdentityParameter.PhoneNumber, [(staff.Id, request.PhoneNumber)]));
            if (conflict)
            {
                return Result.Failure($"Phone Number '{request.PhoneNumber}' already registered", ResponseStatus.Conflict);
            }

            staff.UpdatePhoneNumber(request.PhoneNumber);
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
