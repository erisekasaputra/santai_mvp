using Account.Domain.Aggregates.DrivingLicenseAggregate; 
using Account.Domain.Enumerations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.EntityConfigurations;

public class DrivingLicenseEntityConfiguration : IEntityTypeConfiguration<DrivingLicense>
{
    public void Configure(EntityTypeBuilder<DrivingLicense> e)
    {
        e.HasKey(p => p.Id);

        e.HasIndex(p => new { p.UserId, p.VerificationStatus })
            .IsUnique()
            .HasFilter("[VerificationStatus] = 'Accepted' ");

        e.Property(p => p.VerificationStatus)
            .HasConversion<string>();

        e.Property(p => p.HashedLicenseNumber)
            .HasMaxLength(255)
            .IsRequired()
            .HasConversion(
                v => v.Trim(),
                v => v.Trim());

        e.Property(p => p.EncryptedLicenseNumber)
            .HasMaxLength(255)
            .IsRequired()
            .HasConversion(
                v => v.Trim(),
                v => v.Trim());

        e.Property(p => p.FrontSideImageUrl)
            .HasMaxLength(255)
            .IsRequired()
            .HasConversion(
                v => v.Trim(),
                v => v.Trim());

        e.Property(p => p.BackSideImageUrl)
            .HasMaxLength(255)
            .IsRequired()
            .HasConversion(
                v => v.Trim(),
                v => v.Trim());

        e.HasQueryFilter(p => p.VerificationStatus != VerificationState.Rejected);

        e.HasOne(p => p.MechanicUser)
            .WithMany(p => p.DrivingLicenses)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        e.Ignore(p => p.DomainEvents);
    }
}
