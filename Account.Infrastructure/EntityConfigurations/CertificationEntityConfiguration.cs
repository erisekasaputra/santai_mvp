using Account.Domain.Aggregates.CertificationAggregate;
using Core.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders; 

namespace Account.Infrastructure.EntityConfigurations;

public class CertificationEntityConfiguration : IEntityTypeConfiguration<Certification>
{
    public void Configure(EntityTypeBuilder<Certification> e)
    {
        e.HasKey(p => p.Id);

        e.HasIndex(p => p.CertificationId)
            .IsUnique();

        e.Property(p => p.CertificationId)
            .HasMaxLength(255)
            .IsRequired()
            .HasConversion(
                v => v.Trim(),
                v => v.Trim());

        e.Property(p => p.ValidDateUtc)
            .HasColumnType("datetime2")
            .IsRequired(false)
            .HasConversion(DateTimeExtension.UtcConverter());    

        e.Property(p => p.CertificationName)
            .HasMaxLength(100)
            .IsRequired()
            .HasConversion(
                v => v.Trim(),
                v => v.Trim());

        e.Property(p => p.Specializations)
            .HasConversion(
                v => v == null ? null : string.Join(',', v.Distinct()),  
                v => v == null ? null : v.Split(',', StringSplitOptions.RemoveEmptyEntries).Distinct().ToHashSet(), 
                new ValueComparer<ICollection<string>>(
                    (c1, c2) => c1 == null && c2 == null || c1 != null && c2 != null && c1.OrderBy(x => x).SequenceEqual(c2.OrderBy(x => x)),  
                    c => c == null ? 0 : c.OrderBy(x => x).Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),  
                    c => c == null ? new HashSet<string>() : new HashSet<string>(c)  
                )
            );

        e.HasOne(p => p.MechanicUser)
            .WithMany(p => p.Certifications)
            .HasForeignKey(p => p.MechanicUserId)
            .OnDelete(DeleteBehavior.Cascade);

        e.Ignore(p => p.DomainEvents);
        e.Ignore(p => p.EntityStateAction);
    }
}
