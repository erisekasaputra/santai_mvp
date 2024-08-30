using FluentValidation;
using FluentValidation.Validators;
using Google.Apis.Auth;
using Identity.API.Abstraction;
using Identity.API.CustomAttributes;
using Identity.API.Domain.Entities;
using Identity.API.Domain.Events;
using Identity.API.Dto;
using Identity.API.Enumerations;
using Identity.API.Extensions;
using Identity.API.Infrastructure;
using Identity.API.SeedWork;
using Identity.API.Service;
using Identity.API.Utilities;
using Identity.Contracts.Enumerations;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SantaiClaimType;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace Identity.API.Controllers;


[Route("api/v1/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private const string _controllerName = "Auth";
    private const string _apiVPhoneVerify = "v1";
    private const string _apiVCreateAccount = "v1";
    private const string _googleSigninCallbackActionName = "GoogleSignInCallBack";
    private const string _createIdentityActionName = "CreateIdentity";
    private const string _createAccountActionName = "CreateAccount";
    private const string _verifyPhoneActionName = "VerifyPhoneNumber";
    private const string _verifyLoginActionName = "VerifyLogin";
    private const string _resetPasswordActionName = "ResetPassword";
    private const string _homePageActionName = "Homepage";  
    private const string _sendOtpActionName = "SendOtp";
    private const string _staffLoginActionName = "LoginStaff";
    private const string _userLoginActionName = "LoginUser";

    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IGoogleTokenValidator _googleTokenValidator;
    private readonly IJwtTokenValidator _jwtTokenValidator;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthController> _logger;
    private readonly IMediator _mediator;
    private readonly ApplicationDbContext _dbContext;
    private readonly IOtpService _otpService;
    private readonly HttpContext? _httpContext;
    private readonly ActionMethodService _actionMethodService;
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
    private readonly IPasswordValidator<ApplicationUser> _passwordValidator;
    public AuthController(
        RoleManager<IdentityRole> roleManager,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IGoogleTokenValidator googleTokenValidator,
        IJwtTokenValidator jwtTokenValidator,
        ITokenService tokenService,
        ILogger<AuthController> logger,
        IMediator mediator,
        ApplicationDbContext dbContext,
        IOtpService otpService, 
        IHttpContextAccessor httpContextAccessor,
        ActionMethodService actionMethodService,
        IPublishEndpoint publishEndpoint,
        IPasswordHasher<ApplicationUser> passwordHasher,
        IPasswordValidator<ApplicationUser> passwordValidator)
    {
        _roleManager = roleManager;
        _signInManager = signInManager;
        _userManager = userManager;
        _googleTokenValidator = googleTokenValidator;
        _jwtTokenValidator = jwtTokenValidator;
        _tokenService = tokenService;
        _logger = logger;
        _mediator = mediator;
        _dbContext = dbContext;
        _otpService = otpService;
        _httpContext = httpContextAccessor.HttpContext;
        _actionMethodService = actionMethodService;
        _passwordHasher = passwordHasher;
        _passwordValidator = passwordValidator;
    }


    //[HttpPost("google-signin")]
    //public async Task<IResult> GoogleSignIn([AsParameters] string userType)
    //{ 
    //    if (!UserTypeConfig.AllTypes().Contains(userType))
    //    {
    //        return TypedResults.BadRequest(Result.Failure($"Unknown user type {userType}", 400));
    //    }

    //    var parameters = new
    //    {
    //        userType = userType
    //    };

    //    var properties = _signInManager.ConfigureExternalAuthenticationProperties(
    //        GoogleDefaults.AuthenticationScheme, 
    //        Url.Action(_googleSigninCallbackActionName, _controllerName, parameters, Request.Scheme));

    //    return await Task.FromResult(TypedResults.Challenge(properties, [GoogleDefaults.AuthenticationScheme]));
    //}

    [HttpPost("otp")]
    public async Task<IResult> SendOtp([FromBody] SendOtpRequest request)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {

            if (!AllowedOtpProviderType.GetAll.Contains(request.OtpProviderType))
            {
                return TypedResults.BadRequest(Result.Failure($"Unknown otp provider type '{request.OtpProviderType}'", 400));
            }

            var requestOtp = await _otpService.GetRequestOtpAsync(request.OtpRequestId);

            if (requestOtp is null)
            {
                return TypedResults.Unauthorized();
            }

            if (!_otpService.IsGenerateRequestOtpValidAsync(requestOtp, request.OtpRequestToken))
            {
                return TypedResults.Unauthorized();
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(
                    x => x.PhoneNumber == requestOtp.PhoneNumber 
                    || 
                    (
                        !string.IsNullOrWhiteSpace(requestOtp.Email) 
                        && 
                        !string.IsNullOrWhiteSpace(x.Email) 
                        && 
                        x.Email == requestOtp.Email
                    ));

            if (user is null)
            {
                return TypedResults.Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                _logger.LogWarning("Phone number is missing or invalid for user ID: {Id}", user.Id);
                return TypedResults.InternalServerError(Messages.AccountError);
            }

            var claims = await _userManager.GetClaimsAsync(user);

            if (claims is null)
            {
                _logger.LogWarning("User claims are missing or invalid for user ID: {Id}", user.Id);
                return TypedResults.InternalServerError(Result.Failure(Messages.AccountError, 500));
            }

            var roles = await _userManager.GetRolesAsync(user);

            if (roles is null)
            {
                _logger.LogWarning("User roles are missing or invalid for user ID: {Id}", user.Id);
                return TypedResults.InternalServerError(Result.Failure(Messages.AccountError, 500));
            }

            string phoneNumber = user.PhoneNumber
                ?? throw new ArgumentNullException(user.PhoneNumber, $"User {user.Id} is registered but the phone number is empty");

            if (requestOtp.OtpRequestFor == OtpRequestFor.VerifyPhoneNumber)
            {
                if (user.PhoneNumberConfirmed)
                {
                    return TypedResults.NoContent();
                }

                var otpToken = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);

                await _mediator.Publish(new OtpRequestedDomainEvent(user.PhoneNumber, user.Email, otpToken, request.OtpProviderType));

                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return TypedResults.Accepted(_verifyPhoneActionName, new
                {
                    User = new
                    {
                        Sub = user.Id,
                        Username = user.UserName,
                        PhoneNumber = phoneNumber,
                        Email = user.Email,
                        UserType = user.UserType,
                        BusinessCode = user.BusinessCode
                    },
                    Next = new
                    {
                        Link = Url.Action(_verifyPhoneActionName, _controllerName),
                        Action = _verifyPhoneActionName,
                        Method = _actionMethodService.GetHttpMethodByActionName(_verifyPhoneActionName, _controllerName)
                    }
                });
            }

            if (requestOtp.OtpRequestFor == OtpRequestFor.ForgotPassword)
            {
                (var otp, var remainingTime) = await _otpService.GenerateOtpAsync(user.PhoneNumber);
                if (otp is not null)
                {
                    await _mediator.Publish(new OtpRequestedDomainEvent(user.PhoneNumber, user.Email, otp, request.OtpProviderType));
                }

                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync(); 

                return TypedResults.Accepted(_resetPasswordActionName, new
                {
                    User = new
                    {
                        Sub = user.Id,
                        Username = user.UserName,
                        PhoneNumber = phoneNumber,
                        Email = user.Email,
                        UserType = user.UserType,
                        BusinessCode = user.BusinessCode
                    },
                    Next = new
                    {
                        Link = Url.Action(_resetPasswordActionName, _controllerName),
                        Action = _resetPasswordActionName,
                        Method = _actionMethodService.GetHttpMethodByActionName(_resetPasswordActionName, _controllerName)
                    },
                    RemainingTime = remainingTime
                });
            }

            if (requestOtp.OtpRequestFor == OtpRequestFor.VerifyLogin)
            {
                (var otp, var remainingTime) = await _otpService.GenerateOtpAsync(user.PhoneNumber);
                if (otp is not null)
                {
                    await _mediator.Publish(new OtpRequestedDomainEvent(user.PhoneNumber, user.Email, otp, request.OtpProviderType));
                }

                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return TypedResults.Ok(new
                {
                    User = new
                    {
                        Sub = user.Id,
                        Username = user.UserName,
                        PhoneNumber = phoneNumber,
                        Email = user.Email,
                        UserType = user.UserType,
                        BusinessCode = user.BusinessCode
                    },
                    Next = new
                    {
                        Link = Url.Action(_verifyLoginActionName, _controllerName),
                        Action = _verifyLoginActionName,
                        Method = _actionMethodService.GetHttpMethodByActionName(_verifyLoginActionName, _controllerName)
                    },
                    RemainingTime = remainingTime
                });
            }

            _logger.LogError("Otp request for {otpRequestFor} not found", requestOtp.OtpRequestFor.ToString());
            return TypedResults.InternalServerError(Result.Failure(Messages.InternalServerError, 500));
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Result.Failure(Messages.AccountError, 500));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Result.Failure(Messages.InternalServerError, 500));
        }
    }

    [HttpPost("reset-password")]
    public async Task<IResult> ResetPassword([FromBody] PasswordResetRequest request)
    {
        try
        { 
             var user = await _dbContext.Users.FirstOrDefaultAsync(
                    x => x.PhoneNumber == request.Identity 
                    || 
                    ( 
                        !string.IsNullOrWhiteSpace(x.Email) 
                        && 
                        x.Email == request.Identity
                    ));  

            if (user is null)
            {
                return TypedResults.NotFound(Result.Failure("User not found", 404));
            }

            if (string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                _logger.LogWarning("Phone number is missing or invalid for user ID: {Id}", user.Id);
                return TypedResults.InternalServerError(Messages.AccountError);
            } 

            var isOtpValid = await _otpService.IsOtpValidAsync(user.PhoneNumber, request.OtpCode);

            if (!isOtpValid)
            {
                return TypedResults.BadRequest(
                    Result.Failure("An error has occured", 400)
                        .WithError(new("Credential", "OTP or phone number is invalid")));
            }

            var passwordValidation = await _passwordValidator.ValidateAsync(_userManager, user, request.NewPassword);
            if (!passwordValidation.Succeeded)
            {
                return TypedResults.BadRequest(
                    Result.Failure("Password validation error", 400)
                        .WithErrors(passwordValidation.Errors.Select(x => new ErrorDetail(x.Code, x.Description)).ToList()));
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);

            var updateResult = await _userManager.UpdateAsync(user);

            if (updateResult.Succeeded)
            {
                return TypedResults.Ok();
            }

            return TypedResults.BadRequest(
                Result.Failure("Validation failed", 400)
                    .WithErrors(updateResult.Errors.Select(x => new ErrorDetail(x.Code, $"Description: {x.Description}")).ToList()));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Result.Failure(Messages.InternalServerError, 500));
        } 
    }


    [HttpPost("google-signin/callback")]
    public async Task<IResult> GoogleSignInCallBack([FromBody] GoogleSignInRequest request)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var payload = await _googleTokenValidator.ValidateAsync(request.GoogleIdToken);

            if (payload is null)
            {
                return TypedResults.BadRequest(
                    Result.Failure("Google ID token is invalid", 400));
            }


            // if user does not exist in the database, then give the CreateIdentity instruction for Regular and Mechanic user only, otherwise need to register from administrator
            var user = await _userManager.FindByEmailAsync(payload.Email);

            // if user does not exist in the database, then give the CreateIdentity instruction
            if (user is null)
            {
                return TypedResults.NotFound(
                new
                {
                    GoogleIdToken = request.GoogleIdToken,
                    Next = new
                    {
                        Link = Url.Action(_createIdentityActionName, _controllerName),
                        Action = _createIdentityActionName,
                        Method = _actionMethodService.GetHttpMethodByActionName(_createIdentityActionName, _controllerName),
                        AllowedUserTypes = GetUserTypeConfiguration.AllowedUserRegisterBySelf
                    }
                });
            }

            var claims = await _userManager.GetClaimsAsync(user);
            if (claims is null || !claims.Any())
            {
                _logger.LogWarning("User claims are missing or invalid for user ID: {Id}", user.Id);
                return TypedResults.InternalServerError(Result.Failure(Messages.AccountError, 500));
            }

            var roles = await _userManager.GetRolesAsync(user);

            if (roles is null || !roles.Any())
            {
                _logger.LogWarning("User roles are missing or invalid for user ID: {Id}", user.Id);
                return TypedResults.InternalServerError(Result.Failure(Messages.AccountError, 500));
            }


            var phoneNumber = user.PhoneNumber ?? throw new ArgumentNullException(user.PhoneNumber, $"User {user.Id} with email {user.Email} is registered but the phone number is empty");

            if (!user.PhoneNumberConfirmed)
            {
                (Guid requestId, string otpRequestToken) = await _otpService.GenerateRequestOtpAsync(
                    phoneNumber, user.Email, OtpRequestFor.VerifyPhoneNumber);

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return TypedResults.Accepted(_sendOtpActionName, new
                {
                    User = new
                    {
                        Sub = user.Id,
                        Username = user.UserName,
                        PhoneNumber = phoneNumber,
                        Email = user.Email,
                        UserType = user.UserType,
                        BusinessCode = user.BusinessCode
                    },
                    Next = new
                    {
                        Link = Url.Action(_sendOtpActionName, _controllerName),
                        Action = _sendOtpActionName,
                        Method = _actionMethodService.GetHttpMethodByActionName(_sendOtpActionName, _controllerName),
                        OtpRequestToken = otpRequestToken,
                        OtpRequestId = requestId,
                        OtpProviderTypes = AllowedOtpProviderType.GetAll
                    }
                });
            }



            (Guid newRequestId, string newOtpRequestToken) = await _otpService.GenerateRequestOtpAsync(phoneNumber, user.Email, OtpRequestFor.VerifyLogin);

            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();

            return TypedResults.Accepted(_sendOtpActionName, new
            {
                User = new
                {
                    Sub = user.Id,
                    Username = user.UserName,
                    PhoneNumber = phoneNumber,
                    Email = user.Email,
                    UserType = user.UserType,
                    BusinessCode = user.BusinessCode
                },
                Next = new
                {
                    Link = Url.Action(_sendOtpActionName, _controllerName),
                    Action = _sendOtpActionName,
                    Method = _actionMethodService.GetHttpMethodByActionName(_sendOtpActionName, _controllerName),
                    OtpRequestToken = newOtpRequestToken,
                    OtpRequestId = newRequestId,
                    OtpProviderTypes = AllowedOtpProviderType.GetAll
                }
            });
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Result.Failure(Messages.AccountError, 500));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Result.Failure(Messages.InternalServerError, 500));
        }
    }  


    [HttpPost("login/staff")]
    public async Task<IResult> LoginStaff(
        [FromBody] LoginStaffRequest request,
        [FromServices] IValidator<LoginStaffRequest> validator)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
            {
                return TypedResults.BadRequest(validation.Errors);
            }


            var phoneNumber = request.PhoneNumber.NormalizePhoneNumber(request.RegionCode);

            if (phoneNumber is null)
            {
                return TypedResults.BadRequest(
                    Result.Failure("An error has occured", 400)
                        .WithError(new("PhoneNumber", "Phone number format is invalid")));
            }

            var user = await _dbContext.Users.Where(x => x.PhoneNumber == phoneNumber).FirstOrDefaultAsync();

            if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return TypedResults.BadRequest(
                    Result.Failure("An error has occured", 400)
                        .WithError(new("Credential", "We couldn't find your account")));
            }

            if (user.UserType is not UserType.StaffUser || string.IsNullOrWhiteSpace(request.BusinessCode))
            {
                return TypedResults.BadRequest(
                    Result.Failure("An error has occured", 400)
                        .WithError(new("Credential", "We couldn't find your account")));
            }

            if (!request.BusinessCode.Equals(user.BusinessCode, StringComparison.CurrentCultureIgnoreCase))
            {
                return TypedResults.BadRequest(
                    Result.Failure("An error has occured", 400)
                        .WithError(new("Credential", "We couldn't find your account")));
            }

            if (!user.PhoneNumberConfirmed)
            {
                (Guid requestId, string otpRequestToken) = await _otpService.GenerateRequestOtpAsync(
                    phoneNumber, user.Email, OtpRequestFor.VerifyPhoneNumber);

                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return TypedResults.Accepted(_sendOtpActionName, new
                { 
                    Next = new
                    {
                        Link = Url.Action(_sendOtpActionName, _controllerName),
                        Action = _sendOtpActionName,
                        Method = _actionMethodService.GetHttpMethodByActionName(_sendOtpActionName, _controllerName),
                        OtpRequestToken = otpRequestToken,
                        OtpRequestId = requestId,
                        OtpProviderTypes = AllowedOtpProviderType.GetAll
                    }
                });
            }


            var signInResult = await _signInManager.PasswordSignInAsync(user, request.Password, false, lockoutOnFailure: false);

            if (!signInResult.Succeeded)
            {
                return TypedResults.Unauthorized();
            }


            var claims = await _userManager.GetClaimsAsync(user);

            if (claims is null || !claims.Any())
            {
                _logger.LogWarning("User claims are missing or invalid for user ID: {UserId}", user.Id);
                return TypedResults.InternalServerError(Result.Failure(Messages.AccountError, 500));
            }

            var roles = await _userManager.GetRolesAsync(user);

            if (roles is null || !roles.Any())
            {
                _logger.LogWarning("User roles are missing or invalid for user ID: {UserId}", user.Id);
                return TypedResults.InternalServerError(Result.Failure(Messages.AccountError, 500));
            }


            (Guid newRequestId, string newOtpRequestToken) = await _otpService.GenerateRequestOtpAsync(phoneNumber, user.Email, OtpRequestFor.VerifyLogin);

            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();

            return TypedResults.Accepted(_sendOtpActionName, new
            {
                User = new
                {
                    Sub = user.Id,
                    Username = user.UserName,
                    PhoneNumber = phoneNumber,
                    Email = user.Email,
                    UserType = user.UserType,
                    BusinessCode = user.BusinessCode
                },
                Next = new
                {
                    Link = Url.Action(_sendOtpActionName, _controllerName),
                    Action = _sendOtpActionName,
                    Method = _actionMethodService.GetHttpMethodByActionName(_sendOtpActionName, _controllerName),
                    OtpRequestToken = newOtpRequestToken,
                    OtpRequestId = newRequestId,
                    OtpProviderTypes = AllowedOtpProviderType.GetAll
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Result.Failure(Messages.InternalServerError, 500));
        }
    }


    [HttpPost("login/user")]
    public async Task<IResult> LoginUser(
        [FromBody] LoginUserRequest request,
        [FromServices] IValidator<LoginUserRequest> validator)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
            {
                return TypedResults.BadRequest(validation.Errors);
            }


            var phoneNumber = request.PhoneNumber.NormalizePhoneNumber(request.RegionCode);

            if (phoneNumber is null)
            {
                return TypedResults.BadRequest(
                    Result.Failure("An error has occured", 400)
                        .WithError(new("PhoneNumber", "Phone number format is invalid")));
            }

            var user = await _userManager.FindByNameAsync(phoneNumber);

            if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return TypedResults.BadRequest(
                    Result.Failure("An error has occured", 400)
                        .WithError(new("Credential", "We couldn't find your account")));
            }

            if (user.UserType == UserType.StaffUser)
            {
                return TypedResults.Ok(new
                {
                    Next = new
                    {
                        Link = Url.Action(_staffLoginActionName, _controllerName),
                        Action = _staffLoginActionName,
                        Method = _actionMethodService.GetHttpMethodByActionName(_staffLoginActionName, _controllerName)
                    }
                });
            }


            if (!user.PhoneNumberConfirmed)
            {
                (Guid requestId, string otpRequestToken) = await _otpService.GenerateRequestOtpAsync(
                    phoneNumber, user.Email, OtpRequestFor.VerifyPhoneNumber);

                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return TypedResults.Accepted(_sendOtpActionName, new
                { 
                    Next = new
                    {
                        Link = Url.Action(_sendOtpActionName, _controllerName),
                        Action = _sendOtpActionName,
                        Method = _actionMethodService.GetHttpMethodByActionName(_sendOtpActionName, _controllerName),
                        OtpRequestToken = otpRequestToken,
                        OtpRequestId = requestId,
                        OtpProviderTypes = AllowedOtpProviderType.GetAll
                    }
                });
            }


            var signInResult = await _signInManager.PasswordSignInAsync(user, request.Password, false, lockoutOnFailure: false);

            if (!signInResult.Succeeded)
            {
                return TypedResults.Unauthorized();
            }




            var claims = await _userManager.GetClaimsAsync(user);

            if (claims is null || !claims.Any())
            {
                _logger.LogWarning("User claims are missing or invalid for user ID: {Id}", user.Id);
                return TypedResults.InternalServerError(Result.Failure(Messages.AccountError, 500));
            }

            var roles = await _userManager.GetRolesAsync(user);

            if (roles is null || !roles.Any())
            {
                _logger.LogWarning("User roles are missing or invalid for user ID: {Id}", user.Id);
                return TypedResults.InternalServerError(Result.Failure(Messages.AccountError, 500));
            }



            (Guid newRequestId, string newOtpRequestToken) = await _otpService.GenerateRequestOtpAsync(phoneNumber, user.Email, OtpRequestFor.VerifyLogin);

            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();

            return TypedResults.Accepted(_sendOtpActionName, new
            {
                User = new
                {
                    Sub = user.Id,
                    Username = user.UserName,
                    PhoneNumber = phoneNumber,
                    Email = user.Email,
                    UserType = user.UserType,
                    BusinessCode = user.BusinessCode
                },

                Next = new
                {
                    Link = Url.Action(_sendOtpActionName, _controllerName),
                    Action = _sendOtpActionName,
                    Method = _actionMethodService.GetHttpMethodByActionName(_sendOtpActionName, _controllerName),
                    OtpRequestToken = newOtpRequestToken,
                    OtpRequestId = newRequestId,
                    OtpProviderTypes = AllowedOtpProviderType.GetAll
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Result.Failure(Messages.InternalServerError, 500));
        }
    }

    [HttpPost("register")]
    [Idempotency(nameof(CreateIdentity))]
    public async Task<IResult> CreateIdentity([FromBody] RegisterUserRequest request)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            if (!GetUserTypeConfiguration.AllowedUserRegisterBySelf.Contains(request.UserType))
            {
                return TypedResults.BadRequest(
                    Result.Failure("An error has occured", 500)
                        .WithError(new("UserType", $"Unknown user type {request.UserType}")));
            }

            var phoneNumber = request.PhoneNumber.NormalizePhoneNumber(request.RegionCode);

            if (phoneNumber is null)
            {
                return TypedResults.BadRequest(
                   Result.Failure("An error has occured", 400)
                       .WithError(new("PhoneNumber", "Please provide valid phone number")));
            }



            GoogleJsonWebSignature.Payload? payload = null;

            if (!string.IsNullOrWhiteSpace(request.GoogleIdToken))
            {
                payload = await _googleTokenValidator.ValidateAsync(request.GoogleIdToken);

                var userByEmailClaim = await _userManager.FindByEmailAsync(payload.Email);

                if (userByEmailClaim is not null)
                {
                    if (userByEmailClaim.PhoneNumber is null)
                    {
                        _logger.LogWarning("User with id {Id} has email but the phone number is empty", userByEmailClaim.Id);
                        return TypedResults.InternalServerError(Result.Failure(Messages.AccountError, 500));
                    }


                    var claims = await _userManager.GetClaimsAsync(userByEmailClaim);

                    if (claims is null || !claims.Any())
                    {
                        _logger.LogWarning("User claims are missing or invalid for user ID: {Id}", userByEmailClaim.Id);
                        return TypedResults.InternalServerError(Result.Failure(Messages.AccountError, 500));
                    }

                    var roles = await _userManager.GetRolesAsync(userByEmailClaim);

                    if (roles is null || !roles.Any())
                    {
                        _logger.LogWarning("User roles are missing or invalid for user ID: {Id}", userByEmailClaim.Id);
                        return TypedResults.InternalServerError(Result.Failure(Messages.AccountError, 500));
                    }


                    if (!userByEmailClaim.PhoneNumber.Equals(request.PhoneNumber))
                    {
                        return TypedResults.Conflict(Result.Failure($"Email address already used by another account.", 409));
                    }

                    if (!userByEmailClaim.PhoneNumberConfirmed)
                    {
                        (Guid newRequestId, string newOtpRequestToken) = await _otpService.GenerateRequestOtpAsync(phoneNumber, userByEmailClaim.Email, OtpRequestFor.VerifyPhoneNumber);

                        await _dbContext.SaveChangesAsync();
                        await transaction.CommitAsync();

                        return TypedResults.Accepted(_sendOtpActionName, new
                        {
                            User = new
                            {
                                Sub = userByEmailClaim.Id,
                                Username = userByEmailClaim.PhoneNumber,
                                PhoneNumber = userByEmailClaim.PhoneNumber,
                                Email = userByEmailClaim.Email,
                                UserType = userByEmailClaim.UserType,
                                BusinessCode = userByEmailClaim.BusinessCode
                            },
                            Next = new
                            {
                                Link = Url.Action(_sendOtpActionName, _controllerName),
                                Action = _sendOtpActionName,
                                Method = _actionMethodService.GetHttpMethodByActionName(_sendOtpActionName, _controllerName),
                                OtpRequestToken = newOtpRequestToken,
                                OtpRequestId = newRequestId,
                                OtpProviderTypes = AllowedOtpProviderType.GetAll
                            }
                        });
                    }

                    return TypedResults.Conflict(Result.Failure("Email is already in use", 409));
                }
            }

            var user = await _userManager.FindByNameAsync(phoneNumber);

            if (user != null)
            {
                return TypedResults.Conflict(
                    Result.Failure("An error has occurred", 409)
                       .WithError(new("PhoneNumber", "Phone number is already in use")));
            }

            var newUser = new ApplicationUser
            {
                PhoneNumber = phoneNumber,
                UserName = phoneNumber,
                Email = payload?.Email,
                UserType = request.UserType
            };

            var addUserResult = await _userManager.CreateAsync(newUser, request.Password);

            if (!addUserResult.Succeeded)
            {
                var errors = addUserResult.Errors.Select(e => new ErrorDetail(e.Code, e.Description));
                return TypedResults.InternalServerError(
                   Result.Failure(Messages.InternalServerError, 500)
                      .WithErrors(errors.ToList()));
            }

            if (payload?.Email is not null)
            {
                await _mediator.Publish(
                    new EmailAssignedToAUserDomainEvent(
                        Guid.Parse(newUser.Id), payload.Email, newUser.UserType));

                var userInfoLogin = new UserLoginInfo("google", payload.Subject, "google");
                await _userManager.AddLoginAsync(newUser, userInfoLogin);
                newUser.EmailConfirmed = true;
                await _userManager.UpdateAsync(newUser);
            }

            var assignRoleResult = await _userManager.AddToRoleAsync(newUser, request.UserType.ToString());
            if (!assignRoleResult.Succeeded)
            {
                var errors = assignRoleResult.Errors.Select(e => new ErrorDetail(e.Code, e.Description));
                return TypedResults.InternalServerError(
                   Result.Failure(Messages.InternalServerError, 500)
                      .WithErrors(errors.ToList()));
            }

            var addClaimsResult = await AddClaimsToDatabase(newUser);
            if (!addClaimsResult.Succeeded)
            {
                var errors = addClaimsResult.Errors.Select(e => new ErrorDetail(e.Code, e.Description));
                return TypedResults.InternalServerError(
                   Result.Failure(Messages.InternalServerError, 500)
                      .WithErrors(errors.ToList()));
            }

            (Guid requestId, string otpRequestToken) = await _otpService.GenerateRequestOtpAsync(
                phoneNumber, newUser.Email, OtpRequestFor.VerifyPhoneNumber);

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return TypedResults.Accepted(_sendOtpActionName, new
            {
                User = new
                {
                    Sub = newUser.Id,
                    Username = newUser.UserName,
                    PhoneNumber = newUser.PhoneNumber,
                    Email = newUser.Email,
                    UserType = newUser.UserType,
                    BusinessCode = newUser.BusinessCode
                },
                Next = new
                {
                    Link = Url.Action(_sendOtpActionName, _controllerName),
                    Action = _sendOtpActionName,
                    Method = _actionMethodService.GetHttpMethodByActionName(_sendOtpActionName, _controllerName),
                    OtpRequestToken = otpRequestToken,
                    OtpRequestId = requestId,
                    OtpProviderTypes = AllowedOtpProviderType.GetAll
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Result.Failure(Messages.InternalServerError, 500));
        }
    }  



    [HttpPost("refresh-token")]
    public async Task<IResult> RefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest)
    {
        try 
        {
            if (string.IsNullOrWhiteSpace(refreshTokenRequest.RefreshToken))
            {
                return TypedResults.BadRequest(Result.Failure("Refresh token must not be null", 400));
            } 

            if (_httpContext is null)
            {
                _logger.LogError("Http context is null");
                return TypedResults.InternalServerError(Result.Failure(Messages.InternalServerError, 500));
            }

            var bearer = _httpContext.GetBearerToken();

            if (bearer is null)
            {
                return TypedResults.BadRequest(Result.Failure("Access token must not be null", 400));
            }
             

            if (await _tokenService.IsRefreshTokenBlacklisted(refreshTokenRequest.RefreshToken) || await _tokenService.IsAccessTokenBlacklisted(bearer))
            {
                return TypedResults.Unauthorized();
            }

            var refreshToken = await _tokenService.RotateRefreshTokenAsync(refreshTokenRequest.RefreshToken); 
            if (refreshToken is null)
            {
                return TypedResults.Unauthorized();
            }

            await _tokenService.BlackListRefreshTokenAsync(refreshTokenRequest.RefreshToken);
            await _tokenService.BlackListAccessTokenAsync(bearer);

            var user = await _userManager.FindByIdAsync(refreshToken.UserId); 

            if (user is null)
            {
                return TypedResults.Unauthorized();
            } 

            var claims = await _userManager.GetClaimsAsync(user);  

            if (claims is null || !claims.Any())
            { 
                _logger.LogWarning("User claims are missing or invalid for user ID: {Id}", user.Id);
                return TypedResults.InternalServerError(Result.Failure(Messages.AccountError, 500));
            } 

            var roles = await _userManager.GetRolesAsync(user);

            if (roles is null || !roles.Any())
            {
                _logger.LogWarning("User roles are missing or invalid for user ID: {Id}", user.Id);
                return TypedResults.InternalServerError(Result.Failure(Messages.AccountError, 500));
            }


            var claimsIdentity = new ClaimsIdentity(claims); 
            var accessToken = _tokenService.GenerateAccessToken(claimsIdentity); 

            return TypedResults.Ok(new
            {
                Token = accessToken,
                RefreshToken = refreshToken.Token,
                User = new
                {
                    Sub = user.Id,
                    Username = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    UserType = user.UserType,
                    BusinessCode = user.BusinessCode
                }
            });  
        }
        catch(SecurityTokenException)
        {
            return TypedResults.Unauthorized();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Result.Failure(Messages.InternalServerError, 500));
        }
    }


    [HttpPost("forgot-password")]
    public async Task<IResult> ForgotPassword( 
        [FromBody] PasswordForgotRequest request)
    {
        try
        {  
            ApplicationUser? user = null;
            IdentityType? identityType = null; 

            if (PhoneNumberValidator.IsValid(request.Identity))
            {
                user = await _userManager.FindByNameAsync(request.Identity);
                identityType = IdentityType.PhoneNumber; 
            }

            if (EmailAddress.IsValidEmail(request.Identity))
            {
                user = await _userManager.FindByEmailAsync(request.Identity);
                identityType = IdentityType.Email; 
            }

            if (identityType is null)
            {
                return TypedResults.BadRequest(
                    Result.Failure("Please give the valid email address or phone number", 400)
                        .WithError(new("Identity", "Phone number or email address is invalid")));
            }

            if (user is null)
            {
                return TypedResults.NotFound(Result.Failure("User not found", 404));
            }

            if (!AllowedOtpProviderType.GetAllByName.TryGetValue(
               (IdentityType)identityType,
               out var otpProviderTypes))
            {
                _logger.LogError("Identity type {identityType} does not have configuration", identityType);
                return TypedResults.InternalServerError(Messages.InternalServerError);
            }

            (Guid requestId, string otpRequestToken) = await _otpService.GenerateRequestOtpAsync(
               request.Identity, user.Email, OtpRequestFor.ForgotPassword);  
             
            return TypedResults.Accepted(_sendOtpActionName, new
            {
                User = new
                {
                    Sub = user.Id,
                    PhoneNumber = user.PhoneNumber
                },
                Next = new
                {
                    Link = Url.Action(_sendOtpActionName, _controllerName),
                    Action = _sendOtpActionName,
                    Method = _actionMethodService.GetHttpMethodByActionName(_sendOtpActionName, _controllerName),
                    OtpRequestToken = otpRequestToken,
                    OtpRequestId = requestId,
                    OtpProviderTypes = otpProviderTypes
                }
            }); 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Result.Failure(Messages.AccountError, 500));
        } 
    }

     
    [HttpPost("verify/token")]
    public async Task<IResult> VerifyToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                return TypedResults.BadRequest(Result.Failure("Refresh token must not null", 400));
            }

            if (_httpContext is null)
            {
                _logger.LogError("Http context is null");
                return TypedResults.InternalServerError(Result.Failure(Messages.InternalServerError, 500));
            }

            var bearer = _httpContext.GetBearerToken();

            if (bearer is null)
            {
                return TypedResults.BadRequest(Result.Failure("Access token must not be null", 400));
            }

            var isAccessTokenBlocked = await _tokenService.IsAccessTokenBlacklisted(bearer);
            var isRefreshTokenBlocked = await _tokenService.IsRefreshTokenBlacklisted(request.RefreshToken);

            return isAccessTokenBlocked || isRefreshTokenBlocked ? TypedResults.Forbid() : TypedResults.Ok();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Result.Failure(Messages.InternalServerError, 500));
        } 
    }



    [HttpPatch("verify/login")]
    public async Task<IResult> VerifyLogin(
        [FromBody] VerifyLoginRequest request,
        IValidator<VerifyLoginRequest> validator)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
            {
                return TypedResults.BadRequest(validation.Errors);
            }

            var isOtpValid = await _otpService.IsOtpValidAsync(request.PhoneNumber, request.Token);

            if (!isOtpValid)
            {
                return TypedResults.BadRequest(
                    Result.Failure("An error has occured", 400)
                        .WithError(new("Credential", "OTP or phone number is invalid")));
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber);

            if (user is null)
            {
                return TypedResults.Unauthorized();
            }


            var claims = await _userManager.GetClaimsAsync(user);

            if (claims is null || !claims.Any())
            {
                _logger.LogWarning("User claims are missing or invalid for user ID: {Id}", user.Id);
                return TypedResults.InternalServerError(Result.Failure(Messages.AccountError, 500));
            }

            var roles = await _userManager.GetRolesAsync(user);

            if (roles is null || !roles.Any())
            {
                _logger.LogWarning("User roles are missing or invalid for user ID: {Id}", user.Id);
                return TypedResults.InternalServerError(Result.Failure(Messages.AccountError, 500));
            }

            var accessToken = _tokenService.GenerateAccessToken(new ClaimsIdentity(claims));

            var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id);

            await _otpService.RemoveOtpAsync(request.PhoneNumber);

            string redirectTo = user.IsAccountRegistered ? _homePageActionName : _createAccountActionName;

            await transaction.CommitAsync();

            return TypedResults.Ok(
            new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = new
                {
                    Sub = user.Id,
                    Username = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    UserType = user.UserType,
                    BusinessCode = user.BusinessCode
                },
                Next = new
                {
                    Action = redirectTo
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Result.Failure(Messages.InternalServerError, 500));
        }
    }




    [HttpPatch("verify/phone-number")]
    public async Task<IResult> VerifyPhoneNumber(
        [FromBody] VerifyPhoneNumberRequest request,
        [FromServices] IValidator<VerifyPhoneNumberRequest> validator)
    {
        var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
            {
                return TypedResults.BadRequest(validation.Errors);
            }

            var user = await _userManager.FindByNameAsync(request.PhoneNumber);


            if (user is null)
            {
                return TypedResults.Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                _logger.LogWarning("User phone number is missing or invalid for user ID: {Id}", user.Id);
                return TypedResults.InternalServerError(Result.Failure(Messages.AccountError, 500));
            }

            if (user.PhoneNumberConfirmed)
            {
                return TypedResults.NoContent();
            }


            var changeResult = await _userManager.ChangePhoneNumberAsync(user, request.PhoneNumber, request.Token);

            if (!changeResult.Succeeded)
            {
                var errors = changeResult.Errors.Select(e => new ErrorDetail(e.Code, e.Description));
                return TypedResults.InternalServerError(
                   Result.Failure(Messages.InternalServerError, 500)
                      .WithErrors(errors.ToList()));
            }

            await _mediator.Publish(new PhoneNumberConfirmedDomainEvent(Guid.Parse(user.Id), user.PhoneNumber, user.UserType));

            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();

            return TypedResults.NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, 500));
        } 
    }



    [Authorize]
    [HttpPost("logout")]
    public async Task<IResult> Logout([FromBody] RefreshTokenRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                return TypedResults.BadRequest(Result.Failure("Refresh token must not null", 400));
            }

            if (_httpContext is null)
            {
                _logger.LogError("Http context is null");
                return TypedResults.InternalServerError(
                    Result.Failure(Messages.InternalServerError, 500));
            }

            var bearer = _httpContext.GetBearerToken();

            if (bearer is null)
            {
                return TypedResults.Unauthorized();
            }

            await _tokenService.BlackListRefreshTokenAsync(request.RefreshToken);
            await _tokenService.BlackListAccessTokenAsync(bearer);

            return TypedResults.NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Result.Failure(Messages.InternalServerError, 500));
        }
    }


    private async Task<IdentityResult> AddClaimsToDatabase(ApplicationUser user)
    {
        if(user.PhoneNumber is null)
        {
            throw new ArgumentNullException(user.PhoneNumber);
        }

        var claims = new List<Claim>()
        {
            new (JwtRegisteredClaimNames.Sub, user.Id),
            new (ClaimTypes.Name, user.PhoneNumber),
            new (ClaimTypes.MobilePhone, user.PhoneNumber),
            new (SantaiClaimTypes.UserType, user.UserType.ToString()),
            new (ClaimTypes.Role, user.UserType.ToString())
        };

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
        }

        if (!string.IsNullOrWhiteSpace(user.BusinessCode))
        {
            claims.Add(new Claim(SantaiClaimTypes.BusinessCode, user.BusinessCode));
        } 

        return await _userManager.AddClaimsAsync(user, claims);
    }   
}
