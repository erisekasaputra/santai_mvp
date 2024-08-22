using Identity.API.Domain.Entities;
using Identity.Contracts.Enumerations;
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

        builder.Entity<ApplicationUser>(options =>
        {
            options.Property(u => u.UserType)
                .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<UserType>(v)
                );

            options.HasIndex(u => u.PhoneNumber)
                .IsUnique();
        });

        builder.AddInboxStateEntity();
        builder.AddOutboxMessageEntity();
        builder.AddOutboxStateEntity();
    }
}
