using Account.API.Applications.Services;
using Core.Results;
using Account.Domain.SeedWork;
using MediatR;
using Core.Exceptions;
using Core.CustomMessages;

namespace Account.API.Applications.Commands.StaffCommand.ResetPhoneNumberByStaffId;

public class ResetPhoneNumberByStaffIdCommandHandler( 
    IUnitOfWork unitOfWork,
    ApplicationService service) : IRequestHandler<ResetPhoneNumberByStaffIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service; 

    public async Task<Result> Handle(ResetPhoneNumberByStaffIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.Staffs.GetByIdAsync(request.Id);
            if (user is null)
            {
                return Result.Failure($"User not found", ResponseStatus.NotFound);
            }
              
            user.ResetPhoneNumber();

            _unitOfWork.Staffs.Update(user);

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
