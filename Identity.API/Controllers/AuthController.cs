using Azure.Core;
using FluentValidation;
using Identity.API.Abstraction;
using Identity.API.Domain.Entities;
using Identity.API.Domain.Events;
using Identity.API.Dto;
using Identity.API.Extensions;
using Identity.API.Infrastructure;
using Identity.API.SeedWork; 
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.IdentityModel.Tokens; 
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace Identity.API.Controllers;


[Route("api/v1/[controller]")]
[ApiController]
public class AuthController
{
    const string apiVersionPhoneNumberVerification = "v1";
    const string apiVersionCreateAccount = "v1";

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
        IOtpService otpService)
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
    }

     

    [HttpPost("google-signin")]
    public async Task<IResult> GoogleSignIn([AsParameters] string userType, [FromBody] GoogleSignInRequest model)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            if (!UserTypes.AllowedTypes().Contains(userType))
            {
                return TypedResults.BadRequest(
                    Result.Failure("An error has occured", 500)
                        .WithError(new("UserType", $"Unknown user type ${userType}")));
            }


            var principal = await _googleTokenValidator.ValidateAsync(model.GoogleToken);
            if (principal is null)
                return TypedResults.BadRequest();



            var email = principal.FindFirstValue(ClaimTypes.Email);
            if (email is null)
                return TypedResults.BadRequest();


            // if user does not exist in the database, then give the CreateAccount instruction
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            { 
                return TypedResults.Accepted("CreateAccount",
                new
                {
                    GoogleToken = model.GoogleToken,
                    Link = $"api/{apiVersionCreateAccount}/auth/register?userType={userType}",
                    UserTypes = UserTypes.AllowedTypes()
                });
            }

            var phoneNumber = user.PhoneNumber!;

            if (!user.PhoneNumberConfirmed)
            {
                // Start Send otp for phone number verification  
                var otpToken = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);

                await _mediator.Publish(new OtpRequestedDomainEvent(phoneNumber, otpToken));
                await _dbContext.SaveChangesAsync(); 
                await transaction.CommitAsync();

                return TypedResults.Accepted("VerifyPhoneNumber", new
                {
                    User = new
                    {
                        Sub = user.Id,
                        Username = user.UserName,
                        PhoneNumber = phoneNumber,
                        Email = user.Email
                    },
                    Link = $"api/{apiVersionPhoneNumberVerification}/auth/verify/phone-number"
                });
                // Start Send otp for phone number verification  
            }


            var userClaims = await _userManager.GetClaimsAsync(user);

            if (!userClaims.Any())
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (roles is null || !roles.Any())
                {
                    _logger.LogError("User {0} has user data but does not have user claim and user roles", user.Id);
                    return TypedResults.InternalServerError(Messages.InternalServerError);
                }

                (var addClaimsResult, var newClaims) = await AddClaimsToDatabase(user, [..roles]);
                if (!addClaimsResult.Succeeded)
                {
                    var errors = addClaimsResult.Errors.Select(e => new ErrorDetail(e.Code, e.Description));
                     
                    return TypedResults.InternalServerError(
                       Result.Failure("An error has occured", 500)
                          .WithErrors(errors.ToList()));
                }
            }


            // otp service will returning value if there is no holding otp request for a given phone number and 
            // otherwise it will return null
            (var otp, var remainingTime) = await _otpService.GenerateOtpAsync(phoneNumber);
            if (otp is not null)
            {
                await _mediator.Publish(new OtpRequestedDomainEvent(phoneNumber, otp));
            }

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return TypedResults.Accepted($"VerifyLogin", 
            new
            {
                User = new
                {
                    Sub = user.Id,
                    Username = user.UserName,
                    PhoneNumber = phoneNumber,
                    Email = user.Email
                },
                Link = $"api/{apiVersionPhoneNumberVerification}/auth/verify/login",
                RemainingLockingOtp = remainingTime
            }); 
        }
        catch (Exception ex)
        {  
            _logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    //[HttpPost("otp/send")]
    //public async Task<IResult> SendOtp()

    [HttpPatch("verify/login")]
    public async Task<IResult> VerifyLogin([FromBody] VerifyLoginRequest request, IValidator<VerifyLoginRequest> validator)
    {
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
                        .WithError(new("Credentials", "Otp or phone number is invalid")));
            }

            var user = await _userManager.FindByNameAsync(request.PhoneNumber);

            if (user is null)
            {
                _logger.LogError("User data is missing with unexpected behavior at {0}", DateTime.UtcNow);
                return TypedResults.InternalServerError(Messages.InternalServerError);
            }

            var claims = await _userManager.GetClaimsAsync(user);


            if (claims is null)
            {
                _logger.LogError("Claim for user data {0} is missing with unexpected behavior at {1}", user.Id, DateTime.UtcNow);
                return TypedResults.InternalServerError(Messages.InternalServerError);
            }

            var accessToken = _tokenService.GenerateAccessToken(new ClaimsIdentity(claims));

            var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id);

            await _otpService.RemoveOtpAsync(request.PhoneNumber);

            return TypedResults.Accepted("Dashboard",
            new 
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = new
                {
                    Sub = user.Id,
                    Username = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }


    //[HttpPost("login/staff")]
    //public async Task<IResult> InternalLoginStaff(
    //    [FromBody] LoginStaffRequest model,
    //    [FromServices] IValidator<LoginStaffRequest> validator)
    //{

    //    var transaction = await _dbContext.Database.BeginTransactionAsync();

    //    try
    //    {
    //        var validation = await validator.ValidateAsync(model);

    //        if (!validation.IsValid)
    //        {
    //            await RollbackTransactionAsync(transaction);
    //            return TypedResults.BadRequest(validation.Errors);
    //        }

    //        var phoneNumber = model.PhoneNumber.NormalizePhoneNumber(model.RegionCode);

    //        if (phoneNumber == null)
    //        {
    //            await RollbackTransactionAsync(transaction);
    //            return TypedResults.BadRequest("Please provide valid phone number");
    //        }

    //        var user = await _userManager.FindByNameAsync(phoneNumber);

    //        if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
    //        {
    //            await RollbackTransactionAsync(transaction);
    //            return TypedResults.Unauthorized();
    //        }

    //        var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, false, lockoutOnFailure: false);

    //        if (!signInResult.Succeeded)
    //        {
    //            await RollbackTransactionAsync(transaction);
    //            return TypedResults.Unauthorized();
    //        }



    //        var claims = await _userManager.GetClaimsAsync(user);
    //        if (claims is null)
    //        {
    //            (var addClaimsResult, var newClaims) = await AddClaimsToDatabase(user);
    //            if (!addClaimsResult.Succeeded)
    //            {
    //                var errors = addClaimsResult.Errors.Select(e => new ErrorDetail(e.Code, e.Description));

    //                await RollbackTransactionAsync(transaction);
    //                return TypedResults.InternalServerError(
    //                   Result.Failure("An error has occured", 500)
    //                      .WithErrors(errors.ToList()));
    //            }
    //        }




    //        var token = _tokenService.GenerateAccessToken(new ClaimsIdentity(claims));

    //        var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id);

    //        await _dbContext.SaveChangesAsync(); 
    //        await transaction.CommitAsync(); 

    //        return TypedResults.Ok(new
    //        {
    //            Token = token,
    //            RefreshToken = refreshToken,
    //            User = new
    //            {
    //                Sub = user.Id,
    //                Username = user.UserName,
    //                PhoneNumber = user.PhoneNumber,
    //                Email = user.Email
    //            }
    //        });
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, ex.InnerException?.Message);
    //        return TypedResults.InternalServerError(Messages.InternalServerError);
    //    }
    //}




    [HttpPost("login/user")]
    public async Task<IResult> InternalLoginUser(
        [FromBody] LoginUserRequest model,
        [FromServices] IValidator<LoginUserRequest> validator)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var validation = await validator.ValidateAsync(model);

            if (!validation.IsValid)
            { 
                return TypedResults.BadRequest(validation.Errors);
            }


            var phoneNumber = model.PhoneNumber.NormalizePhoneNumber(model.RegionCode);

            if (phoneNumber is null)
            { 
                return TypedResults.BadRequest(
                    Result.Failure("An error has occured", 400)
                        .WithError(new("PhoneNumber", "Phone number format is invalid")));
            }

            var user = await _userManager.FindByNameAsync(phoneNumber);

            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            { 
                return TypedResults.BadRequest(
                    Result.Failure("An error has occured", 400)
                        .WithError(new("Credentials", "We couldn't find an account with the given details")));
            }


            if (!user.PhoneNumberConfirmed)
            {
                var otpToken = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber!);
               
                await _mediator.Publish(new OtpRequestedDomainEvent(user.PhoneNumber!, otpToken));
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return TypedResults.Accepted($"VerifyPhoneNumber", new
                {
                    User = new
                    {
                        Sub = user.Id,
                        Username = user.UserName,
                        PhoneNumber = user.PhoneNumber,
                        Email = user.Email
                    },
                    Link = $"api/{apiVersionPhoneNumberVerification}/auth/verify/phone-number"
                });
            }


            var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, false, lockoutOnFailure: false);

            if (!signInResult.Succeeded)
            {    
                return TypedResults.Unauthorized();
            }




            var claims = await _userManager.GetClaimsAsync(user);
            if (claims is null)
            {

                var roles = await _userManager.GetRolesAsync(user);

                if (roles is null || !roles.Any())
                {
                    _logger.LogError("User {0} has user data but does not have user claim and user roles", user.Id);
                    return TypedResults.InternalServerError(Messages.InternalServerError);
                }






                (var addClaimsResult, var newClaims) = await AddClaimsToDatabase(user, [..roles]);
                if (!addClaimsResult.Succeeded)
                {
                    var errors = addClaimsResult.Errors.Select(e => new ErrorDetail(e.Code, e.Description));
                     
                    return TypedResults.InternalServerError(
                       Result.Failure("An error has occured", 500)
                          .WithErrors(errors.ToList()));
                }
            }



            // otp service will returning value if there is no holding otp request for a given phone number and 
            // otherwise it will return null
            (var otp, var remainingTime) = await _otpService.GenerateOtpAsync(phoneNumber);
            if (otp is not null)
            {
                await _mediator.Publish(new OtpRequestedDomainEvent(phoneNumber, otp));
            }

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return TypedResults.Accepted($"VerifyLogin", new
            {
                User = new
                {
                    Sub = user.Id,
                    Username = user.UserName,
                    PhoneNumber = phoneNumber,
                    Email = user.Email
                },
                Link = $"api/{apiVersionPhoneNumberVerification}/auth/verify/login",
                RemainingLockingOtp = remainingTime
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
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



            var changeResult = await _userManager.ChangePhoneNumberAsync(user, request.PhoneNumber, request.Token); 
            if (!changeResult.Succeeded)
            {
                var errors = changeResult.Errors.Select(e => new ErrorDetail(e.Code, e.Description)); 
                return TypedResults.InternalServerError(
                   Result.Failure("An error has occured", 500)
                      .WithErrors(errors.ToList()));
            }
             

            return TypedResults.Ok();
        }
        catch (Exception ex) 
        {
            _logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    } 


    [HttpPost("register")]
    public async Task <IResult> RegisterNewRegularUser(
        [AsParameters] string userType, 
        [FromBody] RegisterUserRequest model)
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

            if (!UserTypes.AllTypes().TryGetValue(userType, out string? userTypeResult))
            {
                _logger.LogError("User types does not exists in the list of user roles");
                await RollbackTransactionAsync(transaction);
                return TypedResults.InternalServerError(Messages.InternalServerError);
            }

            var phoneNumber = model.PhoneNumber.NormalizePhoneNumber(model.RegionCode);

            if (phoneNumber is null)
            { 
                return TypedResults.BadRequest(
                   Result.Failure("An error has occured", 400)
                       .WithError(new("PhoneNumber", "Please provide valid phone number")));
            }






            string? emailClaim = await GetEmailClaimFromGoogleToken(model.GoogleIdToken);
             
         
            if (emailClaim is not null)
            { 
                // check from identity service by email , is email exists if exists then retrieve user
                var userByEmailClaim = await _userManager.FindByEmailAsync(emailClaim); 

                if (userByEmailClaim is not null)
                {  
                    return await HandleExistingUser(transaction, userByEmailClaim, userTypeResult);
                } 
            }







            var user = await _userManager.FindByNameAsync(phoneNumber);

            if (user is not null)
            { 
                return TypedResults.Conflict(
                    Result.Failure("An error has occured", 409)
                       .WithError(new("PhoneNumber", "Phone number is already in used"))); 
            }  
             
             
            var newUser = new ApplicationUser
            {
                PhoneNumber = phoneNumber,
                UserName = phoneNumber,
                Email = emailClaim
            };

            var addUserResult = await _userManager.CreateAsync(newUser, model.Password);

            if (!addUserResult.Succeeded) 
            { 
                var errors = addUserResult.Errors.Select(e => new ErrorDetail(e.Code, e.Description));
                 
                return TypedResults.InternalServerError(
                   Result.Failure("An error has occured", 500)
                      .WithErrors(errors.ToList()));
            } 




            var assignRoleResult = await _userManager.AddToRoleAsync(newUser, userTypeResult); 
            if (!assignRoleResult.Succeeded)
            { 
                var errors = assignRoleResult.Errors.Select(e => new ErrorDetail(e.Code, e.Description));
                  
                return TypedResults.InternalServerError(
                   Result.Failure("An error has occured", 500)
                      .WithErrors(errors.ToList()));
            }





            (var addClaimsResult, var claims) = await AddClaimsToDatabase(newUser, userTypeResult);
            if(!addClaimsResult.Succeeded)
            {
                var errors = addClaimsResult.Errors.Select(e => new ErrorDetail(e.Code, e.Description));
                 
                return TypedResults.InternalServerError(
                   Result.Failure("An error has occured", 500)
                      .WithErrors(errors.ToList()));
            }


             

            // Generate change phone number token async
            var newToken = await _userManager.GenerateChangePhoneNumberTokenAsync(newUser, newUser.PhoneNumber); 
            // Send otp domain event
            await _mediator.Publish(new OtpRequestedDomainEvent(newUser.PhoneNumber, newToken)); 
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
 
            return TypedResults.Accepted($"VerifyPhoneNumber", new
            {
                User = new
                {
                    Sub = newUser.Id,
                    Username = newUser.UserName,
                    PhoneNumber = newUser.PhoneNumber,
                    Email = newUser.Email
                },
                Link = $"api/{apiVersionPhoneNumberVerification}/auth/verify/phone-number"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.InnerException?.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

     


    [HttpPost("refresh-token")] 
    public async Task<IResult> RefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest)
    {
        try 
        {
            var newRefreshToken = await _tokenService.RotateRefreshTokenAsync(refreshTokenRequest.RefreshToken); 
            if (newRefreshToken is null)
            {
                return TypedResults.Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(newRefreshToken.UserId); 
            if (user is null)
            {
                return TypedResults.Unauthorized();
            }


            var claims = await _userManager.GetClaimsAsync(user);  
            if (claims is null || !claims.Any())
            {
                return TypedResults.Unauthorized();
            }

            var claimsIdentity = new ClaimsIdentity(claims); 
            var accessToken = _tokenService.GenerateAccessToken(claimsIdentity); 
            return TypedResults.Ok(new
            {
                Token = accessToken,
                RefreshToken = newRefreshToken.Token,
                User = new
                {
                    Sub = user.Id,
                    Username = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email
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














    // Helpers

    private async Task<string?> GetEmailClaimFromGoogleToken(string? googleIdToken)
    {
        if (string.IsNullOrWhiteSpace(googleIdToken))
        {
            return null;
        }

        var principal = await _googleTokenValidator.ValidateAsync(googleIdToken);

        if (principal is null)
        {
            return null;
        }

        return principal.FindFirstValue(ClaimTypes.Email);
    }

    private async Task<IResult> HandleExistingUser(
        IDbContextTransaction transaction,
        ApplicationUser userByEmailClaim, 
        string role)
    { 
        var userClaims = await _userManager.GetClaimsAsync(userByEmailClaim);

        if (userClaims is null)
        {
            (var addClaimsResult, var claims) = await AddClaimsToDatabase(userByEmailClaim, role);
            if (!addClaimsResult.Succeeded)
            {
                var errors = addClaimsResult.Errors.Select(e => new ErrorDetail(e.Code, e.Description));
                 
                return TypedResults.InternalServerError(
                   Result.Failure("An error has occured", 500)
                      .WithErrors(errors.ToList()));
            }
        }

        if (!userByEmailClaim.PhoneNumberConfirmed)
        {
            var token = await _userManager.GenerateChangePhoneNumberTokenAsync(userByEmailClaim, userByEmailClaim.PhoneNumber!);
            await _mediator.Publish(new OtpRequestedDomainEvent(userByEmailClaim.PhoneNumber!, token));

            await _dbContext.SaveChangesAsync(); 
            await transaction.CommitAsync();

            return TypedResults.Accepted("VerifyPhoneNumber",
            new
            {
                Sub = userByEmailClaim.Id,
                Username = userByEmailClaim.PhoneNumber,
                PhoneNumber = userByEmailClaim.PhoneNumber,
                Email = userByEmailClaim.Email,
                Link = $"api/{apiVersionPhoneNumberVerification}/auth/verify/phone-number"
            });
        }

        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        return TypedResults.Ok(new
        {
            User = new
            {
                Sub = userByEmailClaim.Id,
                Username = userByEmailClaim.UserName,
                PhoneNumber = userByEmailClaim.PhoneNumber,
                Email = userByEmailClaim.Email
            } 
        });
    }

    private async Task<(IdentityResult, List<Claim>)> AddClaimsToDatabase(ApplicationUser user, params string[] roles)
    {
        var claims = new List<Claim>()
        {
            new (JwtRegisteredClaimNames.Sub, user.Id),
            new (ClaimTypes.Name, user.PhoneNumber!),
            new (ClaimTypes.MobilePhone, user.PhoneNumber!)
        };

        if (user.Email is not null)
        {
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
        }

        if (roles.Length > 0) 
        {
            foreach(var role in roles)
            { 
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }

        return (await _userManager.AddClaimsAsync(user, claims), claims);
    }

    private async Task RollbackTransactionAsync(IDbContextTransaction transaction)
    {
        if (transaction is null) 
        {
            return;
        }

        await transaction.RollbackAsync();
    }
}
