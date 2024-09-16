using Account.API.Applications.Models; 
using Account.API.Applications.Services.Interfaces; 
using Core.Exceptions;
using Core.Messages;
using Core.Results;   
using Core.Utilities;
using MediatR;

namespace Account.API.Applications.Commands.MechanicUserCommand.SetMechanicLocationByUserIdCommand;

public class SetMechanicLocationByUserIdCommandHandler : IRequestHandler<SetMechanicLocationByUserIdCommand, Result>
{ 
    private readonly IMechanicCache _cacheService;
    private readonly ILogger<SetMechanicLocationByUserIdCommandHandler> _logger;

    public SetMechanicLocationByUserIdCommandHandler( 
        IMechanicCache cacheService,
        ILogger<SetMechanicLocationByUserIdCommandHandler> logger)
    { 
        _cacheService = cacheService;   
        _logger = logger;
    }
    public async Task<Result> Handle(SetMechanicLocationByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var mechanic = new MechanicExistence(request.MechanicId.ToString(), null, request.Latitude, request.Longitude, MechanicStatus.Available);

            await _cacheService.UpdateLocationAsync(mechanic);

            return Result.Success(null, ResponseStatus.NoContent);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }
}
