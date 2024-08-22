using Identity.API.Enumerations;
using Identity.Contracts.Enumerations;

namespace Identity.API.Dto;

public class RegisterUserRequest
{
    public required string PhoneNumber { get; set; }
    public required string Password { get; set; }
    public required string RegionCode { get; set; }
    public required UserType UserType { get; set; }
    public string? GoogleIdToken { get; set; }
    public string? ReturnUrl { get; set; }
}
