using Account.API.Extensions;
using Account.API.SeedWork;
using Account.API.Services;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.StaffCommand.ResetDeviceIdByStaffId;

public class ResetDeviceIdByStaffIdCommandHandler(IUnitOfWork unitOfWork, ApplicationService service) : IRequestHandler<ResetDeviceIdByStaffIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;
    public async Task<Result> Handle(ResetDeviceIdByStaffIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var staff = await _unitOfWork.Staffs.GetByBusinessUserIdAndStaffIdAsync(request.UserId, request.StaffId);
            if (staff is null)
            {
                return Result.Failure($"Staff '{request.StaffId}' not found", ResponseStatus.NotFound);
            }

            staff.ResetDeviceId();

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
