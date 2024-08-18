using Microsoft.AspNetCore.Identity;

namespace Identity.API.Domain.Entities;

public class ApplicationUser : IdentityUser
{  
    public string? BusinessCode { get; set; }
}
