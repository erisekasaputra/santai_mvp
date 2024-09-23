using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Enumerations; 
using Microsoft.EntityFrameworkCore; 
using LinqKit;

namespace Account.Infrastructure.Repositories;

public class StaffRepository : IStaffRepository
{
    private readonly AccountDbContext _context; 
    public StaffRepository(AccountDbContext context)
    {
        _context = context;
    } 

    public async Task<IEnumerable<Staff>?> GetByIdentitiesAsNoTrackingAsync(params (IdentityParameter, IEnumerable<string?>)[] parameters)
    { 
        if (parameters is null || parameters.Length == 0)
        {
            throw new ArgumentException("Please provide parameter type type you want to check at least one parameter type and value");
        }

        var predicate = PredicateBuilder.New<Staff>(true);

        foreach (var (identityParameter, values) in parameters)
        {
            foreach(var value in values)
            { 
                switch (identityParameter)
                { 
                    case IdentityParameter.Email:
                        predicate = predicate.Or(x => (x.HashedEmail == value) 
                            || (x.NewHashedEmail != null && x.NewHashedEmail == value));
                        break;
                    case IdentityParameter.PhoneNumber:
                        predicate = predicate.Or(x => (x.HashedPhoneNumber == value) 
                            || (x.NewHashedPhoneNumber != null && x.NewHashedPhoneNumber == value));
                        break; 
                }
            } 
        }

        return await _context.Staffs.Where(predicate).AsNoTracking().ToListAsync();
    } 
    
    public async Task<IEnumerable<Staff>?> GetByIdentitiesExcludingIdsAsNoTrackingAsync(params (IdentityParameter, IEnumerable<(Guid id, string identity)>)[] parameters)
    { 
        if (parameters is null || parameters.Length == 0)
        {
            throw new ArgumentException("Please provide parameter type type you want to check at least one parameter type and value");
        }

        var predicate = PredicateBuilder.New<Staff>(true);

        foreach (var (identityParameter, values) in parameters)
        {
            foreach (var (id, identity) in values)
            {  
                switch (identityParameter)
                { 
                    case IdentityParameter.Email:
                        predicate = predicate.Or(x => (x.Id != id && (x.HashedEmail == identity))
                            || (x.Id != id && (x.NewHashedEmail != null && x.NewHashedEmail == identity)));
                        break; 
                    case IdentityParameter.PhoneNumber:
                        predicate = predicate.Or(x => (x.Id != id && (x.HashedPhoneNumber == identity))
                            || (x.Id != id && (x.NewHashedPhoneNumber != null && x.NewHashedPhoneNumber == identity)));
                        break;
                }
            }
        }

        return await _context.Staffs.Where(predicate).AsNoTracking().ToListAsync();
    }

    public Task<Staff?> GetByBusinessUserIdAndStaffIdAsync(Guid userId, Guid staffId)
    {
        return _context.Staffs
            .Include(x => x.Fleets)
            .FirstOrDefaultAsync(x => x.BusinessUserId == userId && x.Id == staffId);
    }
     

    public async Task<Staff> CreateAsync(Staff staff)
    {
        var entity = await _context.Staffs.AddAsync(staff);
        return entity.Entity;
    } 

    public async Task<bool> GetAnyAsync(Guid id)
    {
        return await _context.Staffs.AnyAsync(x => x.Id == id);
    }

    public async Task<Staff?> GetByIdAsync(Guid id)
    {
        return await _context.Staffs
            .Include(x => x.Fleets)
            .FirstOrDefaultAsync(x => x.Id == id);
    } 

    public void Update(Staff staff)
    {
        _context.Staffs.Update(staff);
    }

    public void Delete(Staff staff)
    {
        _context.Remove(staff);
    }

    public async Task<bool> GetAnyByIdentitiesAsNoTrackingAsync(params (IdentityParameter, IEnumerable<string>)[] parameters)
    {
        if (parameters is null || parameters.Length == 0)
        {
            throw new ArgumentException("Please provide parameter type type you want to check at least one parameter type and value");
        }

        var predicate = PredicateBuilder.New<Staff>(true);

        foreach (var (identityParameter, values) in parameters)
        {
            foreach (var value in values)
            {
                switch (identityParameter)
                { 
                    case IdentityParameter.Email:
                        predicate = predicate.Or(x => (x.HashedEmail == value)
                            || (x.NewHashedEmail != null && x.NewHashedEmail == value));
                        break;
                    case IdentityParameter.PhoneNumber:
                        predicate = predicate.Or(x => (x.HashedPhoneNumber == value)
                            || (x.NewHashedPhoneNumber != null && x.NewHashedPhoneNumber == value));
                        break;
                }
            }
        }

        return await _context.Staffs.Where(predicate).AsNoTracking().AnyAsync();
    }

    public async Task<bool> GetAnyByIdentitiesExcludingIdsAsNoTrackingAsync(params (IdentityParameter, IEnumerable<(Guid id, string identity)>)[] parameters)
    {
        if (parameters is null || parameters.Length == 0)
        {
            throw new ArgumentException("Please provide parameter type type you want to check at least one parameter type and value");
        }

        var predicate = PredicateBuilder.New<Staff>(true);

        foreach (var (identityParameter, values) in parameters)
        {
            foreach (var (id, identity) in values)
            {
                switch (identityParameter)
                { 
                    case IdentityParameter.Email:
                        predicate = predicate.Or(x => (x.Id != id && (x.HashedEmail == identity))
                            || (x.Id != id && (x.NewHashedEmail != null && x.NewHashedEmail == identity)));
                        break;
                    case IdentityParameter.PhoneNumber:
                        predicate = predicate.Or(x => x.Id != id && (x.HashedPhoneNumber == identity)
                            || (x.Id != id && (x.NewHashedPhoneNumber != null && x.NewHashedPhoneNumber == identity)));
                        break;
                }
            }
        }

        return await _context.Staffs.Where(predicate).AsNoTracking().AnyAsync();
    }

    public async Task<(int TotalCount, int TotalPages, IEnumerable<Staff> Staffs)> GetPaginatedStaffByUserIdAsync(
        Guid userId,
        int pageNumber,
        int pageSize)
    {
        var query = _context.Staffs.AsQueryable();

        var totalCount = await query.CountAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query.Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()    
            .Where(x => x.BusinessUserId == userId)
            .OrderBy(x => x.Name) 
            .ToListAsync();

        return (totalCount, totalPages, items);
    }

    public async Task<string?> GetTimeZoneByIdAsync(Guid staffId)
    {
        return await _context.Staffs
            .Where(x => x.Id == staffId)
            .Select(x => x.TimeZoneId)
            .FirstOrDefaultAsync();
    }

    public async Task<string?> GetEmailByIdAsync(Guid staffId)
    {
        return await _context.Staffs
           .Where(x => x.Id == staffId)
           .Select(x => x.EncryptedEmail)
           .FirstOrDefaultAsync();
    }

    public async Task<string?> GetPhoneNumberByIdAsync(Guid staffId)
    {
        return await _context.Staffs
           .Where(x => x.Id == staffId)
           .Select(x => x.EncryptedPhoneNumber)
           .FirstOrDefaultAsync();
    } 
}
