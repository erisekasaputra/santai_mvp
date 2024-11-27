 
using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Enumerations;
using Core.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders; 
using Newtonsoft.Json;

namespace Account.Infrastructure.EntityConfigurations;

public class MechanicUserEntityConfiguration : IEntityTypeConfiguration<MechanicUser>
{
    public void Configure(EntityTypeBuilder<MechanicUser> e)
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


        e.Property(p => p.Ratings)
             .HasConversion(
                value => JsonConvert.SerializeObject(value),
                value => JsonConvert.DeserializeObject<List<decimal>>(value) ?? new List<decimal>()
             );

        e.Ignore(p => p.Rating); 
    }
}
