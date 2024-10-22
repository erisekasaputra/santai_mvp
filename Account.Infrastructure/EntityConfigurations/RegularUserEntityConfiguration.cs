using Account.Domain.Aggregates.UserAggregate; 
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Extensions;

namespace Account.Infrastructure.EntityConfigurations;

public class RegularUserEntityConfiguration : IEntityTypeConfiguration<RegularUser>
{
    public void Configure(EntityTypeBuilder<RegularUser> e)
    {  
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

            personalInfo.Property(i => i.DateOfBirthUtc)
                .HasConversion(DateTimeExtension.UtcConverter()); 
            
            personalInfo.Property(i => i.LastName)
                .HasMaxLength(50)
                .IsRequired()
                .HasConversion(
                    v => v == null ? null : v.Trim(),
                    v => v == null ? null : v.Trim());

            personalInfo.Property(i => i.Gender)
                .HasConversion<string>()
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
