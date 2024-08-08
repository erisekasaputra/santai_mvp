using Account.Domain.Aggregates.LoyaltyAggregate;
using Account.Domain.Aggregates.ReferralAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Enumerations;

namespace Account.Infrastructure.EntityConfigurations;

public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> e)
    {
        e.HasKey(e => e.Id);

        e.HasIndex(p => p.Username).IsUnique();

        e.HasIndex(p => p.Email).IsUnique();

        e.HasIndex(p => p.PhoneNumber).IsUnique();

        e.Property(p => p.Username).HasMaxLength(20).IsRequired();
          
        e.Property(p => p.Email).HasMaxLength(254).IsRequired();

        e.Property(p => p.NewEmail).HasMaxLength(254).IsRequired(false);

        e.Property(p => p.PhoneNumber).HasMaxLength(20).IsRequired();

        e.Property(p => p.NewPhoneNumber).HasMaxLength(20).IsRequired(false);

        e.Property(p => p.AccountStatus).HasConversion(
            save => save.ToString(),
            retrieve => Enum.Parse<AccountStatus>(retrieve));

        e.Property(p => p.TimeZoneId)
            .HasMaxLength(40).IsRequired();

        e.HasOne(p => p.LoyaltyProgram)
            .WithOne(p => p.User)
            .HasForeignKey<LoyaltyProgram>(p => p.LoyaltyUserId)
            .OnDelete(DeleteBehavior.Cascade);

        e.HasOne(p => p.ReferralProgram)
            .WithOne(p => p.User)
            .HasForeignKey<ReferralProgram>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        e.HasMany(p => p.ReferredPrograms)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.ReferrerId)
            .OnDelete(DeleteBehavior.Cascade);

        e.OwnsOne(p => p.Address, address =>
        {
            address.Property(ap => ap.AddressLine1).HasMaxLength(255).IsRequired();

            address.Property(ap => ap.AddressLine2).HasMaxLength(255).IsRequired(false);

            address.Property(ap => ap.AddressLine3).HasMaxLength(255).IsRequired(false);

            address.Property(ap => ap.City).HasMaxLength(50).IsRequired();

            address.Property(ap => ap.State).HasMaxLength(50).IsRequired();

            address.Property(ap => ap.PostalCode).HasMaxLength(20).IsRequired();

            address.Property(ap => ap.Country).HasMaxLength(50);
        });

        e.Ignore(p => p.DomainEvents);
    }
}
