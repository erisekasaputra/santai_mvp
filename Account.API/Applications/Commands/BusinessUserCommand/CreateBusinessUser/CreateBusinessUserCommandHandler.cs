using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Extensions; 
using Account.API.Options;
using Account.API.SeedWork;
using Account.API.Services;
using Account.Domain.Aggregates.BusinessLicenseAggregate;
using Account.Domain.Aggregates.ReferralAggregate;
using Account.Domain.Aggregates.ReferredAggregate;
using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Enumerations;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using Account.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Options;
using System.Data; 

namespace Account.API.Applications.Commands.BusinessUserCommand.CreateBusinessUser;

public class CreateBusinessUserCommandHandler : IRequestHandler<CreateBusinessUserCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOptionsMonitor<ReferralProgramOption> _referralOptions;
    private readonly ApplicationService _service;
    private readonly IKeyManagementService _kmsClient;
    private readonly IHashService _hashService;

    public CreateBusinessUserCommandHandler(
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

    public async Task<Result> Handle(CreateBusinessUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var errors = new List<ErrorDetail>();

            await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken); 

            // hash email to make it secure but still got unique requirement
            var hashedEmail = await HashAsync(request.Email);

            // hash phone number to make it secure but still got unique requirement
            var hashedPhoneNumber = await HashAsync(request.PhoneNumber);

            // encrypt email to make it secure
            var encryptedEmail = await EncryptAsync(request.Email);
            
            // encrypt phone number to make it secure 
            var encryptedPhoneNumber = await EncryptAsync(request.PhoneNumber);
            
            // encrypt contact person make it secure  
            string encryptedContactPerson = await EncryptAsync(request.ContactPerson);
            
            // encrypt address line 1 make it secure  
            var encryptedAddressLine1 = await EncryptAsync(request.Address.AddressLine1);
            
            // encrypt address line 2 make it secure  
            string? encryptedAddressLine2 = await EncryptNullableAsync(request.Address.AddressLine2);
            
            // encrypt address line 3 make it secure  
            string? encryptedAddressLine3 = await EncryptNullableAsync(request.Address.AddressLine3);
            
            // encrypt tax id make it secure  
            string? encryptedTaxId = await EncryptNullableAsync(request.TaxId);

            // get all users that already registered with related request identities such as email, username, phonenumber, and identity id(from identity database)
            var userConflict = await _unitOfWork.Users.GetByIdentitiesAsNoTrackingAsync(
                (IdentityParameter.Username, request.Username),
                (IdentityParameter.Email, hashedEmail),
                (IdentityParameter.PhoneNumber, hashedPhoneNumber),
                (IdentityParameter.IdentityId, request.IdentityId.ToString()));

            // check if user with conlict identities is not null
            if (userConflict is not null)
            {
                // if it is not null, rollback the trasaction and get the conflict items
                errors.AddRange(UserIdentityConflict(
                    userConflict,
                    request.IdentityId,
                    request.Username,
                    hashedEmail,
                    hashedPhoneNumber));
            }


            ReferralProgram? referralProgram = null;

            // create referred programs when user input the referral code and referral code is valid
            if (!string.IsNullOrEmpty(request.ReferralCode))
            {
                // check is referral code is valid
                referralProgram = await _unitOfWork.ReferralPrograms.GetByCodeAsync(request.ReferralCode);
                if (referralProgram is null)
                {
                    // if it is not found the rollback transaction and give the error message related the error items
                    errors.Add(new ErrorDetail("User.ReferralCode", "Referral code not found"));
                }

                // check is referral program is still valid 
                if (referralProgram is not null && referralProgram.ValidDateUtc < DateTime.UtcNow)
                {
                    errors.Add(new ErrorDetail("User.ReferralCode", "Referral code expired"));
                }
            }
              

            // creating new instance of an address object
            var address = new Address(
                encryptedAddressLine1,
                encryptedAddressLine2,
                encryptedAddressLine3,
                request.Address.City,
                request.Address.State,
                request.Address.PostalCode,
                request.Address.Country);

            // get from environment settings is referral has active options
            int? referralRewardPoint = _referralOptions.CurrentValue.Point;
            int? referralValidMonth = _referralOptions.CurrentValue.ValidMonth;

            // create new instance of business user
            var user = new BusinessUser(
                request.IdentityId,
                request.Username,
                hashedEmail,
                encryptedEmail,
                hashedPhoneNumber,
                encryptedPhoneNumber,
                address,
                request.BusinessName,
                encryptedTaxId,
                encryptedContactPerson,
                request.WebsiteUrl,
                request.BusinessDescription,
                request.TimeZoneId);

            // register referral program if referral option has active value
            if (referralRewardPoint.HasValue && referralValidMonth.HasValue)
            {
                user.AddReferralProgram(referralRewardPoint.Value, referralRewardPoint.Value);
            }

            // create referred programs when user input the referral code and referral code is valid
            if (referralProgram is not null && request.ReferralCode is not null)
            { 
                // creating the referred programs
                await _unitOfWork.ReferredPrograms.CreateReferredProgramAsync(
                    new ReferredProgram(
                        referralProgram.UserId,
                        user.Id,
                        request.ReferralCode,
                        DateTime.UtcNow));
            }

            // get new Staff object with encrypted necessary data
            var staffRequests = await GetStaffWithEncryption(user.Id, user.Code, request.Staffs);

            // if staff request has data then will true and check the related identity is registered or not
            if (staffRequests is not null && staffRequests.Any())
            {
                // get registered identity from database
                var staffConflictAtRepository = await _unitOfWork.Staffs.GetByIdentitiesAsNoTrackingAsync(
                    (IdentityParameter.Username, staffRequests.Select(x => x.Username)),
                    (IdentityParameter.Email, staffRequests.Select(x => x.HashedEmail)),
                    (IdentityParameter.PhoneNumber, staffRequests.Select(x => x.HashedPhoneNumber)));

                // check if data conflicts is any data
                if (staffConflictAtRepository?.Any() ?? false)
                {
                    // if any conflict data then rollback transaction and returning error message
                    errors.AddRange(StaffIdentityConflict(
                         staffConflictAtRepository,
                         staffRequests));
                }

                // product error objects
                var errorStaffs = new List<ErrorDetail>();
                 
                int indexStaff = 0;
                foreach (var staff in staffRequests)
                {
                    (Staff? newStaff, string? errorParameter, string? errorMessage) = user.AddStaff(staff);

                    if (newStaff is null && errorParameter is not null)
                    {
                        errorStaffs.Add(new($"Staff[{indexStaff}].{errorParameter}", 
                            errorMessage ?? string.Empty));
                    }

                    indexStaff++;
                }

                if (errorStaffs is not null && errorStaffs.Count > 0)
                {
                    errors.AddRange(errorStaffs);    
                }
            }

            var businessLicenseRequests = await GetBusinessLicenseWithEncryption(user.Id, request.BusinessLicenses);

            // Register business license data
            if (businessLicenseRequests is not null && businessLicenseRequests.Any())
            {
                var conflictLicenses = await _unitOfWork.BusinessLicenses
                    .GetAcceptedStatusByLicenseNumbersAsNoTrackingAsync(businessLicenseRequests.Select(x => x.HashedLicenseNumber));

                if (conflictLicenses?.Any() ?? false)
                {
                    errors.AddRange(BusinessLicenseConflict(conflictLicenses, businessLicenseRequests));
                }

                var errorLicenses = new List<ErrorDetail>();

                int indexRequestLicense = 0;
                foreach (var businessLicense in businessLicenseRequests)
                {
                    (BusinessLicense? newBusinessLicense, string? errorParameter, string? errorMessage) = user.AddBusinessLicenses(businessLicense);

                    if (newBusinessLicense is null && errorParameter is not null)
                    {
                        errorLicenses.Add(new($"BusinessLicense[{indexRequestLicense}].{errorParameter}", errorMessage ?? string.Empty));
                    }

                    indexRequestLicense++;
                }

                if (errorLicenses is not null && errorLicenses.Count > 0)
                {
                    errors.AddRange(errorLicenses);
                }
            }

            if (errors.Count > 0)
            {
                return await RollbackAndReturnFailureAsync(
                    Result.Failure($"There {(errors.Count <= 1 ? "is" : "are")} few error(s) that you have to fixed", 
                    ResponseStatus.BadRequest).WithErrors(errors), cancellationToken);
            }

            await _unitOfWork.Users.CreateAsync(user);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            
            return Result.Success(ToBusinessUserResponseDto(user, request), ResponseStatus.Created); 
        }
        catch (DomainException ex)
        {
            return await RollbackAndReturnFailureAsync(
                Result.Failure(ex.Message, ResponseStatus.BadRequest),
                cancellationToken);
        }
        catch (Exception ex)
        {
            _service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return await RollbackAndReturnFailureAsync(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError),
                cancellationToken);
        }
    } 

    private static BusinessUserResponseDto ToBusinessUserResponseDto(BusinessUser user, CreateBusinessUserCommand request)
    {
        var address = new AddressResponseDto(
            request.Address.AddressLine1,
            request.Address.AddressLine2,
            request.Address.AddressLine3,
            request.Address.City,
            request.Address.State,
            request.Address.PostalCode,
            request.Address.Country);

        var loyalty = new LoyaltyProgramResponseDto(user.Id, user.LoyaltyProgram.LoyaltyPoints, user.LoyaltyProgram.LoyaltyTier);

        // get original data business licenses from request user
        var businessLicenseRequestList = request.BusinessLicenses.ToList();
        // make empty list of business licenses response dto
        var businessLicenses = new List<BusinessLicenseResponseDto>();
        if (user.BusinessLicenses is not null && user.BusinessLicenses.Count > 0)
        {
            // this this is used for take element inside list of business license request
            int index = 0;

            // looping the saved business licenses user
            foreach (var businessLicense in user.BusinessLicenses)
            {
                // get data business licenses original from business license request at spesific index
                var businessLicenseRequest = businessLicenseRequestList.ElementAt(index);

                // if not null then proccess it 
                if (businessLicenseRequest is not null)
                {  
                    businessLicenses.Add(new BusinessLicenseResponseDto(
                        businessLicense.Id,
                        businessLicenseRequest.LicenseNumber,
                        businessLicense.Name,
                        businessLicense.Description));
                }

                // incrementing index for next looping
                index++;
            }
        }

        // get original data staff from request user
        var staffRequestList = request.Staffs.ToList();
        // make empty list of staff response dto
        var staffs = new List<StaffResponseDto>();
        if (user.Staffs is not null && user.Staffs.Count > 0)
        {
            // this this is used for take element inside list of staff request
            int index = 0;
            
            // looping the saved staffs user
            foreach (var staff in user.Staffs)
            {
                // get data staff original from staff request at spesific index
                var staffRequest = staffRequestList.ElementAt(index);

                // if not null then proccess it 
                if (staffRequest is not null)
                {
                    var staffAddress = new AddressResponseDto(
                         staffRequest.Address.AddressLine1,
                         staffRequest.Address.AddressLine2,
                         staffRequest.Address.AddressLine3,
                         staffRequest.Address.City,
                         staffRequest.Address.State,
                         staffRequest.Address.PostalCode,
                         staffRequest.Address.Country);

                    staffs.Add(new StaffResponseDto(
                        staff.Id,
                        staff.Username,
                        staffRequest.Email,
                        staffRequest.PhoneNumber,
                        staff.Name,
                        staffAddress,
                        user.TimeZoneId));
                } 
                
                // incrementing index for next looping
                index++;
            }
        }

        var businessUserResponseDto = new BusinessUserResponseDto(
                user.Id,
                user.Username,
                request.Email,
                request.PhoneNumber,
                request.TimeZoneId,
                address,
                request.BusinessName,
                request.ContactPerson,
                request.TaxId,
                request.WebsiteUrl,
                request.BusinessDescription,
                loyalty,
                businessLicenses,
                staffs
            );

        return businessUserResponseDto;
    }

    private async Task<IEnumerable<Staff>?> GetStaffWithEncryption(
        Guid businessUserId,
        string code,
        IEnumerable<StaffRequestDto> staffs)
    {
        var newStaffs = new List<Staff>(); 

        if (staffs is null)  return null;     

        foreach (var staff in staffs) 
        {
            var hashedEmail = await HashAsync(staff.Email);
            var hashedPhoneNumber = await HashAsync(staff.PhoneNumber);

            var encryptedEmail = await EncryptAsync(staff.Email);
            var encryptedPhoneNumber = await EncryptAsync(staff.PhoneNumber);

            var encryptedAddressLine1 = await EncryptAsync(staff.Address.AddressLine1);
            var encryptedAddressLine2 = await EncryptNullableAsync(staff.Address.AddressLine2);
            var encryptedAddressLine3 = await EncryptNullableAsync(staff.Address.AddressLine3);

            var address = new Address(
                     encryptedAddressLine1,
                     encryptedAddressLine2,
                     encryptedAddressLine3,
                     staff.Address.City,
                     staff.Address.State,
                     staff.Address.PostalCode,
                     staff.Address.Country);

            newStaffs.Add(new Staff(
                businessUserId,
                code,
                staff.Username,
                hashedEmail,
                encryptedEmail,
                hashedPhoneNumber,
                encryptedPhoneNumber,
                staff.Name,
                address,
                staff.TimeZoneId,
                null));
        }

        return newStaffs;
    }

    private async Task<IEnumerable<BusinessLicense>?> GetBusinessLicenseWithEncryption(
        Guid businessUserId,
        IEnumerable<BusinessLicenseRequestDto> businessLicenses)
    {
        var newBusinessLicenses = new List<BusinessLicense>();

        if (businessLicenses is null)
        {
            return null;
        }

        foreach (var businessLicense in businessLicenses)
        {
            var hashedLicenseNumber = await HashAsync(businessLicense.LicenseNumber); 
            var encryptedLicenseNumber = await EncryptAsync(businessLicense.LicenseNumber);
            
            newBusinessLicenses.Add(new(
                businessUserId,
                hashedLicenseNumber,
                encryptedLicenseNumber,
                businessLicense.Name,
                businessLicense.Description));
        }

        return newBusinessLicenses;
    }

    private async Task<Result> RollbackAndReturnFailureAsync(Result result, CancellationToken cancellationToken)
    {
        await _unitOfWork.RollbackTransactionAsync(cancellationToken);

        return result;
    }

    private static List<ErrorDetail> BusinessLicenseConflict(
        IEnumerable<BusinessLicense> licenseConflicts,
        IEnumerable<BusinessLicense> licenseRequest)
    {
        var errors = new List<ErrorDetail>(); 

        var cleanedRequest = new HashSet<string>(licenseRequest.Select(x => x.HashedLicenseNumber));

        int indexRequestLicense = 0;
        foreach (var businessLicense in licenseConflicts)
        {
            var number = businessLicense.HashedLicenseNumber;
            var errorParameter = nameof(businessLicense.HashedLicenseNumber);

            if (cleanedRequest.Contains(number))
            {
                errors.Add(new ($"BusinessLicense[{indexRequestLicense}].{errorParameter}",
                    "Can not have multiple license numbers with accepted status"));
            } 
            indexRequestLicense++;
        }

        return errors;
    }

    private static List<ErrorDetail> UserIdentityConflict(
        User user,
        Guid identityId,
        string username,
        string email,
        string phoneNumber)
    {
        var conflicts = new List<ErrorDetail>();

        if (user.Username == username)
        {
            conflicts.Add(new ($"User.{nameof(user.Username)}", 
                "User username already registered"));
        }

        if (user.HashedEmail == email || user.NewHashedEmail == email)
        {
            conflicts.Add(new ($"User.{nameof(user.HashedEmail)}", 
                "User email already registered"));
        }

        if (user.HashedPhoneNumber == phoneNumber || user.NewHashedPhoneNumber == phoneNumber)
        {
            conflicts.Add(new ($"User.{nameof(user.HashedPhoneNumber)}", 
                "User phone number already registered"));
        }

        if (user.IdentityId == identityId)
        {
            conflicts.Add(new ($"User.{nameof(user.IdentityId)}", 
                "Identity id already registered"));
        }

        return conflicts;
    }

    private static List<ErrorDetail> StaffIdentityConflict(
        IEnumerable<Staff> conflicts,
        IEnumerable<Staff> staffs)
    {
        var errors = staffs
            .SelectMany((staff, index) =>
            {
                var errorDetails = new List<ErrorDetail>();

                if (conflicts.Any(x => x.Username == staff.Username))
                {
                    errorDetails.Add(new ErrorDetail($"Staff[{index}].{nameof(staff.Username)}",
                        "Username already registered"));
                }

                if (conflicts.Any(x => x.HashedEmail == staff.HashedEmail || x.NewHashedEmail == staff.HashedEmail))
                {
                    errorDetails.Add(new ErrorDetail($"Staff[{index}].{nameof(staff.HashedEmail)}",
                        "Email already registered"));
                }

                if (conflicts.Any(x => x.HashedPhoneNumber == staff.HashedPhoneNumber || x.NewHashedPhoneNumber == staff.HashedPhoneNumber))
                {
                    errorDetails.Add(new ErrorDetail($"Staff[{index}].{nameof(staff.HashedPhoneNumber)}",
                        "Phone number already registered"));
                }

                return errorDetails;
            }).ToList();

        return errors;
    }

    private async Task<string?> EncryptNullableAsync(string? plaintext)
    {
        if (string.IsNullOrEmpty(plaintext))
            return null;

        return await _kmsClient.EncryptAsync(plaintext);
    }

    private async Task<string> EncryptAsync(string plaintext)
    {
        return await _kmsClient.EncryptAsync(plaintext);
    } 

    private async Task<string> HashAsync(string plainText)
    {
        return await _hashService.Hash(plainText);
    }
}

