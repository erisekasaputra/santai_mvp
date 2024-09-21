using Account.API.Applications.Services;
using Core.Results;
using Account.Domain.SeedWork;
using MediatR;
using Core.Exceptions;
using Core.CustomMessages;

namespace Account.API.Applications.Commands.MechanicUserCommand.SetRatingByUserId;

public class SetRatingByUserIdCommandHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service) : IRequestHandler<SetRatingByUserIdCommand, Result>
{

    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;

    public async Task<Result> Handle(SetRatingByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.BaseUsers.GetMechanicUserByIdAsync(request.UserId);
            if (user is null)
            {
                return Result.Failure($"Mechanic user not found", ResponseStatus.NotFound) 
                     .WithError(new("MechanicUser.Id", "Mechanic user not found"));
            }
            
            user.SetRating(request.Rating);

            _unitOfWork.BaseUsers.Update(user);

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
