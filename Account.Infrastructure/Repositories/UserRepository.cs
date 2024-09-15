using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Enumerations;
using Microsoft.EntityFrameworkCore;
using LinqKit;
using Core.Enumerations; 

namespace Account.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{      
    private readonly AccountDbContext _context;

    public UserRepository(AccountDbContext context)
    {
        _context = context;
    }

    public async Task<UserType?> GetUserTypeById(Guid id)
    {
        var user = await _context.BaseUsers.Where(x => x.Id == id)
            .Select(u => new
            {  
                UserType = EF.Property<string>(u, "UserType")
            }).FirstOrDefaultAsync();

        if (user == null)
        {
            return null;
        } 

        return Enum.Parse<UserType>(user.UserType);
    }

    public async Task<BaseUser> CreateAsync(BaseUser user)
    {
        var entry = await _context.BaseUsers.AddAsync(user); 

        return entry.Entity;
    }

    public void Update(BaseUser user)
    {   
        _context.BaseUsers.Update(user);
    }

    public async Task<BusinessUser?> GetBusinessUserByIdAsync(Guid id)
    {
        return await _context.BaseUsers.OfType<BusinessUser>()
            .Include(x => x.BusinessLicenses) 
            .Include(x => x.ReferralProgram)
            .Include(x => x.LoyaltyProgram)
            .Include(x => x.Fleets) 
            .FirstOrDefaultAsync(x => x.Id == id);
    } 
    
    public async Task<RegularUser?> GetRegularUserByIdAsync(Guid id)
    {
        return await _context.BaseUsers.OfType<RegularUser>()
            .Include(x => x.LoyaltyProgram)
            .Include(x => x.ReferralProgram)
            .Include(x => x.Fleets)
            .FirstOrDefaultAsync(x => x.Id == id);
    } 

    public async Task<MechanicUser?> GetMechanicUserByIdAsync(Guid id)
    {
        return await _context.BaseUsers.OfType<MechanicUser>()
            .Include(x => x.LoyaltyProgram)
            .Include(x => x.ReferralProgram)
            .Include(x => x.NationalIdentities)
            .Include(x => x.DrivingLicenses)
            .Include(x => x.Certifications)
            .Include(x => x.MechanicOrderTask)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<BaseUser?> GetByIdentitiesAsNoTrackingAsync(params (IdentityParameter, string?)[] identity)
    { 
        if (identity is null || identity.Length == 0)
        {
            throw new ArgumentException("Please provide identity type you want to check at least one identity type and value");   
        }

        var predicate = PredicateBuilder.New<BaseUser>(true);

        foreach (var (parameter, value) in identity)
        {  
            switch (parameter)
            { 
                case IdentityParameter.Email:
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        predicate = predicate.Or(x => x.HashedEmail == value
                        || (x.NewHashedEmail != null && x.NewHashedEmail == value));
                    }
                    break;
                case IdentityParameter.PhoneNumber:
                    predicate = predicate.Or(x => x.HashedPhoneNumber == value
                    || (x.NewHashedPhoneNumber != null && x.NewHashedPhoneNumber == value));
                    break; 
            }
        }
         
        return await _context.BaseUsers.AsNoTracking().Where(predicate).FirstOrDefaultAsync();
    }

