using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Notification.Worker.Domain;

namespace Notification.Worker.Infrastructure;

public class NotificationDbContext : DbContext
{
    public DbSet<UserProfile> Users { get; set; }

    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserProfile>(configure =>
        {
            configure.HasKey(p => p.Id);
             
            configure.Property(p => p.PhoneNumber)
                .IsRequired()
                .HasMaxLength(256);
             
            configure.Property(p => p.Email)
                .IsRequired(false)
                .HasMaxLength(256);

            configure.Property(p => p.Profiles)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x), 
                    v => v == null ? new List<IdentityProfile>() : JsonConvert.DeserializeObject<List<IdentityProfile>>(v) ?? new List<IdentityProfile>());
        });

        base.OnModelCreating(modelBuilder);
    }
}
