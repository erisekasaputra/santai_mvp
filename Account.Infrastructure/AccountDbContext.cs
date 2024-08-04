using Account.Domain.Aggregates.BusinessLicenseAggregate;
using Account.Domain.Aggregates.IdentificationAggregate;
using Account.Domain.Aggregates.LoyaltyAggregate;
using Account.Domain.Aggregates.ReferralAggregate;
using Account.Domain.Aggregates.ReferredAggregate;
using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Enumerations;
using Microsoft.EntityFrameworkCore; 

namespace Account.Infrastructure;

public class AccountDbContext : DbContext
{
    public DbSet<User> Users { get; set; } 

    public DbSet<BusinessLicense> BusinessLicenses { get; set; }

    public DbSet<DrivingLicense> DrivingLicenses { get; set; } 
    
    public DbSet<NationalIdentity> NationalIdentities { get; set; }  
    
    public DbSet<LoyaltyProgram> LoyaltyPrograms { get; set; }
      
    public DbSet<ReferralProgram> ReferralPrograms { get; set; }

    public DbSet<ReferredProgram> ReferredPrograms { get; set; }

    public DbSet<Staff> Staffs {  get; set; }    

    public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasDiscriminator<string>("UserType")
            .HasValue<BusinessUser>("BusinessUser")
            .HasValue<RegularUser>("RegularUser")
            .HasValue<MechanicUser>("MechanicUser");

        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(e => e.Id);

            e.HasIndex(p => p.Username).IsUnique();
            
            e.HasIndex(p => p.Email).IsUnique();
            
            e.HasIndex(p => p.PhoneNumber).IsUnique();
            
            e.Property(p => p.Username).HasMaxLength(50).IsRequired();
             
            e.Property(p => p.Email).HasMaxLength(50).IsRequired();

            e.Property(p => p.PhoneNumber).HasMaxLength(50).IsRequired();

            e.Property(p => p.AccountStatus).HasConversion(
                save => save.ToString(), 
                retrieve => Enum.Parse<AccountStatus>(retrieve));

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
                address.Property(a => a.AddressLine1).HasMaxLength(255).IsRequired(); 
                
                address.Property(a => a.AddressLine2).HasMaxLength(255).IsRequired(false);
                
                address.Property(a => a.AddressLine3).HasMaxLength(255).IsRequired(false);
                
                address.Property(a => a.City).HasMaxLength(255).IsRequired();
                
                address.Property(a => a.State).HasMaxLength(255).IsRequired();
                
                address.Property(a => a.PostalCode).HasMaxLength(255).IsRequired();
                
