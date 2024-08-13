using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Enumerations; 
using Microsoft.EntityFrameworkCore;
using LinqKit;

namespace Account.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{      
    private readonly AccountDbContext _context;

    public UserRepository(AccountDbContext context)
    {
        _context = context;
    }

    public async Task<User> CreateAsync(User user)
    {
        var entry = await _context.Users.AddAsync(user); 

        return entry.Entity;
    }

    public void Update(User user)
    {   
        _context.Users.Update(user);
    }

    public async Task<BusinessUser?> GetBusinessUserByIdAsync(Guid id)
    {
        return await _context.Users.OfType<BusinessUser>()
            .Include(x => x.BusinessLicenses)
            .Include(x => x.Staffs) 
            .Include(x => x.ReferralProgram)
            .Include(x => x.LoyaltyProgram) 
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == id);
    } 
    
    public async Task<RegularUser?> GetRegularUserByIdAsync(Guid id)
    {
        return await _context.Users.OfType<RegularUser>()
            .Include(x => x.LoyaltyProgram)
            .Include(x => x.ReferralProgram) 
            .FirstOrDefaultAsync(x => x.Id == id);
    } 

    public async Task<MechanicUser?> GetMechanicUserByIdAsync(Guid id)
    {
        return await _context.Users.OfType<MechanicUser>()
            .Include(x => x.LoyaltyProgram)
            .Include(x => x.ReferralProgram)
            .Include(x => x.NationalIdentities)
            .Include(x => x.DrivingLicenses)
            .Include(x => x.Certifications) 
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<User?> GetByIdentitiesAsNoTrackingAsync(params (IdentityParameter, string)[] identity)
    { 
        if (identity is null || identity.Length == 0)
        {
            throw new ArgumentException("Please provide identity type you want to check at least one identity type and value");   
        }

        var predicate = PredicateBuilder.New<User>(true);

        foreach (var (parameter, value) in identity)
        {  
            switch (parameter)
            {
                case IdentityParameter.Username:
                    predicate = predicate.Or(x => x.Username == value);
                    break;
                case IdentityParameter.Email:
                    predicate = predicate.Or(x => x.HashedEmail == value
                    || (x.NewHashedEmail != null && x.NewHashedEmail == value));
                    break;
                case IdentityParameter.PhoneNumber:
                    predicate = predicate.Or(x => x.HashedPhoneNumber == value
                    || (x.NewHashedPhoneNumber != null && x.NewHashedPhoneNumber == value));
                    break;
                case IdentityParameter.IdentityId:
                    predicate = predicate.Or(x => x.IdentityId == Guid.Parse(value));
                    break;
            }
        }
         
        return await _context.Users.AsNoTracking().Where(predicate).FirstOrDefaultAsync();
    }

    public async Task<User?> GetByIdentitiesExcludingIdAsNoTrackingAsync(Guid id, params (IdentityParameter, string)[] identity)
    { 
        if (identity.Length == 0)
        {
            throw new ArgumentException("Please provide identity type you want to check at least one identity type and value");
        }

        var predicate = PredicateBuilder.New<User>(true);
          
        foreach (var (parameter, value) in identity)
        { 
            switch (parameter)
            {
                case IdentityParameter.Username:
                    predicate = predicate.Or(x => x.Username == value);
                    break;
                case IdentityParameter.Email:
                    predicate = predicate.Or(x => x.HashedEmail == value
                    || (x.NewHashedEmail != null && x.NewHashedEmail == value));
                    break;
                case IdentityParameter.PhoneNumber:
                    predicate = predicate.Or(x => x.HashedPhoneNumber == value
                    || (x.NewHashedPhoneNumber != null && x.NewHashedPhoneNumber == value));
                    break;
                case IdentityParameter.IdentityId:
                    predicate = predicate.Or(x => x.IdentityId == Guid.Parse(value));
                    break;
            }
        }
        return await _context.Users.Where(predicate).FirstOrDefaultAsync(x => x.Id != id);
    }

    public void Delete(User user)
    {
        _context.Users.Remove(user);
    }

    public Task<User?> GetByIdAsync(Guid id)
    {
        return _context.Users.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> GetAnyByIdAsync(Guid id)
    {
        return await _context.Users.AnyAsync(x => x.Id == id);
    }

    public async Task<bool> GetAnyByIdentitiesAsNoTrackingAsync(params (IdentityParameter, string)[] identity)
    {
        if (identity is null || identity.Length == 0)
        {
            throw new ArgumentException("Please provide identity type you want to check at least one identity type and value");
        }

        var predicate = PredicateBuilder.New<User>(true);

        foreach (var (parameter, value) in identity)
        {
            switch (parameter)
            {
                case IdentityParameter.Username:
                    predicate = predicate.Or(x => x.Username == value);
                    break;
                case IdentityParameter.Email:
                    predicate = predicate.Or(x => x.HashedEmail == value
                    || (x.NewHashedEmail != null && x.NewHashedEmail == value));
                    break;
                case IdentityParameter.PhoneNumber:
                    predicate = predicate.Or(x => x.HashedPhoneNumber == value
                    || (x.NewHashedPhoneNumber != null && x.NewHashedPhoneNumber == value));
                    break;
                case IdentityParameter.IdentityId:
                    predicate = predicate.Or(x => x.IdentityId == Guid.Parse(value));
                    break;
            }
        }

        return await _context.Users.AsNoTracking().Where(predicate).AnyAsync();
    }

    public async Task<bool> GetAnyByIdentitiesExcludingIdAsNoTrackingAsync(Guid id, params (IdentityParameter, string)[] identity)
    {
        if (identity.Length == 0)
        {
            throw new ArgumentException("Please provide identity type you want to check at least one identity type and value");
        }

        var predicate = PredicateBuilder.New<User>(true);

        foreach (var (parameter, value) in identity)
        {
            switch (parameter)
            {
                case IdentityParameter.Username:
                    predicate = predicate.Or(x => x.Username == value);
                    break;
                case IdentityParameter.Email:
                    predicate = predicate.Or(x => x.HashedEmail == value
                    || (x.NewHashedEmail != null && x.NewHashedEmail == value));
                    break;
                case IdentityParameter.PhoneNumber:
                    predicate = predicate.Or(x => x.HashedPhoneNumber == value
                    || (x.NewHashedPhoneNumber != null && x.NewHashedPhoneNumber == value));
                    break;
                case IdentityParameter.IdentityId:
                    predicate = predicate.Or(x => x.IdentityId == Guid.Parse(value));
                    break;
            }
        }
        return await _context.Users.Where(predicate).AnyAsync(x => x.Id != id);
    }

    public async Task<(int TotalCount, int TotalPages, IEnumerable<RegularUser> Brands)> GetPaginatedRegularUser(int pageNumber, int pageSize)
    {
        var query = _context.Users.AsQueryable();

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
        var query = _context.Users.AsQueryable();

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
        var query = _context.Users.AsQueryable();

        var totalCount = await query.CountAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query.Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .OfType<MechanicUser>()
            .Include(x => x.ReferralProgram)
            .Include(x => x.LoyaltyProgram)  
            .OrderByDescending(x => x.AccountStatus == AccountStatus.Active)
            .OrderByDescending(x => x.IsVerified)
            .OrderBy(x => x.PersonalInfo.FirstName)
            .ToListAsync();

        return (totalCount, totalPages, items);
    }
}
