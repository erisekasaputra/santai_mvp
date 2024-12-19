using Account.API.Applications.Models;
using Account.API.Applications.Services.Interfaces;
using Core.Results;
using MediatR;
using Polly;

namespace Account.API.Applications.Commands.MechanicUserCommand.UpdateLocationByUserId;

public class UpdateLocationByUserIdCommandHandler(IMechanicCache mechanicCache) : IRequestHandler<UpdateLocationByUserIdCommand, Result>
{
    private readonly IMechanicCache _mechanicCache = mechanicCache;
    public async Task<Result> Handle(UpdateLocationByUserIdCommand request, CancellationToken cancellationToken)
    {  
        var mechanic = new MechanicExistence(
            request.MechanicId.ToString(),
            string.Empty,
            string.Empty,
            string.Empty,
            request.Latitude,
            request.Longitude,
            MechanicStatus.Available); 

        bool isSucceed = false;
        const int maxRetry = 3;
        int retryCount = 0;
        while (!isSucceed && retryCount < maxRetry)
        {
            isSucceed = await _mechanicCache.UpdateLocationAsync(mechanic);
            if (!isSucceed)
            {
                retryCount++;
                await Task.Delay(100, cancellationToken);
            }
        }  

        if (isSucceed)
        {
            return Result.Success(null, ResponseStatus.NoContent);
        }

        return Result.Failure("Can not upadate user location", ResponseStatus.BadRequest);
    }
}
