using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Enumerations; 
using LinqKit;
using Microsoft.EntityFrameworkCore; 

namespace Account.Infrastructure.Repositories;

public class StaffRepository : IStaffRepository
{
    private readonly AccountDbContext _context; 
    public StaffRepository(AccountDbContext context)
    {
        _context = context;
    } 

    public async Task<IEnumerable<Staff>?> GetByIdentitiesAsNoTrackingAsync(params (IdentityParameter, IEnumerable<string>)[] parameters)
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
                    case IdentityParameter.Username:
                        predicate = predicate.Or(x => x.Username == value);
                        break;
                    case IdentityParameter.Email:
                        predicate = predicate.Or(x => (x.Email == value) 
                            || (x.NewEmail != null && x.NewEmail == value));
                        break;
                    case IdentityParameter.PhoneNumber:
                        predicate = predicate.Or(x => (x.PhoneNumber == value) 
                            || (x.NewPhoneNumber != null && x.NewPhoneNumber == value));
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
                    case IdentityParameter.Username:
                        predicate = predicate.Or(x => x.Id != id && x.Username == identity);
                        break;
                    case IdentityParameter.Email:
                        predicate = predicate.Or(x => x.Id != id && (x.Email == identity)
                            || (x.NewEmail != null && x.NewEmail == identity));
                        break; 
                    case IdentityParameter.PhoneNumber:
                        predicate = predicate.Or(x => x.Id != id && (x.PhoneNumber == identity)
                            || (x.NewPhoneNumber != null && x.NewPhoneNumber == identity));
                        break;
                }
            }
        }

        return await _context.Staffs.Where(predicate).AsNoTracking().ToListAsync();
    }

    public Task<Staff?> GetByBusinessUserIdAndStaffIdAsync(Guid userId, Guid staffId)
    {
        return _context.Staffs.FirstOrDefaultAsync(x => x.BusinessUserId == userId && x.Id == staffId);
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
        return await _context.Staffs.FirstOrDefaultAsync(x => x.Id == id);
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
                    case IdentityParameter.Username:
                        predicate = predicate.Or(x => x.Username == value);
                        break;
                    case IdentityParameter.Email:
                        predicate = predicate.Or(x => (x.Email == value)
                            || (x.NewEmail != null && x.NewEmail == value));
                        break;
                    case IdentityParameter.PhoneNumber:
                        predicate = predicate.Or(x => (x.PhoneNumber == value)
                            || (x.NewPhoneNumber != null && x.NewPhoneNumber == value));
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
                    case IdentityParameter.Username:
                        predicate = predicate.Or(x => x.Id != id && x.Username == identity);
                        break;
                    case IdentityParameter.Email:
                        predicate = predicate.Or(x => x.Id != id && (x.Email == identity)
                            || (x.NewEmail != null && x.NewEmail == identity));
                        break;
                    case IdentityParameter.PhoneNumber:
                        predicate = predicate.Or(x => x.Id != id && (x.PhoneNumber == identity)
                            || (x.NewPhoneNumber != null && x.NewPhoneNumber == identity));
                        break;
                }
            }
        }

        return await _context.Staffs.Where(predicate).AsNoTracking().AnyAsync();
    }
}
