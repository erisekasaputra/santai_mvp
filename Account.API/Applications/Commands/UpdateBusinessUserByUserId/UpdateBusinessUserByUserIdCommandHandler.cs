using Account.API.Applications.Services;
using Account.API.Extensions;
using Account.API.Mapper;
using Account.API.SeedWork;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.UpdateBusinessUserByUserId;

public class UpdateBusinessUserByUserIdCommandHandler(IUnitOfWork unitOfWork, AppService service) : IRequestHandler<UpdateBusinessUserByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly AppService _service = service; 
    public async Task<Result> Handle(UpdateBusinessUserByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.Users.GetBusinessUserByIdAsync(request.Id);

            if (user is null)
            {
                return Result.Failure($"Business '{request.Id}' not found", ResponseStatus.NotFound);
            }

            user.Update(
                request.BusinessName,
                request.ContactPerson,
                request.TaxId,
                request.WebsiteUrl,
                request.Description,
                request.Address.ToAddress(),
                request.TimeZoneId);

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
