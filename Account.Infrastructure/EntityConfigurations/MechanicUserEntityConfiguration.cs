using Account.Domain.Aggregates.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.EntityConfigurations;

public class MechanicUserEntityConfiguration : IEntityTypeConfiguration<MechanicUser>
{
    public void Configure(EntityTypeBuilder<MechanicUser> e)
    {
        e.Property(p => p.Rating)
                .HasColumnType("decimal(5, 2)");

        e.Property(p => p.DeviceId)
            .HasMaxLength(255)
            .IsRequired(false)
            .HasConversion(
                v => v == null ? null : v.Trim(),
                v => v == null ? null : v.Trim());

        e.HasMany(p => p.DrivingLicenses)
            .WithOne(p => p.MechanicUser)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        e.HasMany(p => p.NationalIdentities)
            .WithOne(p => p.MechanicUser)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        e.HasMany(p => p.Certifications)
            .WithOne(p => p.MechanicUser)
            .HasForeignKey(p => p.MechanicUserId)
            .OnDelete(DeleteBehavior.Cascade); 
    }
}
