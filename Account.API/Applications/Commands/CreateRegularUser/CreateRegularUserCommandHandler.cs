using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Applications.Services;
using Account.API.Extensions;
using Account.API.Mapper;
using Account.API.Options;
using Account.API.SeedWork;
using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Enumerations;
using Account.Domain.SeedWork; 
using MediatR;
using Microsoft.Extensions.Options;

namespace Account.API.Applications.Commands.CreateRegularUser;

public class CreateRegularUserCommandHandler(IUnitOfWork unitOfWork, AppService service, IOptionsMonitor<ReferralProgramOption> referralOptions) : IRequestHandler<CreateRegularUserCommand, Result>
{
    private readonly IUnitOfWork _unitOfWOrk = unitOfWork;
    private readonly AppService _appService = service;
    private readonly IOptionsMonitor<ReferralProgramOption> _referralOptions = referralOptions;

    public async Task<Result> Handle(CreateRegularUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userRequest = request.Request;
            var personalInfoRequest = userRequest.PersonalInfo;

            var userConflict = await _unitOfWOrk.Users.GetByIdentityAsNoTrackAsync(
                    (IdentityParameter.Email, userRequest.Email),    
                    (IdentityParameter.PhoneNumber, userRequest.PhoneNumber),    
                    (IdentityParameter.Username, userRequest.Username) 
                ); 

            if (userConflict is not null)
            {
                return UserIdentityConflict(userConflict, userRequest);
            }

            var user = new RegularUser(
                userRequest.IdentityId,
                userRequest.Username,
                userRequest.Email,
                userRequest.PhoneNumber,
                userRequest.Address.ToAddress(),
                userRequest.PersonalInfo.ToPersonalInfo(userRequest.TimeZoneId),
                userRequest.TimeZoneId,
                userRequest.TimeZoneId);

            // creating referral program if exists
            int? referralRewardPoint = _referralOptions.CurrentValue.Point;
            int? referralValidMonth = _referralOptions.CurrentValue.ValidMonth; 
            if (referralRewardPoint.HasValue && referralValidMonth.HasValue)
            {
                user.AddReferralProgram(referralRewardPoint.Value, referralRewardPoint.Value);
            } 

            await _unitOfWOrk.Users.CreateUserAsync(user); 
            return Result.Success(user, ResponseStatus.Created);
        }
        catch (Exception ex) 
        {
            _appService.Logger.LogError(ex.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }

    private static Result UserIdentityConflict(User user, RegularUserRequestDto request)
    {
        var conflictIdentities = new List<ErrorDetail>();

        if (user.Username == request.Username)
        {
            conflictIdentities.Add(new ErrorDetail(nameof(user.Username), $"Username: {request.Username}"));
        }

        if (user.Email == request.Email || user.NewEmail == request.Email)
        {
            conflictIdentities.Add(new ErrorDetail(nameof(user.Email), $"Email: {request.Email}"));
        }

        if (user.PhoneNumber == request.PhoneNumber || user.NewPhoneNumber == request.PhoneNumber)
        {
            conflictIdentities.Add(new ErrorDetail(nameof(user.PhoneNumber), $"Phone number: {request.PhoneNumber}"));
        }

        var message = conflictIdentities.Count == 1
            ? "There is a conflict"
            : $"There are {conflictIdentities.Count} conflicts";

        return Result.Failure(message, ResponseStatus.BadRequest).WithErrors(conflictIdentities);
    }
}
