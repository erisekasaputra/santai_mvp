using Account.Domain.Aggregates.FleetAggregate;
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
            .HasMaxLength(255);

        e.Property(v => v.EncryptedRegistrationNumber)
            .HasMaxLength(255);

        e.Property(v => v.Make)
            .HasMaxLength(100)
            .IsRequired();

        e.Property(v => v.Model)
            .HasMaxLength(100)
            .IsRequired();

        e.Property(v => v.HashedChassisNumber)
            .HasMaxLength(255);

        e.Property(v => v.EncryptedChassisNumber)
            .HasMaxLength(255);

        e.Property(v => v.HashedEngineNumber)
            .HasMaxLength(255);

        e.Property(v => v.EncryptedEngineNumber)
            .HasMaxLength(255);

        e.Property(v => v.HashedInsuranceNumber)
            .HasMaxLength(255);

        e.Property(v => v.EncryptedInsuranceNumber)
            .HasMaxLength(255);

        e.Property(v => v.Color)
            .HasMaxLength(100)
            .IsRequired();
         
        e.Property(v => v.VehicleType)
            .HasConversion<string>()
            .IsRequired();

        e.Property(v => v.FuelType)
            .HasConversion<string>()
            .IsRequired();

        e.Property(v => v.UsageStatus)
            .HasConversion<string>()
            .IsRequired();

        e.Property(v => v.OwnershipStatus)
            .HasConversion<string>()
            .IsRequired();

        e.Property(v => v.TransmissionType)
            .HasConversion<string>()
            .IsRequired();
         
        e.Property(v => v.YearOfManufacture)
            .IsRequired();

        e.Property(v => v.LastInspectionDateUtc)
            .HasColumnType("datetime2")
            .IsRequired();

        e.Property(v => v.OdometerReading)
            .IsRequired();

        e.Property(v => v.RegistrationDateUtc)
            .HasColumnType("datetime2")
            .IsRequired();

        e.OwnsOne(p => p.Owner, owner =>
        {
            owner.Property(p => p.EncryptedOwnerName)
                .IsRequired()
                .HasMaxLength(50);

            owner.Property(p => p.EncryptedOwnerAddress)
               .IsRequired()
               .HasMaxLength(255);
        });  

        e.Ignore(p => p.DomainEvents); 
         
    }
}  