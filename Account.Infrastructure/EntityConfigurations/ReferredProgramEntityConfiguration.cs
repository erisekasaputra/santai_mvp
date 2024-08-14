using Account.Domain.Aggregates.ReferredAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.EntityConfigurations;

public class ReferredProgramEntityConfiguration : IEntityTypeConfiguration<ReferredProgram>
{
    public void Configure(EntityTypeBuilder<ReferredProgram> e)
    {
        e.HasKey(p => p.Id);

        e.HasIndex(p => p.ReferredUserId)
            .IsUnique();

        e.Property(p => p.ReferrerId)
            .IsRequired();

        e.Property(p => p.ReferredUserId)
            .IsRequired();

        e.Property(v => v.ReferredDateUtc)
            .HasColumnType("datetime2")
            .IsRequired();

        e.Property(p => p.ReferralCode)
            .HasMaxLength(6)
            .IsRequired()
            .HasConversion(
                v => v.Trim(),
                v => v.Trim());

        e.Property(p => p.Status)
            .HasConversion<string>()
            .IsRequired();

        e.HasOne(p => p.User)
            .WithMany(p => p.ReferredPrograms)
            .HasForeignKey(p => p.ReferrerId)
            .OnDelete(DeleteBehavior.Cascade);

        e.Ignore(p => p.DomainEvents);
    }
}
