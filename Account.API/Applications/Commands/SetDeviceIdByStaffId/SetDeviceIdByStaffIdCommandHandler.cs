using Account.API.Applications.Services;
using Account.API.Extensions;
using Account.API.SeedWork;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.SetDeviceIdByStaffId;

public class SetDeviceIdByStaffIdCommandHandler(IUnitOfWork unitOfWork, AppService service) : IRequestHandler<SetDeviceIdByStaffIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly AppService _service = service;
    public async Task<Result> Handle(SetDeviceIdByStaffIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var staff = await _unitOfWork.Staffs.GetByBusinessUserIdAndStaffIdAsync(request.UserId, request.StaffId);

            if (staff is null)
            {
                return Result.Failure($"Staff '{request.StaffId}' with related business user '{request.UserId}' not found", ResponseStatus.NotFound);
            }

            staff.SetDeviceId(request.DeviceId); 

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
