using Account.Domain.Aggregates.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.EntityConfigurations;

public class BusinessUserEntityConfiguration : IEntityTypeConfiguration<BusinessUser>
{
    public void Configure(EntityTypeBuilder<BusinessUser> e)
    {
        e.HasIndex(p => p.Code).IsUnique();

        e.Property(p => p.Code)
            .HasMaxLength(6)
            .IsRequired()
            .HasConversion(
                v => v.Trim(),
                v => v.Trim());

        e.Property(p => p.BusinessName)
            .HasMaxLength(50)
            .IsRequired()
            .HasConversion(
                v => v.Trim(),
                v => v.Trim());

        e.Property(p => p.EncryptedContactPerson)
            .HasMaxLength(255)
            .IsRequired()
            .HasConversion(
                v => v.Trim(),
                v => v.Trim()); 

        e.Property(p => p.EncryptedTaxId)
            .HasMaxLength(255)
            .IsRequired(false)
            .HasConversion(
                v => v == null ? null : v.Trim(),
                v => v == null ? null : v.Trim());

        e.Property(p => p.WebsiteUrl)
            .HasMaxLength(255)
            .IsRequired(false)
            .HasConversion(
                v => v == null ? null : v.Trim(),
                v => v == null ? null : v.Trim());

        e.Property(p => p.Description) 
            .IsRequired(false)
            .HasConversion(
                v => v == null ? null : v.Trim(),
                v => v == null ? null : v.Trim())
            .HasColumnType("nvarchar(1000)");

        e.HasMany(p => p.BusinessLicenses)
            .WithOne(b => b.BusinessUser)
            .HasForeignKey(f => f.BusinessUserId)
            .OnDelete(DeleteBehavior.Cascade);

        e.HasMany(p => p.Staffs)
            .WithOne(s => s.BusinessUser)
            .HasForeignKey(s => s.BusinessUserId) 
            .OnDelete(DeleteBehavior.Cascade); 
    }
}