                address.Property(a => a.Country).HasMaxLength(3);
            });

            e.Ignore(p => p.DomainEvents);
        });

        
        modelBuilder.Entity<BusinessUser>(e =>
        { 
            e.HasIndex(p => p.Code); 
          
            e.Property(p => p.Code).HasMaxLength(6).IsRequired();
            
            e.Property(p => p.BusinessName).HasMaxLength(50).IsRequired();
            
            e.Property(p => p.ContactPerson).HasMaxLength(50).IsRequired();
            
            e.Property(p => p.TaxId).HasMaxLength(50).IsRequired(false);
            
            e.Property(p => p.WebsiteUrl).HasMaxLength(255).IsRequired(false);
            
            e.Property(p => p.Description).HasMaxLength(1000).IsRequired(false);
            
            e.HasMany(p => p.BusinessLicenses)
                .WithOne(b => b.BusinessUser)
                .HasForeignKey(f => f.BusinessUserId)
                .OnDelete(DeleteBehavior.Cascade); 

            e.HasMany(p => p.Staffs)
                .WithOne(s => s.BusinessUser)
                .HasForeignKey(s => s.BusinessUserId)
                .HasPrincipalKey(s => s.Id)
                .OnDelete(DeleteBehavior.Cascade);

            e.Ignore(p => p.DomainEvents);
        });

        modelBuilder.Entity<Staff>(e =>
        {
            e.HasKey(p => p.Id);

            e.HasIndex(p => p.Username).IsUnique(); 
            
            e.HasIndex(p => p.PhoneNumber).IsUnique(); 
            
            e.HasIndex(p => p.Email).IsUnique(); 

            e.Property(p => p.Username).HasMaxLength(50).IsRequired();
            
            e.Property(p => p.BusinessUserId).HasMaxLength(50).IsRequired();
            
            e.Property(p => p.BusinessUserCode).HasMaxLength(6).IsRequired();
            
            e.Property(p => p.PhoneNumber).HasMaxLength(50).IsRequired();
            
            e.Property(p => p.NewPhoneNumber).HasMaxLength(50).IsRequired(false);
            
            e.Property(p => p.IsPhoneNumberVerified).IsRequired();
            
            e.Property(p => p.Email).HasMaxLength(50).IsRequired();
            
            e.Property(p => p.NewEmail).HasMaxLength(50).IsRequired(false);
            
            e.Property(p => p.IsEmailVerified).IsRequired();

            e.Property(p => p.Name).HasMaxLength(50).IsRequired();

            e.Property(p => p.DeviceId).HasMaxLength(50).IsRequired(false); 

            e.HasOne(s => s.BusinessUser)
                .WithMany(b => b.Staffs)
                .HasForeignKey(s => s.BusinessUserId)
                .OnDelete(DeleteBehavior.Restrict);

            e.OwnsOne(p => p.Address, address =>
            {
                address.Property(ap => ap.AddressLine1).HasMaxLength(255).IsRequired();
                                 
                address.Property(ap => ap.AddressLine2).HasMaxLength(255).IsRequired(false);
                                
                address.Property(ap => ap.AddressLine3).HasMaxLength(255).IsRequired(false);
                                 
                address.Property(ap => ap.City).HasMaxLength(255).IsRequired();
                           
                address.Property(ap => ap.State).HasMaxLength(255).IsRequired();
                             
                address.Property(ap => ap.PostalCode).HasMaxLength(255).IsRequired();
                         
                address.Property(ap => ap.Country).HasMaxLength(3);
            });

            e.Ignore(p => p.DomainEvents);
        });

        modelBuilder.Entity<MechanicUser>(e =>
        { 
            e.Property(p => p.Rating)
                .HasColumnType("decimal(5, 2)");

            e.Property(p => p.DeviceId)
                .HasMaxLength(255).IsRequired(true);

            e.HasMany(p => p.DrivingLicenses)
                .WithOne(p => p.MechanicUser)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasMany(p => p.NationalIdentities)
                .WithOne(p => p.MechanicUser)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            e.Ignore(p => p.DomainEvents); 
        });

        modelBuilder.Entity<RegularUser>(e =>
        {
            e.Property(p => p.DeviceId)
                .HasMaxLength(255).IsRequired(true);

            e.OwnsOne(p => p.PersonalInfo, personalInfo =>
            {
                personalInfo.Property(i => i.FirstName)
                    .HasMaxLength(50).IsRequired();

                personalInfo.Property(i => i.MiddleName)
                    .HasMaxLength(50).IsRequired();

                personalInfo.Property(i => i.LastName)
                    .HasMaxLength(50).IsRequired();

                personalInfo.Property(i => i.Gender)
                    .HasConversion(
                        save => save.ToString(), 
                        retrieve => Enum.Parse<Gender>(retrieve))
                    .IsRequired();
                
                personalInfo.Property(i => i.ProfilePictureUrl)
                    .HasMaxLength(255).IsRequired(false);
            });
        });

        modelBuilder.Entity<LoyaltyProgram>(e =>
        {
            e.HasKey(p => p.Id);

            e.Property(p => p.LoyaltyTier)
                .HasConversion(
                    save => save.ToString(),
                    retrieve => Enum.Parse<LoyaltyTier>(retrieve));

            e.HasOne(p => p.User)
                .WithOne(u => u.LoyaltyProgram)
                .HasForeignKey<LoyaltyProgram>(p => p.LoyaltyUserId)
                .OnDelete(DeleteBehavior.Restrict);

            e.Ignore(p => p.DomainEvents);
        });

        modelBuilder.Entity<ReferralProgram>(e =>
        {
            e.HasKey(p => p.Id);

            e.HasIndex(p => p.ReferralCode).IsUnique();

            e.Property(p => p.ReferralCode)
                .HasMaxLength(6).IsRequired();

            e.HasOne(p => p.User)
                .WithOne(u => u.ReferralProgram)
                .HasForeignKey<ReferralProgram>(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            e.Ignore(p => p.DomainEvents);
        });

        modelBuilder.Entity<ReferredProgram>(e =>
        {
            e.HasKey(p => p.Id);

            e.Property(p => p.ReferrerId).IsRequired();

            e.Property(p => p.ReferredUserId).IsRequired();

            e.Property(p => p.ReferralCode)
                .HasMaxLength(6).IsRequired();

            e.Property(p => p.Status)
                .HasConversion(
                    value => value.ToString(),
                    retrieve => Enum.Parse<ReferralStatus>(retrieve))
                .IsRequired();

            e.HasOne(p => p.User)
                .WithMany(p => p.ReferredPrograms)
                .HasForeignKey(p => p.ReferrerId)
                .OnDelete(DeleteBehavior.Restrict);

            e.Ignore(p => p.DomainEvents);
        });


        modelBuilder.Entity<DrivingLicense>(e =>
        {
            e.HasKey(p => p.Id);

            e.HasIndex(p => new { p.UserId, p.VerificationStatus })
                .IsUnique()
                .HasFilter("[VerificationStatus] = 'Accepted' ");

            e.Property(p => p.VerificationStatus)
                .HasConversion(
                    value => value.ToString(),
                    retrieve => Enum.Parse<VerificationState>(retrieve)); 
            
            e.Property(p => p.LicenseNumber).HasMaxLength(255).IsRequired();

            e.Property(p => p.FrontSideImageUrl).HasMaxLength(255).IsRequired();

            e.Property(p => p.BackSideImageUrl).HasMaxLength(255).IsRequired();

            e.HasQueryFilter(p => p.VerificationStatus != VerificationState.Rejected);
             
            e.HasOne(p => p.MechanicUser)
                .WithMany(p => p.DrivingLicenses)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            e.Ignore(p => p.DomainEvents); 
        });

        modelBuilder.Entity<NationalIdentity>(e =>
        {
            e.HasKey(p => p.Id);

            e.HasIndex(p => new { p.UserId, p.VerificationStatus })
                .IsUnique()
                .HasFilter("[VerificationStatus] =  'Accepted' ");

            e.Property(p => p.VerificationStatus)
                .HasConversion(
                    value => value.ToString(), 
                    retrieve => Enum.Parse<VerificationState>(retrieve));

            e.Property(p => p.IdentityNumber).HasMaxLength(255).IsRequired();

            e.Property(p => p.FrontSideImageUrl).HasMaxLength(255).IsRequired();

            e.Property(p => p.BackSideImageUrl).HasMaxLength(255).IsRequired();

            e.HasQueryFilter(p => p.VerificationStatus != VerificationState.Rejected);

            e.HasOne(p => p.MechanicUser)
                .WithMany(p => p.NationalIdentities)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            e.Ignore(p => p.DomainEvents); 
        });

        base.OnModelCreating(modelBuilder);
    }
}
