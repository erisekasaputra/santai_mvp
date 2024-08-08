using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Enumerations; 
using LinqKit; 
using Microsoft.EntityFrameworkCore;

namespace Account.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{ 
    private readonly AccountDbContext _context;

    public UserRepository(AccountDbContext context)
    {
        _context = context;
    }

    public async Task<User> CreateUserAsync(User user)
    {
        var entry = await _context.Users.AddAsync(user); 
        return entry.Entity;
    }

    public void UpdateUser(User user)
    {
        _context.Users.Update(user);   
    }

    public async Task<BusinessUser?> GetBusinessUserByIdAsync(Guid id)
    {
        return await _context.Users.OfType<BusinessUser>()
            .Include(x => x.BusinessLicenses) 
            .Include(x => x.Staffs)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == id);
    } 
    
    public async Task<RegularUser?> GetRegularUserByIdAsync(Guid id)
    {
        return await _context.Users.OfType<RegularUser>()
            .FirstOrDefaultAsync(x => x.Id == id);
    } 

    public async Task<MechanicUser?> GetMechanicUserByIdAsync(Guid id)
    {
        return await _context.Users.OfType<MechanicUser>()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<User?> GetByIdentityAsNoTrackAsync(params (IdentityParameter, string)[] identity)
    {
        var userQueryable = _context.Users
            .AsNoTracking().AsQueryable();

        if (identity is null || identity.Length == 0)
        {
            throw new ArgumentException("Please provide identity type you want to check at least one identity type and value");   
        }

        var predicate = PredicateBuilder.New<User>(true);

        foreach (var (parameter, value) in identity)
        {
            var normalizedIdentity = value.Trim().ToLower();

            switch (parameter)
            {
                case IdentityParameter.Username:
                    predicate = predicate.Or(x => x.Username.Trim().ToLower() == normalizedIdentity);
                    break;
                case IdentityParameter.Email:
                    predicate = predicate.Or(x => x.Email.Trim().ToLower() == normalizedIdentity
                    || (x.NewEmail != null && x.NewEmail.Trim().ToLower() == normalizedIdentity));
                    break;
                case IdentityParameter.PhoneNumber:
                    predicate = predicate.Or(x => x.PhoneNumber.Trim().ToLower() == normalizedIdentity
                    || (x.NewPhoneNumber != null && x.NewPhoneNumber.Trim().ToLower() == normalizedIdentity));
                    break;
            }
        }

        userQueryable = userQueryable.Where(predicate);

        return await userQueryable 
                .FirstOrDefaultAsync();
    }

    public async Task<User?> GetExcludingIdentityAsNoTrackAsync(Guid id, params (IdentityParameter, string)[] identity)
    {
        var userQueryable = _context.Users
            .AsNoTracking().AsQueryable();

        if (userQueryable is null || identity.Length == 0)
        {
            throw new ArgumentException("Please provide identity type you want to check at least one identity type and value");
        }

        var predicate = PredicateBuilder.New<User>(true);
          
        foreach (var (parameter, value) in identity)
        {
            var normalizedIdentity = value.Trim().ToLower();

            switch (parameter)
            {
                case IdentityParameter.Username:
                    predicate = predicate.Or(x => x.Username.Trim().ToLower() == normalizedIdentity);
                    break;
                case IdentityParameter.Email:
                    predicate = predicate.Or(x => x.Email.Trim().ToLower() == normalizedIdentity
                    || (x.NewEmail != null && x.NewEmail.Trim().ToLower() == normalizedIdentity));
                    break;
                case IdentityParameter.PhoneNumber:
                    predicate = predicate.Or(x => x.PhoneNumber.Trim().ToLower() == normalizedIdentity
                    || (x.NewPhoneNumber != null && x.NewPhoneNumber.Trim().ToLower() == normalizedIdentity));
                    break;
            }
        }

        userQueryable = userQueryable.Where(predicate);

        return await userQueryable 
                .FirstOrDefaultAsync(x => x.Id != id);
    }

    public void DeleteUser(User user)
    {
        _context.Users.Remove(user);
    }

    public Task<User?> GetUserByIdAsync(Guid id)
    {
        return _context.Users.FirstOrDefaultAsync(x => x.Id == id);
    }
}
