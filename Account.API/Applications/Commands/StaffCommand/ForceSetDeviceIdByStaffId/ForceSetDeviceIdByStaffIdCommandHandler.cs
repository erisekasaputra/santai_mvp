using Account.API.SeedWork;
using Account.API.Services;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.StaffCommand.ForceSetDeviceIdByStaffId;

public class ForceSetDeviceIdByStaffIdCommandHandler(IUnitOfWork unitOfWork, ApplicationService service) : IRequestHandler<ForceSetDeviceIdByStaffIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;
    public async Task<Result> Handle(ForceSetDeviceIdByStaffIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var staff = await _unitOfWork.Staffs.GetByIdAsync(request.StaffId);
            if (staff is null)
            {
                return Result.Failure($"Staff not found", ResponseStatus.NotFound)
                    .WithError(new("Staff.Id", "User not found"));
            }

            staff.ForceSetDeviceId(request.DeviceId);

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
