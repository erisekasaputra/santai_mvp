using Core.Enumerations;
using Identity.API.Domain.Entities; 
using MassTransit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

            options.Property(p => p.DeviceIds)
                 .HasConversion(
                     v => string.Join(',', v.Distinct()),
                     v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Distinct().ToHashSet(),
                     new ValueComparer<ICollection<string>>(
                         (c1, c2) => c1 == null && c2 == null || c1 != null && c2 != null && c1.OrderBy(x => x).SequenceEqual(c2.OrderBy(x => x)),
                         c => c == null ? 0 : c.OrderBy(x => x).Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                         c => c == null ? new HashSet<string>() : new HashSet<string>(c)
                     )
                 );
        });

        builder.AddInboxStateEntity();
        builder.AddOutboxMessageEntity();
        builder.AddOutboxStateEntity();
    }
}
