using Core.Enumerations;
using Identity.API.Domain.Entities; 
using MassTransit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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

            options.Property(u => u.DeviceIds)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v), 
                    v => v == null ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(v) ?? new List<string>());
        });

        builder.AddInboxStateEntity();
        builder.AddOutboxMessageEntity();
        builder.AddOutboxStateEntity();
    }
}
