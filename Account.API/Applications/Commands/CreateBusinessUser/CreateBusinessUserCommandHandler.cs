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
using Microsoft.EntityFrameworkCore;
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
                    return Result.Failure($"Referral code '{request.Request.ReferralCode}' not found", ResponseStatus.NotFound)
                        .WithError(new ErrorDetail("ReferralCode", "Referral code not found"));
                }

                if (referralProgram.ValidDateUtc < DateTime.UtcNow)
                {
                    return Result.Failure($"Referral code '{request.Request.ReferralCode}' invalid", ResponseStatus.BadRequest)
                        .WithError(new ErrorDetail("ReferralCode", "Referral code expired"));
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
                var staffConflictAtRepository = await _unitOfWork.Staffs.GetByIdentitiesAsNoTrackAsync(
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
                    (Staff? newStaff, string? errorParameter, string? errorMessage) = user.AddStaff(
                         staff.Username,
                         staff.Email,
                         staff.PhoneNumber,
                         staff.Name,
                         staff.Address.ToAddress(),
                         staff.TimeZoneId);

                    if (newStaff is null && errorParameter is not null)
                    {
                        staffConflictAtAggregate.Add(new ErrorDetail(errorParameter, errorMessage ?? string.Empty));
                    }

                    if (newStaff is not null)
                    {
                        _unitOfWork.AttachEntity(newStaff);
                        _unitOfWork.SetEntityState(newStaff, EntityState.Added);
                    }
                }

                if (staffConflictAtAggregate is not null && staffConflictAtAggregate.Count > 0)
                {
                    return Result.Failure("There are conflicts", ResponseStatus.BadRequest)
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
                    (BusinessLicense? newBusinessLicense, string? errorParameter, string? errorMessage) = user.AddBusinessLicenses(
                        businessLicense.LicenseNumber,
                        businessLicense.Name,
                        businessLicense.Description);

                    if (newBusinessLicense is null && errorParameter is not null)
                    {
                        licenseConflictAggregate.Add(new(errorParameter, errorMessage ?? string.Empty));
                    }

                    if (newBusinessLicense is not null)
                    {
                        _unitOfWork.AttachEntity(newBusinessLicense);
                        _unitOfWork.SetEntityState(newBusinessLicense, EntityState.Added);
                    }
                }
            }

            var result = await _unitOfWork.Users.CreateUserAsync(user);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(result.ToBusinessUserResponseDto(), ResponseStatus.Created);
        }
        catch (DomainException ex)
        {
            _service.Logger.LogError(ex.Message);
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (Exception ex)
        {
            _service.Logger.LogError(ex.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }

    private static Result BusinessLicenseConflict(IEnumerable<BusinessLicense> licenseConflicts, IEnumerable<BusinessLicenseRequestDto> licenseRequest)
    {
        var errors = new List<ErrorDetail>();

        var cleanedRequest = new HashSet<string>(licenseRequest.Select(x => x.LicenseNumber));

        foreach (var businessLicense in licenseConflicts)
        {
            var number = businessLicense.LicenseNumber;

            if (cleanedRequest.Contains(number))
            {
                errors.Add(new ErrorDetail(
                    nameof(businessLicense.LicenseNumber), $"Can not have multiple license numbers with accepted status"));
            }
        }

        var message = errors.Count switch
        {
            1 => "There is a conflict",
            _ => $"There are {errors.Count} conflicts"
        };

        return Result.Failure(message, ResponseStatus.BadRequest).WithErrors(errors);
    }

    private static Result UserIdentityConflict(User user, BusinessUserRequestDto request)
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

    private static Result StaffIdentityConflict(IEnumerable<Staff> staffConflicts, IEnumerable<StaffRequestDto> staffs)
    {
        var conflictIdentities = staffs
            .SelectMany(staff => new[]
            {
                staffConflicts.Any(x => x.Username == staff.Username)
                    ? new ErrorDetail(nameof(staff.Username), $"Username: {staff.Username}")
                    : null,
                staffConflicts.Any(x => x.Email == staff.Email || x.NewEmail == staff.Email)
                    ? new ErrorDetail(nameof(staff.Email), $"Email: {staff.Email}")
                    : null,
                staffConflicts.Any(x => x.PhoneNumber == staff.PhoneNumber || x.NewPhoneNumber == staff.PhoneNumber)
                    ? new ErrorDetail(nameof(staff.PhoneNumber), $"Phone number: {staff.PhoneNumber}")
                    : null
            }).Where(detail => detail != null).ToList();

        var message = conflictIdentities.Count == 1
            ? "There is a conflict"
            : $"There are {conflictIdentities.Count} conflicts";

        return Result.Failure(message, ResponseStatus.BadRequest).WithErrors(conflictIdentities!);
    } 
}
