using Account.Domain.Aggregates.LoyaltyAggregate;
using Account.Domain.Enumerations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.EntityConfigurations;

public class LoyaltyProgramEntityConfiguration : IEntityTypeConfiguration<LoyaltyProgram>
{
    public void Configure(EntityTypeBuilder<LoyaltyProgram> e)
    {
        e.HasKey(p => p.Id);

        e.Property(p => p.LoyaltyTier)
            .HasConversion<string>();

        e.HasOne(p => p.BaseUser)
            .WithOne(u => u.LoyaltyProgram)
            .HasForeignKey<LoyaltyProgram>(p => p.LoyaltyUserId)
            .OnDelete(DeleteBehavior.Cascade);

        e.Ignore(p => p.DomainEvents);
        e.Ignore(p => p.EntityStateAction);
    }
}
