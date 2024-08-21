using Identity.Contracts;
using Microsoft.AspNetCore.Identity;

namespace Identity.API.Domain.Entities;

public class ApplicationUser : IdentityUser
{  
    public bool IsAccountRegistered { get; set; }   
    public string? BusinessCode { get; init; } 
    public required UserType UserType { get; init; }
}
