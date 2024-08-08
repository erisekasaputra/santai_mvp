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

    public async Task<IEnumerable<Staff>?> GetByIdsAsNoTrackAsync(params (IdentityParameter, IEnumerable<string>)[] parameters)
    {
        var asQueryable = _context.Staffs
            .AsNoTracking()
            .AsQueryable();

        if (parameters is null || parameters.Length == 0)
        {
            throw new ArgumentException("Please provide parameter type type you want to check at least one parameter type and value");
        }

        var predicate = PredicateBuilder.New<Staff>(true);

        foreach (var (identityParameter, values) in parameters)
        {
            foreach(var identity in values)
            {
                var normalizedIdentity = identity.Trim().ToLower();
                switch (identityParameter)
                {
                    case IdentityParameter.Username:
                        predicate = predicate.Or(x => x.Username.Trim().ToLower() == normalizedIdentity);
                        break;
                    case IdentityParameter.Email:
                        predicate = predicate.Or(x => (x.Email.Trim().ToLower() == normalizedIdentity) 
                            || (x.NewEmail != null && x.NewEmail.Trim().ToLower() == normalizedIdentity));
                        break;
                    case IdentityParameter.PhoneNumber:
                        predicate = predicate.Or(x => (x.PhoneNumber.Trim().ToLower() == normalizedIdentity) 
                            || (x.NewPhoneNumber != null && x.NewPhoneNumber.Trim().ToLower() == normalizedIdentity));
                        break;
                }
            } 
        }

        asQueryable = asQueryable.Where(predicate);
          
        return await asQueryable.ToListAsync();
    } 
    
    public async Task<IEnumerable<Staff>?> GetExcludingIdsAsNoTrackAsync(params (IdentityParameter, IEnumerable<(Guid id, string identity)>)[] parameters)
    {
        var queryAble = _context.Staffs
            .AsNoTracking()
            .AsQueryable();

        if (parameters is null || parameters.Length == 0)
        {
            throw new ArgumentException("Please provide parameter type type you want to check at least one parameter type and value");
        }

        var predicate = PredicateBuilder.New<Staff>(true);

        foreach (var (identityParameter, values) in parameters)
        {
            foreach (var (id, identity) in values)
            {
                var normalizedIdentity = identity.Trim().ToLower();

                switch (identityParameter)
                {
                    case IdentityParameter.Username:
                        predicate = predicate.Or(x => x.Id != id && x.Username.Trim().ToLower() == normalizedIdentity);
                        break;
                    case IdentityParameter.Email:
                        predicate = predicate.Or(x => x.Id != id && (x.Email.Trim().ToLower() == normalizedIdentity)
                            || (x.NewEmail != null && x.NewEmail.Trim().ToLower() == normalizedIdentity));
                        break; 
                    case IdentityParameter.PhoneNumber:
                        predicate = predicate.Or(x => x.Id != id && (x.PhoneNumber.Trim().ToLower() == normalizedIdentity)
                            || (x.NewPhoneNumber != null && x.NewPhoneNumber.Trim().ToLower() == normalizedIdentity));
                        break;
                }
            }
        }

        queryAble = queryAble.Where(predicate);

        return await queryAble.ToListAsync();
    }
}
