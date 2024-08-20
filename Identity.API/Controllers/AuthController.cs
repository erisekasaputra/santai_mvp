using FluentValidation;
using Google.Apis.Auth;
using Identity.API.Abstraction;
using Identity.API.Domain.Entities;
using Identity.API.Domain.Events;
using Identity.API.Dto;
using Identity.API.Enumerations;
using Identity.API.Extensions;
using Identity.API.Infrastructure;
using Identity.API.SeedWork;
using Identity.Contracts;
using MediatR;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SantaiClaimType; 
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
    private const string _homePageActionName = "Homepage";  
    private const string _sendOtpActionName = "SendOtp";

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
        IHttpContextAccessor httpContextAccessor)
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
    }


    [HttpPost("google-signin")]
    public async Task<IResult> GoogleSignIn([AsParameters] string userType)
    { 
        if (!UserTypes.AllTypes().Contains(userType))
        {
            return TypedResults.BadRequest(Result.Failure($"Unknown user type {userType}", 400));
        }

        var parameters = new
        {
            userType = userType
        };

        var properties = _signInManager.ConfigureExternalAuthenticationProperties(
            GoogleDefaults.AuthenticationScheme, 
            Url.Action(_googleSigninCallbackActionName, _controllerName, parameters, Request.Scheme));

        return await Task.FromResult(TypedResults.Challenge(properties, [GoogleDefaults.AuthenticationScheme]));
    }

    [HttpPost("otp")]
    public async Task<IResult> SendOtp(
        [AsParameters] string otpProviderType,
        [FromBody] SendOtpRequest request)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync<IResult>(async () =>
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {  
                if (!OtpProviderType.AllowedProviderType.Contains(otpProviderType))
                {
                    return TypedResults.BadRequest(Result.Failure($"Unknown otp provider type '{otpProviderType}'", 400));
                }

                var requestOtp = await _otpService.GetRequestOtpAsync(request.RequestOtpId);

                if (requestOtp is null)
                {
                    return TypedResults.Unauthorized();
                }

                if (!_otpService.IsGenerateRequestOtpValidAsync(requestOtp, request.RequestOtpToken))
                {
                    return TypedResults.Unauthorized();
                }

                var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.PhoneNumber == requestOtp.PhoneNumber); 

                if (user is null)
                {
                    return TypedResults.Unauthorized();
                }

                string phoneNumber = user.PhoneNumber ?? throw new ArgumentNullException(user.PhoneNumber, $"User {user.Id} is registered but the phone number is empty");

                if (requestOtp.OtpRequestFor == OtpRequestFor.VerifyPhoneNumber)
                {
                    if (user.PhoneNumberConfirmed)
                    {
                        return TypedResults.NoContent();
                    }

                    var otpToken = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber!);

                    await _mediator.Publish(new OtpRequestedDomainEvent(user.PhoneNumber!, otpToken, otpProviderType));

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
                        Link = Url.Action(_verifyPhoneActionName, _controllerName)
                    }); 
                }

                if (requestOtp.OtpRequestFor == OtpRequestFor.VerifyLogin)
                { 
                    (var otp, var remainingTime) = await _otpService.GenerateOtpAsync(user.PhoneNumber!);
                    if (otp is not null)
                    {
                        await _mediator.Publish(new OtpRequestedDomainEvent(user.PhoneNumber!, otp, otpProviderType));
                    }

                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return TypedResults.Ok(new
                    {
                        User = new
                        {
                            Sub = user.Id,
                            Username = user.UserName,
                            PhoneNumber = user.PhoneNumber!,
                            Email = user.Email, 
                            UserType = user.UserType,
                            BusinessCode = user.BusinessCode
                        },
                        Link = Url.Action(_verifyLoginActionName, _controllerName),
                        RemainingLockingOtp = remainingTime
                    });
                }

                _logger.LogError("Otp request for {0} not found", requestOtp.OtpRequestFor.ToString());
                return TypedResults.InternalServerError(Messages.InternalServerError);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, ex.InnerException?.Message);
                return TypedResults.InternalServerError(Messages.AccountError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.InnerException?.Message);
                return TypedResults.InternalServerError(Messages.InternalServerError);
            }
        });    
    } 


    [HttpPost("google-signin/callback")]
    public async Task<IResult> GoogleSignInCallBack(
        [AsParameters] string userType,
        [FromBody] GoogleSignInRequest request)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync<IResult>(async () =>
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                if (!UserTypes.AllTypes().Contains(userType))
                {
                    return TypedResults.BadRequest(
                        Result.Failure($"Unknown user type {userType}", 400));
                }


                var payload = await _googleTokenValidator.ValidateAsync(request.GoogleIdToken);

                if (payload is null)
                {
                    return TypedResults.BadRequest(
                        Result.Failure("Google ID token is invalid", 400));
                }


                // if user does not exist in the database, then give the CreateIdentity instruction for Regular and Mechanic user only, otherwise need to register from administrator
                var user = await _userManager.FindByEmailAsync(payload.Email);

                if (user is null && !UserTypes.AllowedTypes().Contains(userType))
                {
                    return TypedResults.BadRequest(
                        Result.Failure("An error has occured", 500)
                            .WithError(new("UserType", $"User with type of '{userType}' must registered from Santai administrator")));
                }

                // if user does not exist in the database, then give the CreateIdentity instruction
                if (user is null)
                {
                    var parameters = new { userType = userType };

                    return TypedResults.Accepted(_createIdentityActionName,
                    new
                    {
                        GoogleIdToken = request.GoogleIdToken,
                        Link = Url.Action(_createIdentityActionName, _controllerName, parameters, Request.Scheme),
                        UserTypes = UserTypes.AllowedTypes()
                    });
                }


                var claims = await _userManager.GetClaimsAsync(user);

                if (claims is null || claims.Count == 0)
                {
                    await UserDeletion(user);
                    return TypedResults.Unauthorized();
                }


                var phoneNumber = user.PhoneNumber ?? throw new ArgumentNullException(user.PhoneNumber, $"User {user.Id} with email {user.Email} is registered but the phone number is empty");

                if (!user.PhoneNumberConfirmed)
                {  
                    (Guid requestId, string otpRequestToken) = await _otpService.GenerateRequestOtpAsync(phoneNumber, OtpRequestFor.VerifyPhoneNumber);

                    await _dbContext.SaveChangesAsync(); 
                    await transaction.CommitAsync(); 

                    var paramsOtpType1 = new { otpProviderType = "" };

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
                        Link = Url.Action(_sendOtpActionName, _controllerName, paramsOtpType1, Request.Scheme),
                        OtpRequestToken = otpRequestToken,
                        RequestId = requestId
                    }); 
                }



                var userClaims = await _userManager.GetClaimsAsync(user);

                if (!userClaims.Any())
                { 
                    await UserDeletion(user);

                    return TypedResults.Unauthorized();
                } 

                (Guid newRequestId, string newOtpRequestToken) = await _otpService.GenerateRequestOtpAsync(phoneNumber, OtpRequestFor.VerifyLogin);

                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();


                var paramsOtpType2 = new { otpProviderType = "" };

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
                    Link = Url.Action(_sendOtpActionName, _controllerName, paramsOtpType2, Request.Scheme),
                    OtpRequestToken = newOtpRequestToken,
                    RequestId = newRequestId
                }); 
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, ex.InnerException?.Message);
                return TypedResults.InternalServerError(Messages.AccountError); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.InnerException?.Message);
                return TypedResults.InternalServerError(Messages.InternalServerError);
            }
        }); 
    }  


    [HttpPost("login/staff")]
    public async Task<IResult> InternalLoginUser(
        [FromBody] LoginStaffRequest request,
        [FromServices] IValidator<LoginStaffRequest> validator)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync<IResult>(async () =>
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
                            .WithError(new("Credentials", "We couldn't find an account with the given details")));
                }

                if (user.UserType is not UserTypes.StaffUserRole || string.IsNullOrWhiteSpace(request.BusinessCode))
                {
                    return TypedResults.BadRequest(
                        Result.Failure("An error has occured", 400)
                            .WithError(new("Credentials", "We couldn't find an account with the given details")));
                }

                if (!request.BusinessCode.Equals(user.BusinessCode, StringComparison.CurrentCultureIgnoreCase))
                {
                    return TypedResults.BadRequest(
                        Result.Failure("An error has occured", 400)
                            .WithError(new("Credentials", "We couldn't find an account with the given details")));
                }

                if (!user.PhoneNumberConfirmed)
                {
                    (Guid requestId, string otpRequestToken) = await _otpService.GenerateRequestOtpAsync(phoneNumber, OtpRequestFor.VerifyPhoneNumber);

                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();


                    var paramsOtpType1 = new { otpProviderType = "" };

                    return TypedResults.Accepted(_sendOtpActionName, new
                    {
                        User = new
                        {
                            Sub = user.Id,
                            Username = user.UserName,
                            PhoneNumber = user.PhoneNumber,
                            Email = user.Email,
                            BusinessCode = user.BusinessCode,
                            UserTypes = user.UserType
                        },
                        Link = Url.Action(_sendOtpActionName, _controllerName, paramsOtpType1, Request.Scheme),
                        OtpRequestToken = otpRequestToken,
                        RequestId = requestId
                    });
                }


                var signInResult = await _signInManager.PasswordSignInAsync(user, request.Password, false, lockoutOnFailure: false);

                if (!signInResult.Succeeded)
                {
                    return TypedResults.Unauthorized();
                }




                var claims = await _userManager.GetClaimsAsync(user);

                if (claims is null || claims.Count == 0)
                {
                    await UserDeletion(user);

                    _logger.LogError("User {0} has user data but does not have user claim and user roles", user.Id);

                    return TypedResults.InternalServerError(Messages.InternalServerError);
                }

                (Guid newRequestId, string newOtpRequestToken) = await _otpService.GenerateRequestOtpAsync(phoneNumber, OtpRequestFor.VerifyLogin);

                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                var paramsOtpType2 = new { otpProviderType = "" };

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
                    Link = Url.Action(_sendOtpActionName, _controllerName, paramsOtpType2, Request.Scheme),
                    OtpRequestToken = newOtpRequestToken,
                    RequestId = newRequestId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.InnerException?.Message);
                return TypedResults.InternalServerError(Messages.InternalServerError);
            }
        });
    }


    [HttpPost("login/user")]
    public async Task<IResult> InternalLoginUser(
        [FromBody] LoginUserRequest request,
        [FromServices] IValidator<LoginUserRequest> validator)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync<IResult>(async () =>
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
                            .WithError(new("Credentials", "We couldn't find an account with the given details")));
                }
                 
                if (user.UserType == UserTypes.StaffUserRole)
                {
                    return TypedResults.BadRequest(
                        Result.Failure("Staff must login via Staff Login page", 400));
                }

                if (!user.PhoneNumberConfirmed)
                {
                    (Guid requestId, string otpRequestToken) = await _otpService.GenerateRequestOtpAsync(phoneNumber, OtpRequestFor.VerifyPhoneNumber); 
                     
                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();


                    var paramsOtpType1 = new { otpProviderType = "" };

                    return TypedResults.Accepted(_sendOtpActionName, new
                    {
                        User = new
                        {
                            Sub = user.Id,
                            Username = user.UserName,
                            PhoneNumber = user.PhoneNumber,
                            Email = user.Email,
                            UserType = user.UserType,
                            BusinessCode = user.BusinessCode
                        },
                        Link = Url.Action(_sendOtpActionName, _controllerName, paramsOtpType1),
                        OtpRequestToken = otpRequestToken,
                        RequestId = requestId  
                    });
                }


                var signInResult = await _signInManager.PasswordSignInAsync(user, request.Password, false, lockoutOnFailure: false);

                if (!signInResult.Succeeded)
                {
                    return TypedResults.Unauthorized();
                }




                var claims = await _userManager.GetClaimsAsync(user);

                if (claims is null || claims.Count == 0)
                {
                    await UserDeletion(user);

                    _logger.LogError("User {0} has user data but does not have user claim and user roles", user.Id);
                    
                    return TypedResults.InternalServerError(Messages.InternalServerError);
                } 

                (Guid newRequestId, string newOtpRequestToken) = await _otpService.GenerateRequestOtpAsync(phoneNumber, OtpRequestFor.VerifyLogin);

                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                var paramsOtpType2 = new { otpProviderType = "" };

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
                    Link = Url.Action(_sendOtpActionName, _controllerName, paramsOtpType2, Request.Scheme),
                    OtpRequestToken = newOtpRequestToken,
                    RequestId = newRequestId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.InnerException?.Message);
                return TypedResults.InternalServerError(Messages.InternalServerError);
            }
        }); 
    } 
   

    [HttpPost("register/user")]
    public async Task<IResult> CreateIdentity(
        [AsParameters] string userType, 
        [FromBody] RegisterUserRequest request)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync<IResult>(async () =>
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                if (!UserTypes.AllowedTypes().Contains(userType))
                {
                    return TypedResults.BadRequest(
                        Result.Failure("An error has occured", 500)
                            .WithError(new("UserType", $"Unknown user type {userType}")));
                }

                if (!UserTypes.AllTypes().Contains(userType))
                {
                    _logger.LogError("User type {0} does not exists in the list of user types", userType);
                    return TypedResults.InternalServerError(Messages.InternalServerError);
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
                            _logger.LogError("User with id {0} has email but the phone number is empty", userByEmailClaim.Id);
                            return TypedResults.InternalServerError(Messages.AccountError);
                        }

                        var userClaims = await _userManager.GetClaimsAsync(userByEmailClaim);
                        if (userClaims is null)
                        {
                            await UserDeletion(userByEmailClaim);
                            return TypedResults.InternalServerError(Messages.AccountError);
                        }

                        if (!userByEmailClaim.PhoneNumber.Equals(request.PhoneNumber))
                        {
                            return TypedResults.Conflict(Result.Failure($"Email address already used by another account.", 409));
                        }

                        if (!userByEmailClaim.PhoneNumberConfirmed)
                        {
                            (Guid newRequestId, string newOtpRequestToken) = await _otpService.GenerateRequestOtpAsync(phoneNumber, OtpRequestFor.VerifyPhoneNumber);

                            await _dbContext.SaveChangesAsync();
                            await transaction.CommitAsync(); 

                            var paramsOtpType1 = new { otpProviderType = "" };

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
                                Link = Url.Action(_sendOtpActionName, _controllerName, paramsOtpType1, Request.Scheme),
                                OtpRequestToken = newOtpRequestToken,
                                RequestId = newRequestId
                            });
                        }

                        await _dbContext.SaveChangesAsync();
                        await transaction.CommitAsync();

                        return TypedResults.Created(Url.Action(_verifyLoginActionName), new
                        {
                            User = new
                            {
                                Sub = userByEmailClaim.Id,
                                Username = userByEmailClaim.UserName,
                                PhoneNumber = userByEmailClaim.PhoneNumber,
                                Email = userByEmailClaim.Email,
                                UserTypes = userByEmailClaim.UserType,
                                BusinessCode = userByEmailClaim.BusinessCode
                            }
                        });
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
                    UserType = userType
                };

                var addUserResult = await _userManager.CreateAsync(newUser, request.Password);

                if (!addUserResult.Succeeded)
                {
                    var errors = addUserResult.Errors.Select(e => new ErrorDetail(e.Code, e.Description));
                    return TypedResults.InternalServerError(
                       Result.Failure("An error has occurred", 500)
                          .WithErrors(errors.ToList()));
                }

                if (payload?.Email != null)
                {
                    var userInfoLogin = new UserLoginInfo("google", payload.Subject, "google");
                    await _userManager.AddLoginAsync(newUser, userInfoLogin);
                    newUser.EmailConfirmed = true;
                    await _userManager.UpdateAsync(newUser);
                }

                var assignRoleResult = await _userManager.AddToRoleAsync(newUser, userType);
                if (!assignRoleResult.Succeeded)
                {
                    var errors = assignRoleResult.Errors.Select(e => new ErrorDetail(e.Code, e.Description));
                    return TypedResults.InternalServerError(
                       Result.Failure("An error has occurred", 500)
                          .WithErrors(errors.ToList()));
                }

                var addClaimsResult = await AddClaimsToDatabase(newUser);
                if (!addClaimsResult.Succeeded)
                {
                    var errors = addClaimsResult.Errors.Select(e => new ErrorDetail(e.Code, e.Description));
                    return TypedResults.InternalServerError(
                       Result.Failure("An error has occurred", 500)
                          .WithErrors(errors.ToList()));
                }

                (Guid requestId, string otpRequestToken) = await _otpService.GenerateRequestOtpAsync(phoneNumber, OtpRequestFor.VerifyPhoneNumber);

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync(); 

                var paramsOtpType2 = new { otpProviderType = "" };

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
                    Link = Url.Action(_sendOtpActionName, _controllerName, paramsOtpType2, Request.Scheme),
                    OtpRequestToken = otpRequestToken,
                    RequestId = requestId
                });
            } 
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.InnerException?.Message);
                return TypedResults.InternalServerError(Messages.InternalServerError);
            }
        }); 
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
                return TypedResults.InternalServerError(Messages.InternalServerError);
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
                await UserDeletion(user);
                return TypedResults.Unauthorized();
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
            return TypedResults.InternalServerError(Messages.InternalServerError);
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
                return TypedResults.InternalServerError(Messages.InternalServerError);
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
            return TypedResults.InternalServerError(Messages.InternalServerError);
        } 
    }



    [HttpPatch("verify/login")]
    public async Task<IResult> VerifyLogin(
        [FromBody] VerifyLoginRequest request,
        IValidator<VerifyLoginRequest> validator)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync<IResult>(async () =>
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
                            .WithError(new("Credentials", "OTP or phone number is invalid")));
                }

                var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber);

                if (user is null)
                {
                    return TypedResults.Unauthorized();
                }


                var claims = await _userManager.GetClaimsAsync(user);

                if (claims is null)
                {
                    await _userManager.DeleteAsync(user);
                    return TypedResults.Unauthorized();
                }

                var accessToken = _tokenService.GenerateAccessToken(new ClaimsIdentity(claims));

                var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id);

                await _otpService.RemoveOtpAsync(request.PhoneNumber);

                string redirectTo = user.IsAccountRegistered ? _homePageActionName : _createAccountActionName;

                await transaction.CommitAsync();

                return TypedResults.Accepted(redirectTo,
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
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.InnerException?.Message);
                return TypedResults.InternalServerError(Messages.InternalServerError);
            }
        });
    }




    [HttpPatch("verify/phone-number")]
    public async Task<IResult> VerifyPhoneNumber(
        [FromBody] VerifyPhoneNumberRequest request,
        [FromServices] IValidator<VerifyPhoneNumberRequest> validator)
    {
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

            if (user.PhoneNumberConfirmed)
            {
                return TypedResults.NoContent();
            }


            var changeResult = await _userManager.ChangePhoneNumberAsync(user, request.PhoneNumber, request.Token);
            if (!changeResult.Succeeded)
            {
                var errors = changeResult.Errors.Select(e => new ErrorDetail(e.Code, e.Description));
                return TypedResults.InternalServerError(
                   Result.Failure("An error has occured", 500)
                      .WithErrors(errors.ToList()));
            }

            return TypedResults.NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
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
                return TypedResults.InternalServerError(Messages.InternalServerError);
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
            return TypedResults.InternalServerError(Messages.InternalServerError);
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
            new (SantaiClaimTypes.UserType, user.UserType),
            new (ClaimTypes.Role, user.UserType)
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

    private async Task UserDeletion(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var userLoginInfos = await _userManager.GetLoginsAsync(user);

        await _userManager.DeleteAsync(user);
        await _userManager.RemovePasswordAsync(user); 
        await _userManager.RemoveFromRolesAsync(user, roles);

        foreach(var userLoginInfo in userLoginInfos)
        {
            await _userManager.RemoveLoginAsync(user, userLoginInfo.LoginProvider, userLoginInfo.ProviderKey);  
        }
    }

    [Authorize(Policy = "Administrator")]
    [HttpPost("register/business")]
    public IResult GetResource()
    {
        var roles = User.FindFirstValue(ClaimTypes.MobilePhone);
        return TypedResults.Ok(roles); 
    }
}
