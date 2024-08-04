using Account.Domain.Aggregates.BusinessLicenseAggregate;
using Account.Domain.Aggregates.IdentificationAggregate;
using Account.Domain.Aggregates.LoyaltyAggregate;
using Account.Domain.Aggregates.ReferredAggregate;
using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Enumerations;
using Account.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Account.Infrastructure;

public class AccountDbContext : DbContext
{
    public DbSet<User> Users { get; set; } 

    public DbSet<BusinessLicense> BusinessLicenses { get; set; }

    public DbSet<Identification> Identifications { get; set; }  
    
    public DbSet<LoyaltyProgram> LoyaltyPrograms { get; set; }
      
    public DbSet<ReferralProgram> ReferralPrograms { get; set; }

    public DbSet<ReferredProgram> ReferredPrograms { get; set; }

    public DbSet<Staff> Staffs {  get; set; }    

    public AccountDbContext(DbContextOptions<AccountDbContext> options) : base()
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
            
            e.HasIndex(p =>p.Email).IsUnique();
            
            e.HasIndex(p => p.PhoneNumber).IsUnique();
            
            e.Property(p => p.Username).HasMaxLength(20).IsRequired();
             
            e.Property(p => p.Email).HasMaxLength(255).IsRequired();

            e.Property(p => p.PhoneNumber).HasMaxLength(20).IsRequired();

            e.Property(p => p.AccountStatus).HasConversion(
                save => save.ToString(), 
                retrieve => Enum.Parse<AccountStatus>(retrieve));

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
        });

        modelBuilder.Entity<Identification>(e =>
        {
            e.HasKey(e => e.Id);
            
            e.HasIndex(e => e.IdentificationNumber);

            e.HasIndex(e => e.Type);

            e.Property(e => e.IdentificationNumber).HasMaxLength(20).IsRequired();

            e.Property(e => e.Type)
                .HasConversion(save => save.ToString(), retrieve => Enum.Parse<IdentificationType>(retrieve)).IsRequired();

            e.HasQueryFilter(c => c.IsVerified != DocumentVerificationStatus.Rejected
                && (c.Type == IdentificationType.NationalID || c.Type == IdentificationType.DrivingLicense));

            e.Property(e => e.FrontSideImageUrl).HasMaxLength(255).IsRequired();

            e.Property(e => e.BackSideImageUrl).HasMaxLength(255).IsRequired();
        });

        modelBuilder.Entity<BusinessUser>(e =>
        { 
            e.HasIndex(p => p.Code); 
          
            e.Property(p => p.Code).HasMaxLength(6).IsRequired();
            
            e.Property(p => p.BusinessName).HasMaxLength(100).IsRequired();
            
            e.Property(p => p.ContactPerson).HasMaxLength(50).IsRequired();
            
            e.Property(p => p.TaxId).HasMaxLength(100).IsRequired(false);
            
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
        });

        modelBuilder.Entity<Staff>(e =>
        {
            e.HasOne(s => s.BusinessUser)
                .WithMany(b => b.Staffs)
                .HasForeignKey(s => s.BusinessUserId)
                .OnDelete(DeleteBehavior.Restrict);  
        });

        modelBuilder.Entity<MechanicUser>(e =>
        { 

        });

        base.OnModelCreating(modelBuilder);
    }
}
