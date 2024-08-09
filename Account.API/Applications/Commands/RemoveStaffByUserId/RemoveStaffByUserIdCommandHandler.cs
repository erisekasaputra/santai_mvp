using Account.API.Applications.Services;
using Account.API.Extensions;
using Account.API.SeedWork;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.RemoveStaffByUserId;

public class RemoveStaffByUserIdCommandHandler(IUnitOfWork unitOfWork, AppService service) : IRequestHandler<RemoveStaffByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly AppService _service = service;

    public async Task<Result> Handle(RemoveStaffByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.Users.GetBusinessUserByIdAsync(request.BusinessUserId);

            if (user is null)
            {
                return Result.Failure($"Business user '{request.BusinessUserId}' not found", ResponseStatus.NotFound);
            }

            var deletedStaff = user.RemoveStaff(request.StaffId);

            if (deletedStaff is not null) 
            {
                _unitOfWork.AttachEntity(deletedStaff);
                _unitOfWork.SetEntityState(deletedStaff, Microsoft.EntityFrameworkCore.EntityState.Deleted);
            }

            _unitOfWork.Users.UpdateUser(user);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(null, ResponseStatus.NoContent);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (Exception ex)
        {
            _service.Logger.LogError(ex.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        } 
    }
}
