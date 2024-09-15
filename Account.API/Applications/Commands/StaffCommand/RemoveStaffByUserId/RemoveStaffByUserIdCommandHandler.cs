using Account.API.Applications.Services;
using Core.Results;
using Core.Messages; 
using Account.Domain.SeedWork;
using MediatR;
using Core.Exceptions;

namespace Account.API.Applications.Commands.StaffCommand.RemoveStaffByUserId;

public class RemoveStaffByUserIdCommandHandler(IUnitOfWork unitOfWork, ApplicationService service) : IRequestHandler<RemoveStaffByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;

    public async Task<Result> Handle(RemoveStaffByUserIdCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, cancellationToken);
        try
        {

            var staff = await _unitOfWork.Staffs.GetByIdAsync(request.StaffId);
            if (staff is null)
            {
                return Result.Failure($"Staff not found", ResponseStatus.NotFound)
                    .WithError(new("Staff.Id", "User not found"));
            }

            var fleets = await _unitOfWork.Fleets.GetByStaffIdAsync(staff.Id); 
            if(fleets is not null && fleets.Any())
            {
                foreach (var f in fleets)
                {
                    f.RemoveStaff();
                }
                _unitOfWork.Fleets.UpdateRange(fleets); 
            }

            _unitOfWork.Staffs.Delete(staff); 
            await _unitOfWork.CommitTransactionAsync(cancellationToken); 
            return Result.Success(null, ResponseStatus.NoContent);
        }
        catch (DomainException ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _service.Logger.LogError(ex, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }
}
