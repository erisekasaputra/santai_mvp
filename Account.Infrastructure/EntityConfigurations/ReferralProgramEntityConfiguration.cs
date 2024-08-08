using Account.Domain.Aggregates.ReferralAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.EntityConfigurations;

public class ReferralProgramEntityConfiguration : IEntityTypeConfiguration<ReferralProgram>
{
    public void Configure(EntityTypeBuilder<ReferralProgram> e)
    {
        e.HasKey(p => p.Id);

        e.HasIndex(p => p.ReferralCode)
            .IsUnique();

        e.Property(p => p.ReferralCode)
            .HasMaxLength(6)
            .IsRequired()
            .HasConversion(
                v => v.Trim(),
                v => v.Trim());

        e.HasOne(p => p.User)
            .WithOne(u => u.ReferralProgram)
            .HasForeignKey<ReferralProgram>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        e.Ignore(p => p.DomainEvents);
    }
}
