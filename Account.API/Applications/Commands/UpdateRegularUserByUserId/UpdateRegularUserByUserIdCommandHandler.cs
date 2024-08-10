using Account.API.Applications.Services;
using Account.API.Extensions;
using Account.API.Mapper;
using Account.API.SeedWork;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.UpdateRegularUserByUserId;

public class UpdateRegularUserByUserIdCommandHandler(IUnitOfWork unitOfWork, AppService service) : IRequestHandler<UpdateRegularUserByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly AppService _service = service;

    public async Task<Result> Handle(UpdateRegularUserByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.Users.GetRegularUserByIdAsync(request.UserId);

            if (user is null)
            {
                return Result.Failure($"User {request.UserId} not found", ResponseStatus.NotFound);
            }

            user.Update(request.TimeZoneId, request.Address.ToAddress(), request.PersonalInfo.ToPersonalInfo(request.TimeZoneId));

            _unitOfWork.Users.Update(user);
            
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
