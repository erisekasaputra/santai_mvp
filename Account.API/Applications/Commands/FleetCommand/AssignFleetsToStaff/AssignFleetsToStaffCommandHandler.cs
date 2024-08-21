using Account.API.Options;
using Account.API.SeedWork;
using Account.API.Services;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;
using Microsoft.Extensions.Options;

namespace Account.API.Applications.Commands.FleetCommand.AssignFleetsToStaff;

public class AssignFleetsToStaffCommandHandler : IRequestHandler<AssignFleetsToStaffCommand, Result>
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly IOptionsMonitor<ReferralProgramOption> _referralOptions;
    private readonly ApplicationService _service;
    private readonly IKeyManagementService _kmsClient;
    private readonly IHashService _hashService;

    public AssignFleetsToStaffCommandHandler(
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


    public async Task<Result> Handle(AssignFleetsToStaffCommand request, CancellationToken cancellationToken)
    {
        try
        { 
            await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, cancellationToken);

            if (request is null || !request.FleetIds.Any())
            {
                return Result.Failure("Please provide the fleet ids", ResponseStatus.BadRequest)
                    .WithError(new ("Fleet.Ids", "Fleets id request is empty"));
            }

            var fleets = await _unitOfWork.Fleets.GetByIdsAsync(request.FleetIds);

            if (fleets is null || !fleets.Any())
            {
                return Result.Failure("Fleets does not have any record", ResponseStatus.NotFound)
                     .WithError(new ErrorDetail("Fleet.Ids", "Fleets not found"));
            }


            var staff = await _unitOfWork.Staffs.GetByBusinessUserIdAndStaffIdAsync(request.UserId, request.StaffId);

            if (staff is null)
            {
                return Result.Failure("Staff not found", ResponseStatus.NotFound)
                    .WithError(new ErrorDetail("Staff.Id", "Staff not found"));
            }


            int countFleetResponse = fleets.Count();
            int countFleetRequest = request.FleetIds.Count();
            int notFoundFleet = countFleetResponse - countFleetRequest;

            if (countFleetResponse < countFleetRequest)
            {
                var fleetIds = fleets.Select(x => x.Id);

                var errors = new List<ErrorDetail>();

                errors.AddRange(
                    request.FleetIds
                        .Where(x => !fleetIds.Contains(x))
                        .Select((fleetId, index) => new ErrorDetail($"Fleet[{index}].Id", "Fleet not found")));

                return Result.Failure($"{notFoundFleet} fleet(s) {(notFoundFleet <= 1 ? "was" : "were")} not found", ResponseStatus.BadRequest)
                    .WithErrors(errors);
            }

            foreach (var fleet in fleets)
            {
                fleet.AssignStaff(staff);
                _unitOfWork.Fleets.Update(fleet);
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result.Success(null, ResponseStatus.NoContent);
        }
        catch (DomainException ex)
        {
            return await RollbackAndReturnFailureAsync(
                Result.Failure(ex.Message, ResponseStatus.BadRequest),
                cancellationToken);
        }
        catch (Exception ex)
        {
            _service.Logger.LogError(ex, ex.InnerException?.Message);
            return await RollbackAndReturnFailureAsync(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError),
                cancellationToken);
        }
    }

    private async Task<Result> RollbackAndReturnFailureAsync(Result result, CancellationToken cancellationToken)
    {
        await _unitOfWork.RollbackTransactionAsync(cancellationToken);

        return result;
    } 
}
