using Identity.API.Domain.Entities; 
using Identity.Contracts;
using MassTransit; 
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;  

namespace Identity.API.Infrastructure;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{ 

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    { 
        base.OnModelCreating(builder); 
         
        builder.Entity<ApplicationUser>()
            .Property(u => u.UserType)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<UserType>(v) 
            );

        builder.AddInboxStateEntity();
        builder.AddOutboxMessageEntity();
        builder.AddOutboxStateEntity();
    }
}
