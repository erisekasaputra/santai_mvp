using Account.API.Extensions;
using Account.API.SeedWork;
using Account.API.Services;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.BusinessUserCommand.DeleteBusinessUserByUserId;

public class DeleteBusinessUserByUserIdCommandHandler(IUnitOfWork unitOfWork, ApplicationService service) : IRequestHandler<DeleteBusinessUserByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;
    public async Task<Result> Handle(DeleteBusinessUserByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.Users.GetBusinessUserByIdAsync(request.Id);
            if (user is null)
            {
                return Result.Failure($"Business user with id {request.Id} is not found", ResponseStatus.NotFound);
            }

            user.Delete();
            
            _unitOfWork.Users.Delete(user);
            
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
