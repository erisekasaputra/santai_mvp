using Account.Domain.Aggregates.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Account.Infrastructure.EntityConfigurations;

public class StaffEntityConfiguration : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> e)
    {
        e.HasKey(p => p.Id); 

        e.HasIndex(p => p.HashedPhoneNumber)
            .IsUnique();

        e.HasIndex(p => p.HashedEmail)
            .IsUnique();  

        e.Property(p => p.BusinessUserId)
            .HasMaxLength(50)
            .IsRequired();

        e.Property(p => p.BusinessUserCode)
            .HasMaxLength(6)
            .IsRequired()
            .HasConversion(
                v => v.Trim(),
                v => v.Trim());

        e.Property(p => p.HashedPhoneNumber)
            .HasMaxLength(255)
            .IsRequired(false);

        e.Property(p => p.EncryptedPhoneNumber)
            .HasMaxLength(255)
            .IsRequired(false);

        e.Property(p => p.NewHashedPhoneNumber)
            .HasMaxLength(255)
            .IsRequired(false);

        e.Property(p => p.NewEncryptedPhoneNumber)
            .HasMaxLength(255)
            .IsRequired(false);

        e.Property(p => p.IsPhoneNumberVerified)
            .IsRequired();

        e.Property(p => p.HashedEmail)
            .HasMaxLength(255)
            .IsRequired(false);

        e.Property(p => p.EncryptedEmail)
             .HasMaxLength(255)
             .IsRequired(false);

        e.Property(p => p.NewHashedEmail)
            .HasMaxLength(255)
            .IsRequired(false);


        e.Property(p => p.NewEncryptedEmail)
            .HasMaxLength(255)
            .IsRequired(false);

        e.Property(p => p.IsEmailVerified)
            .IsRequired();

        e.Property(p => p.Name)
            .HasMaxLength(50)
            .IsRequired()
            .HasConversion(
                v => v.Trim(),
                v => v.Trim()); 

        e.Property(p => p.TimeZoneId)
            .HasMaxLength(40)
            .IsRequired()
            .HasConversion(
                v => v.Trim(),
                v => v.Trim());  

        e.OwnsOne(p => p.Address, address =>
        {
            address.Property(ap => ap.EncryptedAddressLine1)
                .HasMaxLength(255)
                .IsRequired(true)
                .HasConversion(
                    v => v.Trim(),
                    v => v.Trim());

            address.Property(ap => ap.EncryptedAddressLine2)
                .HasMaxLength(255)
                .IsRequired(false)
                .HasConversion(
                    v => v == null ? null : v.Trim(),
                    v => v == null ? null : v.Trim());

            address.Property(ap => ap.EncryptedAddressLine3)
                .HasMaxLength(255)
                .IsRequired(false)
                .HasConversion(
                    v => v == null ? null : v.Trim(),
                    v => v == null ? null : v.Trim());

            address.Property(ap => ap.City)
                .HasMaxLength(50)
                .IsRequired()
                .HasConversion(
                    v => v.Trim(),
                    v => v.Trim());

            address.Property(ap => ap.State)
                .HasMaxLength(50)
                .IsRequired()
                .HasConversion(
                    v => v.Trim(),
                    v => v.Trim());

            address.Property(ap => ap.PostalCode)
                .HasMaxLength(20)
                .IsRequired()
                .HasConversion(
                    v => v.Trim(),
                    v => v.Trim());

            address.Property(ap => ap.Country)
                .HasMaxLength(50)
                .HasConversion(
                    v => v.Trim(),
                    v => v.Trim());
        });
         
        e.HasMany(s => s.Fleets)
            .WithOne()
            .HasForeignKey(f => f.StaffId)
            .OnDelete(DeleteBehavior.NoAction);

        e.Ignore(p => p.Password); 
        e.Ignore(p => p.DomainEvents);
        e.Ignore(p => p.EntityStateAction);
    }
}
