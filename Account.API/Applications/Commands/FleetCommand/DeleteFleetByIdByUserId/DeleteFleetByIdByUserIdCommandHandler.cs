using Account.API.Options;
using Account.API.SeedWork;
using Account.API.Services; 
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;
using Microsoft.Extensions.Options;

namespace Account.API.Applications.Commands.FleetCommand.DeleteFleetByIdByUserId;

public class DeleteFleetByIdByUserIdCommandHandler : IRequestHandler<DeleteFleetByIdByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOptionsMonitor<ReferralProgramOption> _referralOptions;
    private readonly ApplicationService _service;
    private readonly IKeyManagementService _kmsClient;
    private readonly IHashService _hashService;

    public DeleteFleetByIdByUserIdCommandHandler(
      IUnitOfWork unitOfWork,
      IOptionsMonitor<ReferralProgramOption> referralOptions,
      ApplicationService service,
      IKeyManagementService kmsClient,
      IHashService hashService)
    {
        _unitOfWork = unitOfWork;
        _referralOptions = referralOptions;
        _service = service;
        _kmsClient = kmsClient;
        _hashService = hashService;
    }


    public async Task<Result> Handle(DeleteFleetByIdByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {  
            var fleet = await _unitOfWork.Fleets.GetByUserIdAndIdAsync(request.UserId, request.FleetId);

            if (fleet is null)
            {
                return Result.Failure($"Fleet not found", ResponseStatus.NotFound)
                    .WithError(new("Fleet.Id", "Fleet not found")); ;
            } 

            _unitOfWork.Fleets.Delete(fleet);

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
