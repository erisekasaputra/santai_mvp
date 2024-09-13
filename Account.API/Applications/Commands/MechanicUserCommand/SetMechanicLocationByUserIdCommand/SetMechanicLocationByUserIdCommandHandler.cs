using Account.API.Applications.Services;
using Account.Domain.SeedWork;
using Core.Exceptions;
using Core.Messages;
using Core.Results;
using Core.Services.Interfaces;
using MediatR;

namespace Account.API.Applications.Commands.MechanicUserCommand.SetMechanicLocationByUserIdCommand;

public class SetMechanicLocationByUserIdCommandHandler : IRequestHandler<SetMechanicLocationByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationService _service; 
    private readonly ICacheService _cacheService;

    public SetMechanicLocationByUserIdCommandHandler(
        IUnitOfWork unitOfWork,
        ApplicationService service,
        ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _service = service;
        _cacheService = cacheService;   
    }
    public async Task<Result> Handle(SetMechanicLocationByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
             

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
