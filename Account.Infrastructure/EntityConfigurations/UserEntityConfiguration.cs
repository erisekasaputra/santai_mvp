 
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Aggregates.LoyaltyAggregate;
using Account.Domain.Aggregates.ReferralAggregate;
using Newtonsoft.Json;
using Core.Extensions;

namespace Account.Infrastructure.EntityConfigurations;

public class UserEntityConfiguration : IEntityTypeConfiguration<BaseUser>
{
    public void Configure(EntityTypeBuilder<BaseUser> e)
    {
        e.HasKey(e => e.Id);
          
        e.HasIndex(p => p.HashedEmail).IsUnique();

        e.HasIndex(p => p.HashedPhoneNumber).IsUnique(); 
          
        e.Property(p => p.HashedEmail).HasMaxLength(255).IsRequired(false);
        
        e.Property(p => p.EncryptedEmail).HasMaxLength(255).IsRequired(false);

        e.Property(p => p.NewHashedEmail).HasMaxLength(255).IsRequired(false);
        
        e.Property(p => p.NewEncryptedEmail).HasMaxLength(255).IsRequired(false);

        e.Property(p => p.HashedPhoneNumber).HasMaxLength(255).IsRequired(false);
        
        e.Property(p => p.EncryptedPhoneNumber).HasMaxLength(255).IsRequired(false);

        e.Property(p => p.NewHashedPhoneNumber).HasMaxLength(255).IsRequired(false);

        e.Property(p => p.NewEncryptedPhoneNumber).HasMaxLength(255).IsRequired(false);

        e.Property(v => v.CreatedAtUtc)
            .HasColumnType("datetime2")
            .IsRequired()
            .HasConversion(DateTimeExtension.UtcConverter());

        e.Property(v => v.UpdatedAtUtc)
            .HasColumnType("datetime2")
            .IsRequired()
            .HasConversion(DateTimeExtension.UtcConverter());

        e.Property(p => p.AccountStatus).HasConversion<string>();

        e.Property(p => p.Name)
            .HasMaxLength(100)
            .HasDefaultValue("")
            .IsRequired(); 

        e.Property(p => p.TimeZoneId)
            .HasMaxLength(40).IsRequired();

        e.HasMany(s => s.Fleets)
           .WithOne()
           .HasForeignKey(f => f.UserId)
           .OnDelete(DeleteBehavior.Cascade);

        e.HasOne(s => s.LoyaltyProgram)
           .WithOne(f => f.BaseUser)
           .HasForeignKey<LoyaltyProgram>(f => f.LoyaltyUserId)
           .OnDelete(DeleteBehavior.Cascade);

        e.HasOne(s => s.ReferralProgram)
           .WithOne(f => f.BaseUser)
           .HasForeignKey<ReferralProgram>(f => f.UserId)
           .OnDelete(DeleteBehavior.Cascade); 
         

        e.OwnsOne(p => p.Address, address =>
        {
            address.Property(ap => ap.EncryptedAddressLine1).HasMaxLength(255).IsRequired();

            address.Property(ap => ap.EncryptedAddressLine2).HasMaxLength(255).IsRequired(false);

            address.Property(ap => ap.EncryptedAddressLine3).HasMaxLength(255).IsRequired(false);

            address.Property(ap => ap.City).HasMaxLength(50).IsRequired();

            address.Property(ap => ap.State).HasMaxLength(50).IsRequired();

            address.Property(ap => ap.PostalCode).HasMaxLength(20).IsRequired();

            address.Property(ap => ap.Country).HasMaxLength(50);
        });
         
        e.Ignore(p => p.DomainEvents);
        e.Ignore(p => p.EntityStateAction);
    }
}
