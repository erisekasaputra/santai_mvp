 
using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Enumerations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.EntityConfigurations;

public class MechanicUserEntityConfiguration : IEntityTypeConfiguration<MechanicUser>
{
    public void Configure(EntityTypeBuilder<MechanicUser> e)
    {
        e.Property(p => p.Rating)
                .HasColumnType("decimal(5, 2)");

        e.Property(p => p.DeviceId)
            .HasMaxLength(255)
            .IsRequired(false)
            .HasConversion(
                v => v == null ? null : v.Trim(),
                v => v == null ? null : v.Trim()); 

        e.OwnsOne(p => p.PersonalInfo, personalInfo =>
        {
            personalInfo.Property(i => i.FirstName)
                .HasMaxLength(50)
                .IsRequired()
                .HasConversion(
                    v => v.Trim(),
                    v => v.Trim());

            personalInfo.Property(i => i.MiddleName)
                .HasMaxLength(50)
                .IsRequired()
                .HasConversion(
                    v => v == null ? null : v.Trim(),
                    v => v == null ? null : v.Trim());

            personalInfo.Property(i => i.LastName)
                .HasMaxLength(50)
                .IsRequired()
                .HasConversion(
                    v => v == null ? null : v.Trim(),
                    v => v == null ? null : v.Trim());

            personalInfo.Property(i => i.Gender)
                .HasConversion(
                    save => save.ToString(),
                    retrieve => Enum.Parse<Gender>(retrieve))
                .IsRequired();

            personalInfo.Property(i => i.ProfilePictureUrl)
                .HasMaxLength(255)
                .IsRequired(false)
                .HasConversion(
                    v => v == null ? null : v.Trim(),
                    v => v == null ? null : v.Trim());
        }); 

        //e.Ignore(p => p.DomainEvents); // no need domain event because the parent entity (BaseUser is implementing DomainEvent ignorance) 
    }
}
