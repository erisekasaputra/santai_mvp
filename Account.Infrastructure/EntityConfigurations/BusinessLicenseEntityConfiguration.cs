using Account.Domain.Aggregates.BusinessLicenseAggregate;
using Account.Domain.Enumerations;
using Core.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.EntityConfigurations;

public class BusinessLicenseEntityConfiguration : IEntityTypeConfiguration<BusinessLicense>
{
    public void Configure(EntityTypeBuilder<BusinessLicense> e)
    {
        e.HasKey(p => p.Id);

        e.HasIndex(p => new { p.HashedLicenseNumber, p.VerificationStatus })
            .IsUnique()
            .HasFilter("[VerificationStatus] = 'Accepted'");

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


        e.Property(p => p.Name)
            .HasMaxLength(100)
            .IsRequired()
            .HasConversion(
                v => v.Trim(),
                v => v.Trim());

        e.Property(p => p.Description)
           .IsRequired()
           .HasConversion(
                v => v.Trim(),
                v => v.Trim())
           .HasColumnType("nvarchar(1000)");

        e.Property(p => p.VerificationStatus)
            .HasConversion<string>();

        e.HasOne(p => p.BusinessUser)
            .WithMany(p => p.BusinessLicenses)
            .HasForeignKey(p => p.BusinessUserId)
            .OnDelete(DeleteBehavior.Cascade);   

        e.HasQueryFilter(p => p.VerificationStatus != VerificationState.Rejected);

        e.Ignore(p => p.DomainEvents);
        e.Ignore(p => p.EntityStateAction);
    }
}
