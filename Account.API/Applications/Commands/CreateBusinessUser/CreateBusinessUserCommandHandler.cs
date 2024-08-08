using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Applications.Services;
using Account.API.Configurations;
using Account.API.Extensions;
using Account.API.Mapper;
using Account.API.SeedWork;
using Account.Domain.Aggregates.BusinessLicenseAggregate;
using Account.Domain.Aggregates.ReferredAggregate;
using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Enumerations;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using Account.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Options;
using System.Data;

namespace Account.API.Applications.Commands.CreateBusinessUser;

public class CreateBusinessUserCommandHandler : IRequestHandler<CreateBusinessUserCommand, Result>
{ 
    private readonly IUnitOfWork _unitOfWork;

    private readonly IOptionsMonitor<MasterReferralProgram> _optionReferralMaster;

    private readonly AppService _service;

    public CreateBusinessUserCommandHandler(IUnitOfWork unitOfWork, IOptionsMonitor<MasterReferralProgram> optionReferralMaster, AppService service)
    {
        _unitOfWork = unitOfWork;
        _optionReferralMaster = optionReferralMaster;
        _service = service;
    }

    public async Task<Result> Handle(CreateBusinessUserCommand request, CancellationToken cancellationToken)
    { 
        try
        {
            var userConflict = await _unitOfWork.Users.GetByIdentityAsNoTrackAsync(
                (IdentityParameter.Username, request.Request.Username),
                (IdentityParameter.Email, request.Request.Email),
                (IdentityParameter.PhoneNumber, request.Request.PhoneNumber));

            if (userConflict is not null)
            {
                return UserIdentityConflict(userConflict, request.Request);
            }

            var address = new Address(
                request.Request.Address.AddressLine1,
                request.Request.Address.AddressLine2,
                request.Request.Address.AddressLine3,
                request.Request.Address.City,
                request.Request.Address.State,
                request.Request.Address.PostalCode,
                request.Request.Address.Country);

            int? referralRewardPoint = _optionReferralMaster.CurrentValue.Point;
            int? referralValidMonth = _optionReferralMaster.CurrentValue.ValidMonth;

            var user = new BusinessUser(
                request.Request.IdentityId,
                request.Request.Username,
                request.Request.Email,
                request.Request.PhoneNumber,
                address,
                request.Request.BusinessName,
                request.Request.TaxId,
                request.Request.ContactPerson,
                request.Request.WebsiteUrl,
                request.Request.BusinessDescription, 
                request.Request.TimeZoneId);

            // Registering referral program
            if (referralRewardPoint.HasValue && referralValidMonth.HasValue)
            {
                user.AddReferralProgram(referralRewardPoint.Value, referralRewardPoint.Value);
            }

            // Create referred programs
            if (!string.IsNullOrEmpty(request.Request.ReferralCode))
            {
                var referralProgram = await _unitOfWork.ReferralPrograms.GetByCodeAsync(request.Request.ReferralCode);

                if (referralProgram is null)
                {
                    return Result.Failure($"Referral code {request.Request.ReferralCode} is not found", ResponseStatus.NotFound)
                        .WithError(new ErrorDetail("ReferralCode", "Referral code is not found"));
                }

                if (referralProgram.ValidDateUtc < DateTime.UtcNow)
                {
                    return Result.Failure($"Referral code {request.Request.ReferralCode} is invalid", ResponseStatus.BadRequest)
                        .WithError(new ErrorDetail("ReferralCode", "Referral code is expired"));
                }

                await _unitOfWork.ReferredPrograms.CreateReferredProgramAsync(
                    new ReferredProgram(
                        referralProgram.UserId,
                        user.Id,
                        request.Request.ReferralCode,
                        DateTime.UtcNow));
            }

            // Registering staff data
            if (request.Request.Staffs.Any())
            {  
                var staffConflictAtRepository = await _unitOfWork.Staffs.GetByIdsAsNoTrackAsync(
                    (IdentityParameter.Username, request.Request.Staffs.Select(x => x.Username)),
                    (IdentityParameter.Email, request.Request.Staffs.Select(x => x.Email)),
                    (IdentityParameter.PhoneNumber, request.Request.Staffs.Select(x => x.PhoneNumber)));

                if (staffConflictAtRepository?.Any() ?? false)
                {
                    return StaffIdentityConflict(staffConflictAtRepository, request.Request.Staffs);
                }


                var staffConflictAtAggregate = new List<ErrorDetail>();

                foreach (var staff in request.Request.Staffs)
                {
                    (string? errorParameter, string? errorMessage) =  user.AddStaff(
                                                                         staff.Username,
                                                                         staff.Email,
                                                                         staff.PhoneNumber,
                                                                         staff.Name,
                                                                         staff.Address.ToAddress(),
                                                                         staff.TimeZoneId);

                    if (errorParameter is not null) 
                    {
                        staffConflictAtAggregate.Add(new ErrorDetail(errorParameter, errorMessage ?? string.Empty));
                    }
                }

                if (staffConflictAtAggregate is not null && staffConflictAtAggregate.Count > 0)
                {
                    return Result.Failure("There are staff identity conflict errors", ResponseStatus.BadRequest)
                        .WithErrors(staffConflictAtAggregate);
                }
            }

            // Register business license data
            if (request.Request.BusinessLicenses.Any())
            {
                var licenseConflictAtRepository = await _unitOfWork.BusinessLicenses.GetAcceptedByNumbersAsNoTrackAsync(request.Request.BusinessLicenses.Select(x => x.LicenseNumber));

                if (licenseConflictAtRepository?.Any() ?? false) 
                {
                    return BusinessLicenseConflict(licenseConflictAtRepository, request.Request.BusinessLicenses);
                }
                 
                var licenseConflictAggregate = new List<ErrorDetail>();

                foreach (var businessLicense in request.Request.BusinessLicenses)
                { 
                    (string? errorParameter, string? errorMessage) = user.AddBusinessLicenses(
                                                                        businessLicense.LicenseNumber,
                                                                        businessLicense.Name,
                                                                        businessLicense.Description);

                    if (errorParameter is not null)
                    {
                        licenseConflictAggregate.Add(new (errorParameter, errorMessage ?? string.Empty));
                    }
                }
            } 

            var result = await _unitOfWork.Users.CreateUserAsync(user);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(result.ToBusinessUserResponseDto() , ResponseStatus.Created);
        }
        catch (DomainException ex)
        {
            _service.Logger.LogError(ex.Message); 
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (Exception ex) 
        {
            _service.Logger.LogError(ex.Message);  
            return Result.Failure("An error occurred while processing your request", ResponseStatus.InternalServerError);
        } 
    }

    private static Result BusinessLicenseConflict(IEnumerable<BusinessLicense> licenseConflicts, IEnumerable<BusinessLicenseRequestDto> licenseRequest)
    {
        var errors = new List<ErrorDetail>();

        var cleanedRequest = new HashSet<string>(licenseRequest.Select(x => x.LicenseNumber.CleanAndLowering())); 

        foreach (var businessLicense in licenseConflicts)
        {
            var cleanedConflict = businessLicense.LicenseNumber.CleanAndLowering(); 

            if (cleanedRequest.Contains(cleanedConflict))
            {
                errors.Add(new ErrorDetail(
                    nameof(businessLicense.LicenseNumber),
                    $"License Number: {cleanedConflict} with statuc 'Accepted' already registered "));
            } 
        }

        var message = errors.Count switch
        {
            1 => "There is one conflict",
            _ => $"There are {errors.Count} conflicts"
        };

        return Result.Failure(message, ResponseStatus.BadRequest).WithErrors(errors);
    }

    private static Result UserIdentityConflict(User user, BusinessUserRequestDto request)
    {
        var conflictIdentities = new List<ErrorDetail>();

        string cleanedUsername = user.Username.CleanAndLowering();
        string cleanedEmail = user.Email.CleanAndLowering();
        string? cleanedNewEmail = user.NewEmail?.CleanAndLowering();
        string cleanedPhoneNumber = user.PhoneNumber.Clean();
        string? cleanedNewPhoneNumber = user.NewPhoneNumber?.Clean();

        if (cleanedUsername.Equals(request.Username.CleanAndLowering(), StringComparison.CurrentCultureIgnoreCase))
        {
            conflictIdentities.Add(new ErrorDetail(
                nameof(user.Username),
                $"Username: {request.Username}"));
        }

        if (cleanedEmail.Equals(request.Email.CleanAndLowering(), StringComparison.CurrentCultureIgnoreCase)
            || (cleanedNewEmail?.Equals(request.Email.CleanAndLowering(), StringComparison.CurrentCultureIgnoreCase) == true))
        {
            conflictIdentities.Add(new ErrorDetail(
                nameof(user.Email),
                $"Email: {request.Email}"));
        }

        if (cleanedPhoneNumber.Equals(request.PhoneNumber.Clean(), StringComparison.CurrentCultureIgnoreCase)
            || (cleanedNewPhoneNumber?.Equals(request.PhoneNumber.Clean(), StringComparison.CurrentCultureIgnoreCase) == true))
        {
            conflictIdentities.Add(new ErrorDetail(
                nameof(user.PhoneNumber),
                $"Phone number: {request.PhoneNumber}"));
        }

        var message = conflictIdentities.Count switch
        {
            1 => "There is one conflict",
            _ => $"There are {conflictIdentities.Count} conflicts"
        };

        return Result.Failure(message, ResponseStatus.BadRequest).WithErrors(conflictIdentities);
    }

    private static Result StaffIdentityConflict(IEnumerable<Staff> staffConflicts, IEnumerable<StaffRequestDto> staffs)
    {
        var conflictIdentities = new List<ErrorDetail>();
         
        var cleanedUsernames = new HashSet<string>(staffs.Select(x => x.Username.CleanAndLowering()));
        var cleanedEmails = new HashSet<string>(staffs.Select(x => x.Email.CleanAndLowering()));
        var cleanedPhoneNumbers = new HashSet<string>(staffs.Select(x => x.PhoneNumber.Clean()));

        foreach (var staff in staffConflicts)
        {
            var cleanedUsername = staff.Username.CleanAndLowering();
            var cleanedEmail = staff.Email.CleanAndLowering();
            var cleanedNewEmail = staff.NewEmail?.CleanAndLowering();
            var cleanedPhoneNumber = staff.PhoneNumber.Clean();
            var cleanedNewPhoneNumber = staff.NewPhoneNumber?.Clean();

            if (cleanedUsernames.Contains(cleanedUsername))
            {
                conflictIdentities.Add(new ErrorDetail(
                    nameof(staff.Username),
                    $"Username: {cleanedUsername}"));
            }

            if (cleanedEmails.Contains(cleanedEmail) || (cleanedNewEmail != null && cleanedEmails.Contains(cleanedNewEmail)))
            {
                conflictIdentities.Add(new ErrorDetail(
                    nameof(staff.Email),
                    $"Email: {cleanedEmail}"));
            }

            if (cleanedPhoneNumbers.Contains(cleanedPhoneNumber) || (cleanedNewPhoneNumber != null && cleanedPhoneNumbers.Contains(cleanedNewPhoneNumber)))
            {
                conflictIdentities.Add(new ErrorDetail(
                    nameof(staff.PhoneNumber),
                    $"Phone number: {cleanedPhoneNumber}"));
            }
        }

        var message = conflictIdentities.Count switch
        {
            1 => "There is one conflict",
            _ => $"There are {conflictIdentities.Count} conflicts"
        };

        return Result.Failure(message, ResponseStatus.BadRequest).WithErrors(conflictIdentities);
    }

}
