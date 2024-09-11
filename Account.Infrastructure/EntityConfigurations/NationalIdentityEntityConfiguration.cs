using Account.Domain.Aggregates.NationalIdentityAggregate;
using Account.Domain.Enumerations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.EntityConfigurations;

public class NationalIdentityEntityConfiguration : IEntityTypeConfiguration<NationalIdentity>
{
    public void Configure(EntityTypeBuilder<NationalIdentity> e)
    {
        e.HasKey(p => p.Id);

        e.HasIndex(p => new { p.UserId, p.VerificationStatus })
            .IsUnique()
            .HasFilter("[VerificationStatus] =  'Accepted' ");

        e.Property(p => p.VerificationStatus)
            .HasConversion<string>();

        e.Property(p => p.HashedIdentityNumber)
            .HasMaxLength(255)
            .IsRequired()
            .HasConversion(
                v => v.Trim(),
                v => v.Trim());

        e.Property(p => p.EncryptedIdentityNumber)
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
            .WithMany(p => p.NationalIdentities)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        e.Ignore(p => p.DomainEvents);
        e.Ignore(p => p.EntityStateAction);
    }
}