    public async Task<BaseUser?> GetByIdentitiesExcludingIdAsNoTrackingAsync(Guid id, params (IdentityParameter, string?)[] identity)
    { 
        if (identity.Length == 0)
        {
            throw new ArgumentException("Please provide identity type you want to check at least one identity type and value");
        }

        var predicate = PredicateBuilder.New<BaseUser>(true);
          
        foreach (var (parameter, value) in identity)
        { 
            switch (parameter)
            { 
                case IdentityParameter.Email:
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        predicate = predicate.Or(x => x.HashedEmail == value
                        || (x.NewHashedEmail != null && x.NewHashedEmail == value));
                    }
                    break;
                case IdentityParameter.PhoneNumber:
                    predicate = predicate.Or(x => x.HashedPhoneNumber == value
                    || (x.NewHashedPhoneNumber != null && x.NewHashedPhoneNumber == value));
                    break; 
            }
        }
        return await _context.BaseUsers.Where(predicate).FirstOrDefaultAsync(x => x.Id != id);
    }

    public void Delete(BaseUser user)
    {
        _context.BaseUsers.Remove(user);
    }

    public Task<BaseUser?> GetByIdAsync(Guid id)
    {
        return _context.BaseUsers
            .Include(x => x.LoyaltyProgram)
            .Include(x => x.ReferralProgram)    
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> GetAnyByIdAsync(Guid id)
    {
        return await _context.BaseUsers.AnyAsync(x => x.Id == id);
    }

    public async Task<bool> GetAnyByIdentitiesAsNoTrackingAsync(params (IdentityParameter, string?)[] identity)
    {
        if (identity is null || identity.Length == 0)
        {
            throw new ArgumentException("Please provide identity type you want to check at least one identity type and value");
        }

        var predicate = PredicateBuilder.New<BaseUser>(true);

        foreach (var (parameter, value) in identity)
        {
            switch (parameter)
            { 
                case IdentityParameter.Email:
                    if (!string.IsNullOrEmpty(value))
                    {
                        predicate = predicate.Or(x => x.HashedEmail == value
                        || (x.NewHashedEmail != null && x.NewHashedEmail == value));
                    }
                    break;
                case IdentityParameter.PhoneNumber:
                    predicate = predicate.Or(x => x.HashedPhoneNumber == value
                    || (x.NewHashedPhoneNumber != null && x.NewHashedPhoneNumber == value));
                    break; 
            }
        }

        return await _context.BaseUsers.AsNoTracking().Where(predicate).AnyAsync();
    }

    public async Task<bool> GetAnyByIdentitiesExcludingIdAsNoTrackingAsync(Guid id, params (IdentityParameter, string?)[] identity)
    {
        if (identity.Length == 0)
        {
            throw new ArgumentException("Please provide identity type you want to check at least one identity type and value");
        }

        var predicate = PredicateBuilder.New<BaseUser>(true);

        foreach (var (parameter, value) in identity)
        {
            switch (parameter)
            { 
                case IdentityParameter.Email:
                    if (!string.IsNullOrEmpty(value))
                    { 
                        predicate = predicate.Or(x => x.HashedEmail == value
                        || (x.NewHashedEmail != null && x.NewHashedEmail == value));
                    }
                    break;
                case IdentityParameter.PhoneNumber:
                    predicate = predicate.Or(x => x.HashedPhoneNumber == value
                    || (x.NewHashedPhoneNumber != null && x.NewHashedPhoneNumber == value));
                    break; 
            }
        }
        return await _context.BaseUsers.Where(predicate).AnyAsync(x => x.Id != id);
    }

    public async Task<(int TotalCount, int TotalPages, IEnumerable<RegularUser> Brands)> GetPaginatedRegularUser(int pageNumber, int pageSize)
    {
        var query = _context.BaseUsers.AsQueryable();

        var totalCount = await query.CountAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query.Skip((pageNumber - 1) * pageSize)
            .Take(pageSize) 
            .AsNoTracking()
            .OfType<RegularUser>()
            .Include(x => x.ReferralProgram)
            .Include(x => x.LoyaltyProgram)
            .OrderByDescending(x => x.AccountStatus == AccountStatus.Active) 
            .OrderBy(x => x.PersonalInfo.FirstName)
            .ToListAsync();

        return (totalCount, totalPages, items);
    }

    public async Task<(int TotalCount, int TotalPages, IEnumerable<BusinessUser> Brands)> GetPaginatedBusinessUser(int pageNumber, int pageSize)
    {
        var query = _context.BaseUsers.AsQueryable();

        var totalCount = await query.CountAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query.Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .OfType<BusinessUser>()
            .Include(x => x.ReferralProgram)
            .Include(x => x.LoyaltyProgram)  
            .OrderByDescending(x => x.AccountStatus == AccountStatus.Active)
            .OrderBy(x => x.BusinessName)       
            .ToListAsync();

        return (totalCount, totalPages, items);
    }

    public async Task<(int TotalCount, int TotalPages, IEnumerable<MechanicUser> Brands)> GetPaginatedMechanicUser(int pageNumber, int pageSize)
    {
        var query = _context.BaseUsers.AsQueryable();

        var totalCount = await query.CountAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query.Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .OfType<MechanicUser>()
            .Include(x => x.ReferralProgram)
            .Include(x => x.LoyaltyProgram)  
            .Include(x => x.MechanicOrderTask)
            .OrderByDescending(x => x.AccountStatus == AccountStatus.Active)
            .OrderByDescending(x => x.IsVerified)
            .OrderBy(x => x.PersonalInfo.FirstName)
            .ToListAsync();

        return (totalCount, totalPages, items);
    }

    public async Task<string?> GetTimeZoneById(Guid id)
    {
        return await _context.BaseUsers 
            .Where(x => x.Id == id)
            .Select(x => x.TimeZoneId)
            .FirstOrDefaultAsync();
    }

    public async Task<string?> GetEmailById(Guid id)
    {
        return await _context.BaseUsers
           .Where(x => x.Id == id)
           .Select(x => x.EncryptedEmail)
           .FirstOrDefaultAsync();
    }

    public async Task<string?> GetPhoneNumberById(Guid id)
    {
        return await _context.BaseUsers
           .Where(x => x.Id == id)
           .Select(x => x.EncryptedPhoneNumber)
           .FirstOrDefaultAsync();
    }

    public async Task<string?> GetDeviceIdByMechanicUserId(Guid id)
    {
        return await _context.BaseUsers
           .OfType<MechanicUser>()
           .Where(x => x.Id == id)
           .Select(x => x.DeviceId)
           .FirstOrDefaultAsync();
    }

    public async Task<string?> GetDeviceIdByRegularUserId(Guid id)
    {
        return await _context.BaseUsers
           .OfType<RegularUser>()
           .Where(x => x.Id == id)
           .Select(x => x.DeviceId)
           .FirstOrDefaultAsync();
    } 
}
