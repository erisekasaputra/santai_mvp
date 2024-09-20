using Account.API.Applications.Services;
using Core.Results;
using Core.Messages; 
using Account.Domain.SeedWork;
using MediatR;
using Core.Exceptions;

namespace Account.API.Applications.Commands.StaffCommand.SetDeviceIdByStaffId;

public class SetDeviceIdByStaffIdCommandHandler(IUnitOfWork unitOfWork, ApplicationService service) : IRequestHandler<SetDeviceIdByStaffIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;
    public async Task<Result> Handle(SetDeviceIdByStaffIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var staff = await _unitOfWork.Staffs.GetByIdAsync(request.StaffId);
            if (staff is null)
            {
                return Result.Failure($"Staff not found", ResponseStatus.NotFound)
                    .WithError(new("Staff.Id", "User not found"));
            }

            staff.AddDeviceId(request.DeviceId);

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
