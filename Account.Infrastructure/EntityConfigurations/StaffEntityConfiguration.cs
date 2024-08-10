using Account.Domain.Aggregates.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.EntityConfigurations;

public class StaffEntityConfiguration : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> e)
    {
        e.HasKey(p => p.Id);

        e.HasIndex(p => p.Username)
            .IsUnique();

        e.HasIndex(p => p.PhoneNumber)
            .IsUnique();

        e.HasIndex(p => p.Email)
            .IsUnique(); 

        e.Property(p => p.Username)
            .HasMaxLength(20)
            .IsRequired()
            .HasConversion(
                v => v,
                v => v);

        e.Property(p => p.BusinessUserId)
            .HasMaxLength(50)
            .IsRequired();

        e.Property(p => p.BusinessUserCode)
            .HasMaxLength(6)
            .IsRequired()
            .HasConversion(
                v => v.Trim(),
                v => v.Trim());

        e.Property(p => p.PhoneNumber)
            .HasMaxLength(20)
            .IsRequired()
            .HasConversion(
                v => v.Trim(),
                v => v.Trim());

        e.Property(p => p.NewPhoneNumber)
            .HasMaxLength(20)
            .IsRequired(false)
            .HasConversion(
                v => v == null ? null : v.Trim(),
                v => v == null ? null : v.Trim());

        e.Property(p => p.IsPhoneNumberVerified)
            .IsRequired();

        e.Property(p => p.Email)
            .HasMaxLength(254)
            .IsRequired()
            .HasConversion(
                v => v,
                v => v);

        e.Property(p => p.NewEmail)
            .HasMaxLength(254)
            .IsRequired(false)
            .HasConversion(
                v => v == null ? null : v,
                v => v == null ? null : v);

        e.Property(p => p.IsEmailVerified)
            .IsRequired();

        e.Property(p => p.Name)
            .HasMaxLength(50)
            .IsRequired()
            .HasConversion(
                v => v.Trim(),
                v => v.Trim());

        e.Property(p => p.DeviceId)
            .HasMaxLength(255)
            .HasConversion(
                v => v == null ? null : v.Trim(),
                v => v == null ? null : v.Trim());

        e.Property(p => p.TimeZoneId)
            .HasMaxLength(40)
            .IsRequired()
            .HasConversion(
                v => v.Trim(),
                v => v.Trim());

        e.HasOne(s => s.BusinessUser)
            .WithMany(b => b.Staffs)
            .HasForeignKey(s => s.BusinessUserId)
            .OnDelete(DeleteBehavior.Cascade);

        e.OwnsOne(p => p.Address, address =>
        {
            address.Property(ap => ap.AddressLine1)
                .HasMaxLength(255)
                .IsRequired(true)
                .HasConversion(
                    v => v.Trim(),
                    v => v.Trim());

            address.Property(ap => ap.AddressLine2)
                .HasMaxLength(255)
                .IsRequired(false)
                .HasConversion(
                    v => v == null ? null : v.Trim(),
                    v => v == null ? null : v.Trim());

            address.Property(ap => ap.AddressLine3)
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

        e.Ignore(p => p.DomainEvents);
    }
}
