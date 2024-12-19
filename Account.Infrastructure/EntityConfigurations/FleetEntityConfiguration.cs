using Account.Domain.Aggregates.FleetAggregate;
using Core.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.EntityConfigurations;

public class FleetEntityConfiguration : IEntityTypeConfiguration<Fleet>
{
    public void Configure(EntityTypeBuilder<Fleet> e)
    { 
        e.HasKey(p => p.Id);

        e.HasIndex(p => p.HashedRegistrationNumber)
            .IsUnique();

        e.HasIndex(p => p.HashedEngineNumber)
            .IsUnique();

        e.HasIndex(p => p.HashedChassisNumber) 
            .IsUnique(); 
         
        e.Property(v => v.HashedRegistrationNumber)
            .HasMaxLength(255)
            .IsRequired(false);

        e.Property(v => v.EncryptedRegistrationNumber)
            .HasMaxLength(255)
            .IsRequired(false);

        e.Property(v => v.Brand)
            .HasMaxLength(100)
            .IsRequired();

        e.Property(v => v.Model)
            .HasMaxLength(100)
            .IsRequired();

        e.Property(v => v.ImageUrl)
           .HasMaxLength(100)
           .IsRequired();

        e.Property(v => v.HashedChassisNumber)
            .HasMaxLength(255)
            .IsRequired(false);

        e.Property(v => v.EncryptedChassisNumber)
            .HasMaxLength(255)
            .IsRequired(false);

        e.Property(v => v.HashedEngineNumber)
            .HasMaxLength(255)
            .IsRequired(false);

        e.Property(v => v.EncryptedEngineNumber)
            .HasMaxLength(255)
            .IsRequired(false);

        e.Property(v => v.HashedInsuranceNumber)
            .HasMaxLength(255)
            .IsRequired(false);

        e.Property(v => v.EncryptedInsuranceNumber)
            .HasMaxLength(255)
            .IsRequired(false);

        e.Property(v => v.VehicleType)
            .HasConversion<string>()
            .IsRequired()
            .IsRequired(false);

        e.Property(v => v.FuelType)
            .HasConversion<string>()
            .IsRequired()
            .IsRequired(false);

        e.Property(v => v.UsageStatus)
            .HasConversion<string>()
            .IsRequired()
            .IsRequired(false);

        e.Property(v => v.OwnershipStatus)
            .HasConversion<string>()
            .IsRequired()
            .IsRequired(false);

        e.Property(v => v.TransmissionType)
            .HasConversion<string>()
            .IsRequired()
            .IsRequired(false);

        e.Property(v => v.YearOfManufacture)
            .IsRequired()
            .IsRequired(false);

        e.Property(v => v.LastInspectionDateUtc)
            .HasColumnType("datetime2")
            .IsRequired(false)
            .HasConversion(DateTimeExtension.UtcConverter());

        e.Property(v => v.OdometerReading)
            .IsRequired()
            .IsRequired(false);

        e.Property(v => v.RegistrationDateUtc)
            .HasColumnType("datetime2")
            .IsRequired(false)
            .HasConversion(DateTimeExtension.UtcConverter());
         
        e.OwnsOne(p => p.Owner, owner =>
        {
            owner.Property(p => p.EncryptedOwnerName)
                .IsRequired(false)
                .HasMaxLength(50);

            owner.Property(p => p.EncryptedOwnerAddress)
               .IsRequired(false)
               .HasMaxLength(255);
        });  

        e.Ignore(p => p.DomainEvents);
        e.Ignore(p => p.EntityStateAction); 
    }
}  